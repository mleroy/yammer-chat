using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interactivity;
using Yammer.Chat.Core.Models;
using System.Windows;

namespace Yammer.Chat.WP.Behaviors
{
    public class BottomScroller : Behavior<LongListSelector>
    {
        public object ObjectThatShouldBeInView
        {
            get { return GetValue(MessageThatShouldBeInViewProperty); }
            set { SetValue(MessageThatShouldBeInViewProperty, value); }
        }

        public static readonly DependencyProperty MessageThatShouldBeInViewProperty =
            DependencyProperty.Register("ObjectThatShouldBeInView", typeof(object), typeof(BottomScroller), new PropertyMetadata(ObjectThatShouldBeInViewChanged));

        private static void ObjectThatShouldBeInViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                ((BottomScroller)d).ScrollToObject(e.NewValue);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.Loaded -= AssociatedObject_Loaded;

            base.OnDetaching();
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var source = this.AssociatedObject.ItemsSource;

            if (source.Count > 0)
            {
                this.AssociatedObject.ScrollTo(source[source.Count - 1]);
            }
        }

        private void ScrollToObject(object message)
        {
            if (message != null)
            {
                var source = this.AssociatedObject.ItemsSource as ICollection<Message>;

                if (source.Contains(message))
                {
                    this.AssociatedObject.ScrollTo(message);
                }
            }
        }
    }
}
