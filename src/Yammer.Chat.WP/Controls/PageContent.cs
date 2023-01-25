using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Yammer.Chat.WP.Controls
{
    public class PageContent : ContentControl
    {
        private Button PrimaryActionButton;
        private Button SecondaryActionButton;

        #region Header

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(PageContent), null);

        #endregion

        #region Primary action

        public ContentControl PrimaryActionControl
        {
            get { return (ContentControl)GetValue(PrimaryActionControlProperty); }
            set { SetValue(PrimaryActionControlProperty, value); }
        }

        public static readonly DependencyProperty PrimaryActionControlProperty =
            DependencyProperty.Register("PrimaryActionControl", typeof(ContentControl), typeof(PageContent), new PropertyMetadata(null));

        public bool PrimaryActionEnabled
        {
            get { return (bool)GetValue(PrimaryActionEnabledProperty); }
            set { SetValue(PrimaryActionEnabledProperty, value); }
        }

        public static readonly DependencyProperty PrimaryActionEnabledProperty =
            DependencyProperty.Register("PrimaryActionEnabled", typeof(bool), typeof(PageContent), new PropertyMetadata(true, PrimaryActionEnabledPropertyChanged));

        private static void PrimaryActionEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
                ((PageContent)d).SetPrimaryActionEnabled();
        }

        #endregion

        #region Secondary action

        public ContentControl SecondaryActionControl
        {
            get { return (ContentControl)GetValue(SecondaryActionControlProperty); }
            set { SetValue(SecondaryActionControlProperty, value); }
        }

        public static readonly DependencyProperty SecondaryActionControlProperty =
            DependencyProperty.Register("SecondaryActionControl", typeof(ContentControl), typeof(PageContent), new PropertyMetadata(null));

        #endregion

        public event EventHandler PrimaryActionTap;
        public event EventHandler SecondaryActionTap;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PrimaryActionButton = this.GetTemplateChild("PrimaryActionButton") as Button;
            this.SecondaryActionButton = this.GetTemplateChild("SecondaryActionButton") as Button;

            if (this.PrimaryActionButton != null)
                this.PrimaryActionButton.Click += PrimaryActionButton_Click;

            if (this.SecondaryActionButton != null)
                this.SecondaryActionButton.Click += SecondaryActionButton_Click;

            this.SetPrimaryActionEnabled();
        }

        private void PrimaryActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.PrimaryActionTap != null)
                this.PrimaryActionTap(this, e);
        }

        private void SecondaryActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.SecondaryActionTap != null)
                this.SecondaryActionTap(this, e);
        }

        private void SetPrimaryActionEnabled()
        {
            if (this.PrimaryActionButton != null)
                this.PrimaryActionButton.Visibility = this.PrimaryActionEnabled ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
