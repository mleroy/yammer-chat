using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interactivity;

namespace Yammer.Chat.WP.Triggers
{
    public class LoadMore : TriggerBase<LongListSelector>
    {
        public enum TriggerDirection { Top, Bottom };

        public int Threshold { get; set; }

        public TriggerDirection Direction { get; set; }

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.ItemRealized += AssociatedObject_ItemRealized;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.AssociatedObject.ItemRealized -= AssociatedObject_ItemRealized;
        }

        void AssociatedObject_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (e.ItemKind != LongListSelectorItemKind.Item)
                return;

            var items = this.AssociatedObject.ItemsSource;
            var item = e.Container.Content;

            if ((this.Direction == TriggerDirection.Top && items.IndexOf(item) <= Threshold)
             || (this.Direction == TriggerDirection.Bottom && items.IndexOf(item) >= items.Count - Threshold - 1))
            {
                InvokeActions(null);
            }
        }
    }
}
