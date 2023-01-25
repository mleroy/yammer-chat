using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core;

namespace Yammer.Chat.WP
{
    public class Emailer : IEmailer
    {
        public void Open(string to, string subject)
        {
            var task = new EmailComposeTask();

            task.To = to;
            task.Subject = subject;

            task.Show();
        }
    }
}
