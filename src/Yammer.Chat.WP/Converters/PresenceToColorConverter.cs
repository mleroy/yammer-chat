using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Yammer.Chat.Core.Models;

namespace Yammer.Chat.WP.Converters
{
    public class PresenceToColorConverter : IValueConverter
    {
        public Brush OfflineBrush { get; set; }
        public Brush MobileBrush { get; set; }
        public Brush OnlineBrush { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is UserPresence))
            {
                throw new ArgumentException("Converter expects user presence value type");
            }

            switch ((UserPresence)value)
            {
                case UserPresence.Offline:
                case UserPresence.Idle:
                    return this.OfflineBrush;

                case UserPresence.Mobile:
                    return this.MobileBrush;

                case UserPresence.Online:
                    return this.OnlineBrush;

                case UserPresence.Unknown:
                default:
                    return new SolidColorBrush(Colors.Transparent);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
