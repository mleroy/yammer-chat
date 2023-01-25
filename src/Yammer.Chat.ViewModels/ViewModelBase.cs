using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.ViewModels
{
    public class ViewModelBase : Screen
    {
        public void SetProperty<T>(ref T property, T data, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(property, data))
            {
                return;
            }

            property = data;
            this.NotifyOfPropertyChange(propertyName);
        }
    }
}
