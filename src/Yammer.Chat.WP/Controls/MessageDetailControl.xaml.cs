using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Yammer.Chat.Core.Models;
using System.Net.Http;
using Yammer.Chat.Core.API;

namespace Yammer.Chat.WP.Controls
{
    public partial class MessageDetailControl : UserControl
    {
        public MessageDetailControl()
        {
            InitializeComponent();
        }

        public Message Message
        {
            get { return (Message)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(Message), typeof(MessageDetailControl), new PropertyMetadata(null));

        public IHttpService HttpService
        {
            get { return (IHttpService)GetValue(HttpServiceProperty); }
            set { SetValue(HttpServiceProperty, value); }
        }

        public static readonly DependencyProperty HttpServiceProperty =
            DependencyProperty.Register("HttpService", typeof(IHttpService), typeof(MessageDetailControl), new PropertyMetadata(null));
    }
}
