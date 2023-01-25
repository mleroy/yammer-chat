using Cometd.Bayeux;
using Cometd.Bayeux.Client;
using Cometd.Client;
using Cometd.Client.Transport;
using Microsoft.Devices;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using Yammer.Chat.Core;
using Yammer.Chat.Core.API.Dtos;
using Yammer.Chat.Core.Models;
using Yammer.Chat.Core.Repositories;

namespace Yammer.Chat.WP.Core
{
    public class RealtimeManager : IRealtimeManager, IRealtimeInfoListener
    {
        private readonly IIdentityStore identityStore;
        private readonly IThreadRepository threadRepository;
        private readonly IRealtimeRepository realtimeRepository;
        private readonly IVibrator vibrator;

        private BayeuxClient client;

        private RealtimeInfo realtimeInfo;

        public RealtimeManager(IIdentityStore identityStore, IThreadRepository threadRepository, IRealtimeRepository realtimeRepository, IVibrator vibrator)
        {
            this.identityStore = identityStore;
            this.threadRepository = threadRepository;
            this.realtimeRepository = realtimeRepository;
            this.vibrator = vibrator;
        }

        public void Initialize()
        {
            this.realtimeRepository.SetRealtimeInfoListener(this);
        }

        public void onRealtimeInfoChanged()
        {
            this.Setup();
        }

        public void Setup()
        {
            if (this.client != null && this.client.Connected)
            {
                return;
            }

            this.realtimeInfo = this.realtimeRepository.GetRealtimeInfo();

            if (this.realtimeInfo == null)
            {
                return;
            }

            this.CreateClient();

            this.Handshake();
        }

        private void CreateClient()
        {
            this.client = new BayeuxClient(this.realtimeInfo.Uri, new List<ClientTransport> { new LongPollingTransport(null) });
        }

        private void Handshake()
        {
            this.client.getChannel(Channel_Fields.META_HANDSHAKE).addListener(new HandshakeMessageListener(this.SubscribeToChannels));

            this.client.handshake(new Dictionary<string, object> { 
                { "ext", new { 
                    token = this.identityStore.Token, 
                    auth = "oauth",
                    push_message_bodies = true }
                }});
        }

        private void SubscribeToChannels()
        {
            var primaryChannel = this.client.getChannel(this.realtimeInfo.PrimaryChannelId);
            var secondaryChannel = this.client.getChannel(this.realtimeInfo.SecondaryChannelId);

            var messageListener = new MessageListener(this.threadRepository, this.vibrator, this.identityStore);

            this.client.batch(new BatchDelegate(() =>
            {
                primaryChannel.subscribe(messageListener);
                secondaryChannel.subscribe(messageListener);
            }));
        }

        public void Disconnect()
        {
            // Bayeux spec doesn't indicate that unsubscribing from channels is necessary. So let's not do it.
            if (this.client != null && this.client.Connected)
            {
                this.client.disconnect();
            }
        }

        public bool IsClientConnected()
        {
            return this.client != null && this.client.Connected;
        }
    }

    public class MessageListener : IMessageListener
    {
        private readonly IThreadRepository threadRepository;
        private readonly IVibrator vibrator;
        private readonly IIdentityStore identityStore;

        public MessageListener(IThreadRepository threadRepository, IVibrator vibrator, IIdentityStore identityStore)
        {
            this.threadRepository = threadRepository;
            this.vibrator = vibrator;
            this.identityStore = identityStore;
        }

        public void onMessage(IClientSessionChannel channel, IMessage message)
        {
            var realtimeDataDto = JsonConvert.DeserializeObject<RealtimeDataDto>(message.Data.ToString());

            Deployment.Current.Dispatcher.BeginInvoke(async () =>
            {
                await this.threadRepository.AddMessages(realtimeDataDto.MessagesEnvelope, markThreadsAsUnread: true);

                // Only vibrate for messages from other people. This probably doesn't belong here.
                if (realtimeDataDto.MessagesEnvelope.Messages.Any(m => m.SenderId != identityStore.UserId))
                {
                    this.vibrator.Vibrate(TimeSpan.FromMilliseconds(10));
                }
            });
        }
    }

    public class HandshakeMessageListener : IMessageListener
    {
        private Action onHandshake;

        public HandshakeMessageListener(Action onHandshake)
        {
            this.onHandshake = onHandshake;
        }

        public void onMessage(IClientSessionChannel channel, IMessage message)
        {
            if (this.onHandshake != null)
            {
                this.onHandshake();
            }
        }
    }
}
