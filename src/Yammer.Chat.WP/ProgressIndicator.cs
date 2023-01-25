using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Windows.Controls;
using System.Windows.Navigation;
using Yammer.Chat.Core;

namespace Yammer.Chat.WP.Core
{
    public class ProgressIndicator : IProgressIndicator
    {
        private readonly Microsoft.Phone.Shell.ProgressIndicator progressIndicator;

        public ProgressIndicator(Frame rootFrame)
        {
            this.progressIndicator = new Microsoft.Phone.Shell.ProgressIndicator();

            rootFrame.Navigated += RootFrameOnNavigated;
        }

        private void RootFrameOnNavigated(object sender, NavigationEventArgs args)
        {
            var content = args.Content;
            var page = content as PhoneApplicationPage;
            if (page == null)
                return;

            page.SetValue(SystemTray.ProgressIndicatorProperty, this.progressIndicator);
        }

        IDisposable IProgressIndicator.Show()
        {
            return ((IProgressIndicator)this).Show(null);
        }

        IDisposable IProgressIndicator.Show(string text)
        {
            this.progressIndicator.Text = text;
            this.progressIndicator.IsIndeterminate = true;
            this.progressIndicator.IsVisible = true;

            Action action = (this as IProgressIndicator).Hide;
            return new DisposableAction(action);
        }

        void IProgressIndicator.Hide()
        {
            this.progressIndicator.IsIndeterminate = false;
            this.progressIndicator.IsVisible = false;
        }

        bool IProgressIndicator.IsShowing()
        {
            return this.progressIndicator.IsVisible;
        }
    }
}
