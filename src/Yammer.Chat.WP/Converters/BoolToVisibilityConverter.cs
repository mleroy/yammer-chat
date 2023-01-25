using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Yammer.Chat.Core;

namespace Yammer.Chat.WP.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is bool) || (parameter != null && parameter.ToString() != "I"))
            {
                throw new ArgumentException("Converter expects boolean value and optional 'I' string parameter");
            }

            var inverse = parameter != null && parameter.ToString() == "I";
            var boolValue = (bool)value;

            if (inverse)
            {
                boolValue = !boolValue;
            }

            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
