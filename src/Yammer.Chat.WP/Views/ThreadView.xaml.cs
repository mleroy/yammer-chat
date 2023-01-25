using Microsoft.Phone.Controls;
using Yammer.Chat.ViewModels;

namespace Yammer.Chat.WP.Views
{
    public partial class ThreadView : PhoneApplicationPage
    {
        public ThreadView()
        {
            InitializeComponent();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            var viewModel = this.DataContext as ThreadViewModel;

            if (viewModel != null)
            {
                if (this.MessageDetailControl.Visibility == System.Windows.Visibility.Visible)
                {
                    viewModel.SelectedMessage = null;

                    e.Cancel = true;
                }
            }

            base.OnBackKeyPress(e);
        }
    }
}