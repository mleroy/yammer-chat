using Microsoft.Phone.Scheduler;
using Microsoft.Practices.Unity;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using Yammer.Chat.Core;
using Yammer.Chat.Core.API;
using Yammer.Chat.Core.Parsers;
using Yammer.Chat.Core.Repositories;
using Yammer.Chat.Core.Services;
using Yammer.Chat.WP.Core;

namespace Yammer.Chat.WP.BackgroundAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        UnityContainer container;

        static ScheduledAgent()
        {
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        protected override async void OnInvoke(ScheduledTask task)
        {
            Configure();

            Initialize();

            var count = await GetUnreadCount();

            UpdateTile(count);

            NotifyComplete();
        }

        /// <summary>
        /// Gets the count of unread threads since the last time the app was opened. 
        /// This is the standard way live tiles operate.
        /// </summary>
        private async Task<int> GetUnreadCount()
        {
            var threadRepository = container.Resolve<IThreadRepository>();
            var settings = container.Resolve<ISettings>();

            var lastLaunchTime = DateTime.MaxValue;
            settings.TryGetValue<DateTime>("LastActivationTime", out lastLaunchTime);
            
            var count = await threadRepository.GetUnreadThreadCount(lastLaunchTime);

            return count;
        }

        private void Configure()
        {
            container = new UnityContainer();

            container.RegisterType<IHttpClientProvider, HttpClientProvider>(new ContainerControlledLifetimeManager()); // It is recommended to re-use a single instance of HttpClient

            // Services
            container.RegisterType<IApiService, ApiService>();
            container.RegisterType<IMessagesService, MessagesService>();
            container.RegisterType<IUserService, UserService>();

            // Repositories
            container.RegisterType<IClientConfiguration, DefaultClientConfiguration>();
            container.RegisterType<IIdentityStore, IdentityStore>();
            container.RegisterType<ITokenStore, TokenStore>();
            container.RegisterType<IThreadRepository, ThreadRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<IUserRepository, UserRepository>(new ContainerControlledLifetimeManager());

            // Parsers
            container.RegisterType<IThreadParser, ThreadParser>();
            container.RegisterType<IMessageParser, MessageParser>();
            container.RegisterType<IAttachmentParser, AttachmentParser>();
            container.RegisterType<IParticipantParser, ParticipantParser>();
            container.RegisterType<IUserParser, UserParser>();

            // Phone services
            container.RegisterType<ICryptographer, Cryptographer>();
            container.RegisterType<ISettings, Settings>();
            container.RegisterType<ITileManager, TileManager>();
        }

        private void Initialize()
        {
            var identityStore = container.Resolve<IIdentityStore>();
            identityStore.AutoLogin();
        }

        private void UpdateTile(int count)
        {
            var tileManager = container.Resolve<ITileManager>();
            tileManager.UpdateTile(count);
        }
    }
}