using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows;

namespace Yammer.Chat.WP.Behaviors
{
    public class BindableFocus : Behavior<Control>
    {
        public bool HasFocus
        {
            get { return (bool)GetValue(HasFocusProperty); }
            set { SetValue(HasFocusProperty, value); }
        }

        public static readonly DependencyProperty HasFocusProperty =
            DependencyProperty.Register("HasFocus", typeof(bool), typeof(BindableFocus), new PropertyMetadata(default(bool)));

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.GotFocus += AssociatedObject_GotFocus;
            this.AssociatedObject.LostFocus += AssociatedObject_LostFocus;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.GotFocus -= AssociatedObject_GotFocus;
            this.AssociatedObject.LostFocus -= AssociatedObject_LostFocus;
            base.OnDetaching();
        }

        void AssociatedObject_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            this.HasFocus = true;
        }

        void AssociatedObject_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            this.HasFocus = false;
        }
    }
}
