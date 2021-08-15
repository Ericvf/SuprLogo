using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SuprLogo.Framework
{
    public class RoundNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            int decimals = 0;

            if (parameter.GetType() == typeof(string))
                decimals = System.Convert.ToInt32(parameter);

            if (value.GetType() == typeof(double))
                return Math.Round((double)value, decimals);

            return null;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
