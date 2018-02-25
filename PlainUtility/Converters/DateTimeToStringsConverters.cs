using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Converters
{
    /// <summary>
    /// System.DateTime convert 'HH:mm:ss' format string
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateTimeToLongTimeStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>formatted string</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (DateTime)value;
            return val.ToString("HH:mm:ss");
        }
        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>System.DateTime</returns>
        /// <exception cref="Exception">
        /// convert error : value
        /// or
        /// convert source error : source is not 'System.String'
        /// </exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val;
            try
            {
                val = (string)value;
            }
            catch
            {
                throw new Exception("System.DateTime convert source error : convert source is not 'System.String'");
            }
            if (DateTime.TryParse(val, out DateTime ret))
            {
                return ret.Date;
            }
            else
            {
                throw new Exception("System.DateTime convert error : not parsed is '" + val + "'");
            }
        }
    }
    /// <summary>
    /// System.DateTime convert 'HH:mm' format string
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateTimeToTimeStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>formatted string</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (DateTime)value;
            return val.ToString("HH:mm");
        }
        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>System.DateTime</returns>
        /// <exception cref="Exception">
        /// convert error : value
        /// or
        /// convert source error : source is not 'System.String'
        /// </exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val;
            try
            {
                val = (string)value;
            }
            catch
            {
                throw new Exception("System.DateTime convert source error : source is not 'System.String'");
            }
            if (DateTime.TryParse(val, out DateTime ret))
            {
                return ret.Date;
            }
            else
            {
                throw new Exception("System.DateTime convert error : not parsed is '" + val + "'");
            }
        }
    }
    /// <summary>
    /// System.DateTime convert 'yyyy/MM/dd' format string
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateTimeToDateStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>formatted string</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (DateTime)value;
            return val.ToString("yyyy/MM/dd");
        }
        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>System.DateTime</returns>
        /// <exception cref="Exception">
        /// convert error : value
        /// or
        /// convert source error : source is not 'System.String'
        /// </exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val;
            try
            {
                val = (string)value;
            }
            catch
            {
                throw new Exception("System.DateTime convert source error : source is not 'System.String'");
            }
            if (DateTime.TryParse(val, out DateTime ret))
            {
                return ret.Date;
            }
            else
            {
                throw new Exception("System.DateTime convert error : not parsed is '" + val + "'");
            }
        }
    }
    /// <summary>
    /// System.DateTime convert 'MM/dd' format string
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateTimeToShortDateStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>formatted string</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (DateTime)value;
            return val.ToString("MM/dd");
        }
        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>System.DateTime</returns>
        /// <exception cref="Exception">
        /// convert error : value
        /// or
        /// convert source error : source is not 'System.String'
        /// </exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val;
            try
            {
                val = (string)value;
            }
            catch
            {
                throw new Exception("System.DateTime convert source error : source is not 'System.String'");
            }
            if (DateTime.TryParse(val, out DateTime ret))
            {
                return ret.Date;
            }
            else
            {
                throw new Exception("System.DateTime convert error : not parsed is '" + val + "'");
            }
        }
    }
}
