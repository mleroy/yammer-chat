using Caliburn.Micro;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using Yammer.Chat.Core;
using Yammer.Chat.ViewModels;

namespace Yammer.Chat.WP
{
    public class Navigator : FrameAdapter, INavigator
    {
        public SessionType SessionType { get; set; }
        public bool MustClearPageStack { get; set; }

        private bool wasRelaunched = false;

        public Navigator(PhoneApplicationFrame frame)
            : base(frame)
        {
            base.Navigating += frame_Navigating;
            base.Navigated += frame_Navigated;
        }

        private void frame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            // If the session type is None or New, check the navigation Uri to determine if the
            // navigation is a deep link or if it points to the app's main page.
            if (this.SessionType == SessionType.None && e.NavigationMode == NavigationMode.New)
            {
                // This block will run if the current navigation is part of the app's intial launch

                // Keep track of Session Type 
                //if (e.Uri.ToString().Contains("DeepLink=true"))
                //{
                //    SessionType = SessionType.DeepLink;
                //}
                //else 
                if (e.Uri.ToString().Contains("/ShellView.xaml"))
                {
                    this.SessionType = SessionType.Home;
                }
            }


            if (e.NavigationMode == NavigationMode.Reset)
            {
                // This block will execute if the current navigation is a relaunch.
                // If so, another navigation will be coming, so this records that a relaunch just happened
                // so that the next navigation can use this info.
                this.wasRelaunched = true;
            }
            else if (e.NavigationMode == NavigationMode.New && this.wasRelaunched)
            {
                // This block will run if the previous navigation was a relaunch
                this.wasRelaunched = false;

                //if (e.Uri.ToString().Contains("DeepLink=true"))
                //{
                //    // This block will run if the launch Uri contains "DeepLink=true" which
                //    // was specified when the secondary tile was created in MainPage.xaml.cs

                //    SessionType = SessionType.DeepLink;
                //    // The app was relaunched via a Deep Link.
                //    // The page stack will be cleared.
                //}
                //else
                if (e.Uri.ToString().Contains("/ShellView.xaml"))
                {
                    // This block will run if the navigation Uri is the main page
                    if (this.SessionType == SessionType.DeepLink)
                    {
                        // When the app was previously launched via Deep Link and relaunched via Main Tile, we need to clear the page stack. 
                        this.SessionType = SessionType.Home;
                    }
                    else
                    {
                        if (!this.MustClearPageStack)
                        {
                            //The app was previously launched via Main Tile and relaunched via Main Tile. Cancel the navigation to resume.
                            e.Cancel = true;
                            base.Navigated -= ClearBackStackAfterReset;
                        }
                    }
                }

                this.MustClearPageStack = false;
            }
        }

        public void frame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            // If the app has received a 'reset' navigation, then we need to check
            // on the next navigation to see if the page stack should be reset
            if (e.NavigationMode == NavigationMode.Reset)
                base.Navigated += ClearBackStackAfterReset;
        }

        public void Navigate<TViewModel>()
        {
            var uriBuilder = this.UriFor<TViewModel>();
            uriBuilder.Navigate();

            Analytics.Default.LogPageView(uriBuilder.BuildUri().ToString());
        }

        public new void RemoveBackEntry()
        {
            base.RemoveBackEntry();
        }

        public void ClearHistory()
        {
            while (base.RemoveBackEntry() != null) { }
        }

        public void Navigate<TViewModel, TValue>(Expression<Func<TViewModel, TValue>> property, TValue value, NavigationFlags flags)
        {
            var uri = this.UriFor<TViewModel>()
                .WithParam<TValue>(property, value)
                .BuildUri();

            var targetPageIsInBackStack = this.BackStack.Any(x => relativeUrisAreEqual(
                x.Source, uri,
                flags.HasFlag(NavigationFlags.IgnoreParametersForBackStackLookup)));

            // Go back to previous page(s) if it exists in backstack
            if (targetPageIsInBackStack)
            {
                while (!relativeUrisAreEqual(
                        this.BackStack.First().Source, uri,
                        flags.HasFlag(NavigationFlags.IgnoreParametersForBackStackLookup)))
                {
                    this.RemoveBackEntry();
                }

                this.GoBack();
            }
            else
            {
                var uriBuilder = this.UriFor<TViewModel>()
                     .WithParam<TValue>(property, value);
                uriBuilder.Navigate();

                Analytics.Default.LogPageView(uriBuilder.BuildUri().ToString());

                if (flags.HasFlag(NavigationFlags.RemoveCurrentPageFromBackStack))
                {
                    this.RemoveBackEntry();
                }
            }
        }

        public new void GoBack()
        {
            base.GoBack();
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Unregister the event so it doesn't get called again
            base.Navigated -= ClearBackStackAfterReset;

            // Only clear the stack for 'new' (forward) and 'refresh' navigations
            if (e.NavigationMode != NavigationMode.New)
            {
                return;
            }

            // For UI consistency, clear the entire page stack
            this.ClearHistory();
        }

        private bool relativeUrisAreEqual(Uri uri1, Uri uri2, bool ignoreParameters)
        {
            return ignoreParameters
                ? getRelativeUriWithoutParameters(uri1) == getRelativeUriWithoutParameters(uri2)
                : uri1.Equals(uri2);
        }

        private string getRelativeUriWithoutParameters(Uri uri)
        {
            if (uri.OriginalString.IndexOf("?") < 0)
            {
                return uri.OriginalString;
            }

            return uri.OriginalString.Substring(0, uri.OriginalString.IndexOf("?"));
        }
    }
}
