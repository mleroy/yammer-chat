using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using Yammer.Chat.Core.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Yammer.Chat.Core.API;

namespace Yammer.Chat.WP.Controls
{
    public partial class MessageControl : UserControl
    {
        // The logic to handle single/double taps should live in a reusable component (behavior, trigger, attached event?)
        // I couldn't figure out how to add a handler to an attached event with Caliburn
        public event EventHandler SingleTap;
        public new event EventHandler DoubleTap;

        private bool isDoubleTap;

        public MessageControl()
        {
            InitializeComponent();
        }

        protected async override void OnTap(System.Windows.Input.GestureEventArgs e)
        {
            base.OnTap(e);

            this.isDoubleTap = false;

            await Task.Delay(200);

            if (!this.isDoubleTap)
            {
                if (this.SingleTap != null)
                {
                    this.SingleTap(this, e);
                }
            }
        }

        protected override void OnDoubleTap(System.Windows.Input.GestureEventArgs e)
        {
            base.OnDoubleTap(e);

            this.isDoubleTap = true;

            if (this.DoubleTap != null)
            {
                this.DoubleTap(this, e);
            }
        }

        public Message Message
        {
            get { return (Message)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(Message), typeof(MessageControl), new PropertyMetadata(null));

        public Brush BubbleBackground
        {
            get { return (Brush)GetValue(BubbleBackgroundProperty); }
            set { SetValue(BubbleBackgroundProperty, value); }
        }

        public static readonly DependencyProperty BubbleBackgroundProperty =
            DependencyProperty.Register("BubbleBackground", typeof(Brush), typeof(MessageControl), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        public Brush BodyForeground
        {
            get { return (Brush)GetValue(BodyForegroundProperty); }
            set { SetValue(BodyForegroundProperty, value); }
        }

        public static readonly DependencyProperty BodyForegroundProperty =
            DependencyProperty.Register("BodyForeground", typeof(Brush), typeof(MessageControl), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public Brush LikeForeground
        {
            get { return (Brush)GetValue(LikeForegroundProperty); }
            set { SetValue(LikeForegroundProperty, value); }
        }

        public static readonly DependencyProperty LikeForegroundProperty =
            DependencyProperty.Register("LikeForeground", typeof(Brush), typeof(MessageControl), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public IHttpService HttpService
        {
            get { return (IHttpService)GetValue(HttpServiceProperty); }
            set { SetValue(HttpServiceProperty, value); }
        }

        public static readonly DependencyProperty HttpServiceProperty =
            DependencyProperty.Register("HttpService", typeof(IHttpService), typeof(MessageControl), new PropertyMetadata(null));
    }
}
