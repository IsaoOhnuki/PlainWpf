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
using Mvvm;
using System.Windows.Input;
using System.ComponentModel;
using MvvmOption;

namespace PlainWpf.ViewModels
{
    public class SubPage2ViewModel : BindableBase
    {
        private Orientation orientation;
        public Orientation Orientation
        {
            get { return orientation; }
            set { SetProperty(ref orientation, value); }
        }

        public DelegateCommand OrientationChangeCommand { get; set; }

        public SubPage2ViewModel()
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

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private bool IsOk
        {
            get { return !string.IsNullOrWhiteSpace(Name); }
        }

        private ICommand _helloCommand;
        public ICommand Hello
        {
            get
            {
                if (_helloCommand == null)
                {
                    _helloCommand = new DelegateCommand(
                        () => this.HelloAction(),
                        () => this.IsOk);
                }
                return _helloCommand;
            }
        }

        public void HelloAction()
        {
            var result = MessengerService.SendMessage(typeof(SubPage2ViewModel), new OkCancelDialogRequestMessage { Title = "", Content = Name + "さん、こんにちは。" });
            if (((OkCancelDialogRequestMessage)result).ConfirmationDialogResult == ConfirmationDialogResult.Ok)
            {
                Name = "";
            }
        }
        //#region INotifyPropertyChanged メンバ

        //public event PropertyChangedEventHandler PropertyChanged;

        //protected void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChangedEventHandler handler = this.PropertyChanged;
        //    if (handler != null)
        //        handler(this, new PropertyChangedEventArgs(propertyName));
        //}
        //#endregion
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
