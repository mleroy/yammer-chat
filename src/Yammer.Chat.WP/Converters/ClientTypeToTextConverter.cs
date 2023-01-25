using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Yammer.Chat.Core;
using Yammer.Chat.Core.Models;
using Yammer.Chat.Core.Resources;

namespace Yammer.Chat.WP.Converters
{
    public class ClientTypeToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ClientType))
            {
                throw new ArgumentException("Converter expects ClientType value");
            }

            var clientType = (ClientType)value;

            switch (clientType)
            {
                case ClientType.Mobile:
                    return AppResources.SentFromMobileText;
                case ClientType.Web:
                default:
                    return AppResources.SentFromWebText;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
