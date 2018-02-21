using Utilitys;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PlainWpf.ViewModels
{
    public class SabPage2ViewModel : BindableBase
    {
        private Orientation orientation;
        public Orientation Orientation
        {
            get { return orientation; }
            set { SetProperty(ref orientation, value); }
        }

        public DelegateCommand OrientationChangeCommand { get; set; }

        public SabPage2ViewModel()
        {
            OrientationChangeCommand = new DelegateCommand(() => {
                if (Orientation == Orientation.Horizontal)
                {
                    Orientation = Orientation.Vertical;
                }
                else
                {
                    Orientation = Orientation.Horizontal;
                }
            });
        }

        private bool canCommand;
        public bool CanCommand
        {
            get { return canCommand; }
            set { SetProperty(ref canCommand, value); }
        }

        public DelegateCommand Command { get; set; }
    }

    public class OrientationToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Orientation)value) == Orientation.Vertical;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Orientation.Vertical : Orientation.Horizontal;
        }
    }

    public class OrientationHorizontalToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Orientation)value) == Orientation.Horizontal ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Visibility)value) == Visibility.Visible ? Orientation.Horizontal : Orientation.Vertical;
        }
    }

    public class OrientationVerticalToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Orientation)value) == Orientation.Vertical ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Visibility)value) == Visibility.Visible ? Orientation.Vertical : Orientation.Horizontal;
        }
    }
}
