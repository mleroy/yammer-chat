using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;
using Telerik.Windows.Controls;

namespace Yammer.Chat.WP.Triggers
{
    public class SelectedListItemObserver : TriggerBase<RadDataBoundListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
        }

        private void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.AssociatedObject.SelectedItem != null)
            {
                this.InvokeActions(this.AssociatedObject.SelectedItem);

                // Resetting SelectedItem is necessary, otherwise this event will not fire for a second tap on the same item.
                this.AssociatedObject.SelectedItem = null;
            }
        }
    }
}
