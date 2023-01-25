using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core
{
    public interface IResumeManager
    {
        bool NeedsToRefreshThreads { get; set; }
        bool NeedsToRefreshThread { get; set; }
    }

    public class ResumeManager : IResumeManager
    {
        public bool NeedsToRefreshThreads { get; set; }
        public bool NeedsToRefreshThread { get; set; }
    }
}
