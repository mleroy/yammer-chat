using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core.Models;

namespace Yammer.Chat.Core
{
    public interface IRealtimeManager
    {
        void Initialize();
        void Setup();
        void Disconnect();
        bool IsClientConnected();
    }
}
