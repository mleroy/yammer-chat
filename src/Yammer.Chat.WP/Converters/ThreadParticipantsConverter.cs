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
    public class ThreadParticipantsConverter : IValueConverter
    {
        public long CurrentUserId { get; set; }

        public ThreadParticipantsConverter()
        {
            // Using this anti-pattern as we apparently can't bind a property of a converter (because it isn't a frameworkelement)
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

            participants = participants.Where(p => p.Id != this.CurrentUserId);

            if (participants.Count() == 1)
            {
                return string.Join(", ", participants.Select(p => p.FullName));
            }
            else if (participants.Count() <= 3)
            {
                return string.Join(", ", participants.Select(p => p.FirstName));
            }
            else
            {
                var names = string.Join(", ", participants.Take(3).Select(p => p.FirstName));

                return string.Format("{0} +{1}", names, participants.Count() - 3);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
