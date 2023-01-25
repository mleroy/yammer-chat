using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using Yammer.Chat.Core.Models;
using Yammer.Chat.Core.Repositories;
using Yammer.Chat.Core.Resources;

namespace Yammer.Chat.WP.Converters
{
    public class ThreadMugshotConverter : IValueConverter
    {
        public IValueConverter MugshotTemplateConverter { get; set; }
        
        public long CurrentUserId { get; set; }

        public ThreadMugshotConverter()
        {
            var identityStore = IoC.Get<IIdentityStore>();

            if (identityStore != null)
            {
                this.CurrentUserId = identityStore.UserId;
            }
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var participants = value as IEnumerable<User>;

            if (participants == null || participants.Count() < 2)
            {
                throw new ArgumentException("Converter expects an enumerable of at least 2 participants");
            }

            if (this.MugshotTemplateConverter == null)
            {
                throw new ArgumentNullException("MugshotTemplateConverter");
            }

            participants = participants.Where(p => p.Id != this.CurrentUserId);

            if (participants.Count() == 1)
            {
                return this.MugshotTemplateConverter.Convert(participants.First().MugshotTemplate, targetType, parameter, culture);
            }
            else
            {
                return "/Assets/groups.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
