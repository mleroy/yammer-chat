using System;
using System.Collections;
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
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var inverse = parameter != null && parameter.ToString() == "I";

            var isNull = value == null;

            // Take care of specific types that have an equivalent null state
            if (value is string)
            {
                isNull = string.IsNullOrEmpty(value.ToString());
            }

            if (value is int)
            {
                isNull = (int)value == 0;
            }

            if (value is IEnumerable<object>)
            {
                isNull = ((IEnumerable<object>)value).Count() == 0;
            }

            if (inverse)
            {
                isNull = !isNull;
            }

            return isNull ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
