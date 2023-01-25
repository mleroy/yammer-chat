using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core
{
    public interface IApplicationVersion
    {
        string VersionNumber { get; }
    }
}
