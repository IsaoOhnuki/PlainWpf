using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace Converters
{
    [ValueConversion(typeof(Color), typeof(Brush))]
    class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = (Color)value;
            Brush ret = new SolidColorBrush(val);
            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = ((SolidColorBrush)value).Color;
            return (Color)val;
        }
    }
}
