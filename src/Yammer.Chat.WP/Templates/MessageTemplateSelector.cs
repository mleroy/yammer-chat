using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Yammer.Chat.Core.Models;

namespace Yammer.Chat.WP.Templates
{
    public class MessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FromCurrentUserTemplate { get; set; }
        public DataTemplate FromOtherTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var message = item as Message;

            if (message != null)
            {
                return message.IsFromCurrentUser
                    ? this.FromCurrentUserTemplate
                    : this.FromOtherTemplate;
            }

            return base.SelectTemplate(item, container);
        }
        
    }
}
