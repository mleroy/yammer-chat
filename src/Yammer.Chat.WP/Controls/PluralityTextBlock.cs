using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Yammer.Chat.WP.Controls
{
    [TemplatePart(Name = textBlockPartName, Type = typeof(TextBlock))]
    public class PluralityTextBlock : Control
    {
        private const string textBlockPartName = "PART_TextBlock";

        private TextBlock _textBlock;

        #region Dependency Properties

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.RegisterAttached("Source", typeof(object), typeof(PluralityTextBlock), new PropertyMetadata(OnSourceChanged));

        public static readonly DependencyProperty ZeroValueProperty =
            DependencyProperty.RegisterAttached("ZeroValue", typeof(string), typeof(PluralityTextBlock), new PropertyMetadata(string.Empty, Refresh));

        public static readonly DependencyProperty SingularValueProperty =
            DependencyProperty.RegisterAttached("SingularValue", typeof(string), typeof(PluralityTextBlock), new PropertyMetadata(string.Empty, Refresh));

        public static readonly DependencyProperty PluralValueProperty =
            DependencyProperty.RegisterAttached("PluralValue", typeof(string), typeof(PluralityTextBlock), new PropertyMetadata(string.Empty, Refresh));

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PluralityTextBlock;

            if (e.OldValue != null && e.OldValue is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)e.OldValue).CollectionChanged -= control.Source_CollectionChanged;
            }

            if (e.NewValue != null && e.NewValue is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)e.NewValue).CollectionChanged += control.Source_CollectionChanged;
            }

            Refresh(d, e);
        }

        private void Source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.Refresh();
        }

        public static void Refresh(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PluralityTextBlock)d).Refresh();
        }

        public object Source
        {
            get { return GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public string ZeroValue
        {
            get { return GetValue(ZeroValueProperty) as string; }
            set { SetValue(ZeroValueProperty, value); }
        }

        public string SingularValue
        {
            get { return GetValue(SingularValueProperty) as string; }
            set { SetValue(SingularValueProperty, value); }
        }

        public string PluralValue
        {
            get { return GetValue(PluralValueProperty) as string; }
            set { SetValue(PluralValueProperty, value); }
        }

        #endregion

        public PluralityTextBlock()
        {
            DefaultStyleKey = typeof(PluralityTextBlock);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _textBlock = GetTemplateChild(textBlockPartName) as TextBlock ?? new TextBlock();

            Refresh();
        }

        private void Refresh()
        {
            if (_textBlock == null)
            {
                return; // template not initialized
            }

            var count = GetCount();

            if (count == 0)
            {
                _textBlock.Text = ZeroValue;
            }
            else if (count == 1)
            {
                _textBlock.Text = SingularValue;
            }
            else
            {
                _textBlock.Text = string.Format(PluralValue, count);
            }
        }

        private long GetCount()
        {
            var source = Source;
            if (source == null)
            {
                return 0;
            }

            var convertible = source as IConvertible;
            if (convertible != null)
            {
                return convertible.ToInt64(CultureInfo.InvariantCulture);
            }

            var enumerable = source as IEnumerable;
            if (enumerable != null)
            {
                return enumerable.Cast<object>().Count();
            }

            return 0;
        }
    }
}