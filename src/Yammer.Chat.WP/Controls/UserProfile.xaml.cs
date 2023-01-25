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

namespace Yammer.Chat.WP.Controls
{
    public partial class UserProfile : UserControl
    {
        public User User
        {
            get { return (User)GetValue(UserProperty); }
            set { SetValue(UserProperty, value); }
        }

        public static readonly DependencyProperty UserProperty =
            DependencyProperty.Register("User", typeof(User), typeof(UserProfile), new PropertyMetadata(null));

        public UserProfile()
        {
            InitializeComponent();
        }
    }
}
