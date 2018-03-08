using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace DatabaseUtility
{
    public class ConnectionAuthenticateWindowsToBooleanConverter : IValueConverter
    {
        // Enum → bool
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ServerConnectionAuthenticate val = (ServerConnectionAuthenticate)value;
            return val == ServerConnectionAuthenticate.Windows;
        }

        // bool → Enum
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool val = (bool)value;
            return val ? ServerConnectionAuthenticate.Windows : ServerConnectionAuthenticate.SQLServer;
        }
    }

    public class ConnectionAuthenticateSQLServerToBooleanConverter : IValueConverter
    {
        // Enum → bool
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ServerConnectionAuthenticate val = (ServerConnectionAuthenticate)value;
            return val == ServerConnectionAuthenticate.SQLServer;
        }

        // bool → Enum
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool val = (bool)value;
            return val ? ServerConnectionAuthenticate.SQLServer : ServerConnectionAuthenticate.Windows;
        }
    }
}
