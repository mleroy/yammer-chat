using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Telerik.Windows.Controls;

namespace Yammer.Chat.WP.Behaviors
{
    public class ListTemplateSelector : Behavior<RadDataBoundListBox>
    {
        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(ListTemplateSelector), new PropertyMetadata(IsLoadingChanged));

        public Exception LoadingException
        {
            get { return (Exception)GetValue(LoadingExceptionProperty); }
            set { SetValue(LoadingExceptionProperty, value); }
        }

        public static readonly DependencyProperty LoadingExceptionProperty =
            DependencyProperty.Register("LoadingException", typeof(Exception), typeof(ListTemplateSelector), new PropertyMetadata(LoadingExceptionChanged));

        public DataTemplate LoadingTemplate
        {
            get { return (DataTemplate)GetValue(LoadingTemplateProperty); }
            set { SetValue(LoadingTemplateProperty, value); }
        }

        public static readonly DependencyProperty LoadingTemplateProperty =
            DependencyProperty.Register("LoadingTemplate", typeof(DataTemplate), typeof(ListTemplateSelector), null);

        public DataTemplate EmptyTemplate
        {
            get { return (DataTemplate)GetValue(EmptyTemplateProperty); }
            set { SetValue(EmptyTemplateProperty, value); }
        }

        public static readonly DependencyProperty EmptyTemplateProperty =
            DependencyProperty.Register("EmptyTemplate", typeof(DataTemplate), typeof(ListTemplateSelector), null);

        public DataTemplate ErrorTemplate
        {
            get { return (DataTemplate)GetValue(ErrorTemplateProperty); }
            set { SetValue(ErrorTemplateProperty, value); }
        }

        public static readonly DependencyProperty ErrorTemplateProperty =
            DependencyProperty.Register("ErrorTemplate", typeof(DataTemplate), typeof(ListTemplateSelector), null);

        private static void IsLoadingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (ListTemplateSelector)d;

            var isLoading = (bool)e.NewValue;
            var isEmpty = behavior.AssociatedObject.ItemsSource.Count() == 0;


            if (isEmpty)
            {
                if (isLoading)
                {
                    behavior.AssociatedObject.ListHeaderTemplate = behavior.LoadingTemplate;
                }
                else
                {
                    behavior.AssociatedObject.ListHeaderTemplate = behavior.EmptyTemplate;
                }
            }
            else
            {
                behavior.AssociatedObject.ListHeaderTemplate = null;
            }
        }

        private static void LoadingExceptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (ListTemplateSelector)d;
            var hasErrored = behavior.LoadingException != null;

            behavior.AssociatedObject.ListHeaderTemplate = hasErrored ? behavior.ErrorTemplate : null;
        }
    }
}
