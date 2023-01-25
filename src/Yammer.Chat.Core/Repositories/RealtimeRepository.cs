using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core.API.Dtos;
using Yammer.Chat.Core.Models;
using Yammer.Chat.Core.Parsers;
using Yammer.Chat.Core.Services;

namespace Yammer.Chat.Core.Repositories
{
    public interface IRealtimeInfoListener
    {
        void onRealtimeInfoChanged();
    }

    public interface IRealtimeRepository
    {
        void SetRealtimeInfo(RealtimeDto dto);

        RealtimeInfo GetRealtimeInfo();

        void SetRealtimeInfoListener(IRealtimeInfoListener realtimeInfoListener);
    }

    public class RealtimeRepository : IRealtimeRepository
    {
        private IRealtimeInfoListener realtimeInfoListener;

        private RealtimeInfo realtimeInfo;

        public RealtimeRepository()
        {
        }

        public void SetRealtimeInfo(RealtimeDto dto)
        {
            var realtimeInfo = new RealtimeInfo
            {
                Uri = dto.Uri,
                PrimaryChannelId = string.Format("/feeds/{0}/primary", dto.ChannelId),
                SecondaryChannelId = string.Format("/feeds/{0}/secondary", dto.ChannelId)
            };

            this.realtimeInfo = realtimeInfo;

            if (this.realtimeInfoListener != null)
            {
                this.realtimeInfoListener.onRealtimeInfoChanged();
            }
        }

        public RealtimeInfo GetRealtimeInfo()
        {
            return this.realtimeInfo;
        }

        public void SetRealtimeInfoListener(IRealtimeInfoListener realtimeInfoListener)
        {
            this.realtimeInfoListener = realtimeInfoListener;
        }
    }
}
