using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Yammer.Chat.ViewModels;

namespace Yammer.Chat.WP.Views
{
    public partial class SsoLoginView : PhoneApplicationPage
    {
        private SsoLoginViewModel viewModel;

        public SsoLoginView()
        {
            InitializeComponent();

            this.Loaded += SsoLoginView_Loaded;
            this.WebBrowserControl.Navigated += WebBrowserControl_Navigated;
        }

        private void SsoLoginView_Loaded(object sender, RoutedEventArgs e)
        {
            this.viewModel = (SsoLoginViewModel)this.DataContext;

            this.WebBrowserControl.Navigate(this.viewModel.InitialUri);
        }

        private async void WebBrowserControl_Navigated(object sender, NavigationEventArgs e)
        {
            await this.viewModel.Navigated(e.Uri);
        }
    }
}