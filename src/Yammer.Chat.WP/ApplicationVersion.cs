using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yammer.Chat.Core;

namespace Yammer.Chat.WP
{
    public class ApplicationVersion : IApplicationVersion
    {
        public string VersionNumber
        {
            get
            {
                // Major/minor only... rest seems superfluous
                return Regex.Match(typeof(App).Assembly.FullName, @"(\d+)(.\d+)(.\d+)").ToString();
            }
        }
    }
}
