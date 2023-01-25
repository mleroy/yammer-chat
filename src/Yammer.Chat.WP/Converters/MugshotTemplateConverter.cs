using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using Yammer.Chat.Core.Resources;

namespace Yammer.Chat.WP.Converters
{
    public class MugshotTemplateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var size = parameter is double ? (double)parameter : 80;
            var url = value as string;

            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            // Given size is in logical pixels
            // Request higher resolution image so that it looks good on all resolutions
            size = (int)(size * 1.6); // 1.6 is the WXGA-to-logical ratio and is standard

            return url
                .Replace("{width}", size.ToString())
                .Replace("{height}", size.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
