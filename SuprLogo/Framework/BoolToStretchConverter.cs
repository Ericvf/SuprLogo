using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SuprLogo.Framework
{
    public class BoolToStretchConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value.GetType() == typeof(bool))
                return (bool)value
                    ? Stretch.Uniform
                    : Stretch.None;

            return null;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
