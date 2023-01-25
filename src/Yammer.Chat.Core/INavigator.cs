using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core
{
    [Flags]
    public enum NavigationFlags
    {
        None,

        RemoveCurrentPageFromBackStack,

        /// <summary>
        /// When navigating, we first look if the target page is in our history. If it is, we navigate back instead of forward.
        /// To determine if the target page is in our history, we can compare Uri's including query parameters or not.
        /// </summary>
        IgnoreParametersForBackStackLookup
    }

    public interface INavigator
    {
        void Navigate<TViewModel>();

        void Navigate<TViewModel, TValue>(Expression<Func<TViewModel, TValue>> property, TValue value, NavigationFlags flags = NavigationFlags.None);

        void RemoveBackEntry();

        void ClearHistory();

        void GoBack();

        bool MustClearPageStack { get; set; }
        SessionType SessionType { get; set; }
    }
}
