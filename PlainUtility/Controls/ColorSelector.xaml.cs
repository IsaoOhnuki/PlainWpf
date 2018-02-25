using Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Controls
{
    /// <summary>
    /// Interaction logic for ColorSelector
    /// </summary>
    public partial class ColorSelector : UserControl, INotifyPropertyChanged
    {
        //http://www.losttechnology.jp/WebDesign/colorlist.html さまざまな色一覧
        public ObservableCollection<ColorPlus> ColorEnum { get; set; }

        public ColorPlus SelectedColorPlus
        {
            get { return (ColorPlus)GetValue(SelectedColorPlusProperty); }
            set { SetValue(SelectedColorPlusProperty, value); }
        }

        internal static readonly DependencyProperty SelectedColorPlusProperty = DependencyProperty.Register(
            nameof(SelectedColorPlus),
            typeof(ColorPlus),
            typeof(ColorSelector),
            new PropertyMetadata(new ColorPlus(), (d, e) =>
            {
                if (e.NewValue == e.OldValue)
                    return;
                ColorSelector obj = d as ColorSelector;
                obj.SelectedColor = ((ColorPlus)e.NewValue)?.Color ?? Colors.White;
                obj.TextColor = ((ColorPlus)e.NewValue)?.TextColor ?? Colors.Black;
                obj.TextName = ((ColorPlus)e.NewValue)?.Name ?? "Black";
            }));

        private Color textColor = Colors.Black;
        public Color TextColor
        {
            get { return textColor; }
            private set
            {
                textColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TextColor)));
            }
        }

        private string textName = "Black";
        public string TextName
        {
            get { return textName; }
            private set
            {
                textName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TextName)));
            }
        }

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(
            nameof(SelectedColor),
            typeof(Color),
            typeof(ColorSelector),
            new FrameworkPropertyMetadata(defaultValue: Colors.White, flags: FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, propertyChangedCallback: (d, e) =>
            {
                if (e.NewValue == e.OldValue)
                    return;
                ColorSelector obj = d as ColorSelector;
                obj.SelectedColorPlus = obj.ColorEnum.Where(x => x.Color == (Color)e.NewValue).FirstOrDefault();
            }));

        public event PropertyChangedEventHandler PropertyChanged;

        private class ColorStruct
        {
            public Color Color { get; set; }
            public string Name { get; set; }
            public override bool Equals(object obj)
            {
                ColorStruct v = obj as ColorStruct;
                return v != null && Color.Equals(v.Color);
            }
            public override int GetHashCode()
            {
                return Color.GetHashCode();
            }
        }

        public ColorSelector()
        {
            var colList = new List<ColorStruct>(typeof(Colors).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(x => x.Name != nameof(Colors.Transparent))
                .Select(x => new ColorStruct { Color = (Color)x.GetValue(null, null), Name = x.Name }).Distinct());

            ColorEnum = new ObservableCollection<ColorPlus>(colList
                .Select(x => new ColorPlus
                {
                    Color = x.Color,
                    TextColor = BackColorToTextColor(x.Color),
                    Name = x.Name,
                    TextColorName = BackColorToTextColor(x.Color).ToString(),
                    Hue = ColorToHue(x.Color),
                    Lightness = ColorToLightness(x.Color) == 0 ? 0 : ColorToLightness(x.Color) < 0.25 ? 0.25 : ColorToLightness(x.Color) < 0.5 ? 0.49 : ColorToLightness(x.Color) == 0.5 ? 0.5 : ColorToLightness(x.Color) < 0.75 ? 0.75 : 1,
                    Brightness = ColorToBrightness(x.Color) == 0 ? 0 : ColorToBrightness(x.Color) < 0.2 ? 0.2 : ColorToBrightness(x.Color) < 0.4 ? 0.4 : ColorToBrightness(x.Color) < 0.6 ? 0.6 : ColorToBrightness(x.Color) < 0.8 ? 0.8 : 1,
                    Saturation = ColorToSaturation(x.Color) == 0 ? 0 : ColorToSaturation(x.Color) < 0.2 ? 0.2 : ColorToSaturation(x.Color) < 0.4 ? 0.4 : ColorToSaturation(x.Color) < 0.6 ? 0.6 : ColorToSaturation(x.Color) < 0.8 ? 0.8 : 1,
                })
                .OrderByDescending(x => ColorToGray(x.Color))
                .ThenByDescending(x => x.Color.R == 0 && x.Color.G == 0 && x.Color.B == 0 ? 1 : x.Lightness)
                .ThenBy(x => x.Color.R == 0 && x.Color.G == 0 && x.Color.B == 0 ? 0 : x.Saturation)
                .ThenBy(x => x.Hue)
                );

            SelectedColorPlus = ColorEnum.Where(x => x.Color == Colors.White).FirstOrDefault();

            InitializeComponent();

            comboBox.DataContext = this;
        }

        public double ColorToGray(Color col)
        {
            if (col.R == col.G && col.R == col.B)
                return (double)col.R / 255;
            else
                return 0;
        }

        public static double ColorToValue(Color col)
        {
            return (
                //((double)col.A / 255)
                //+
                ((double)col.R / 255) * 1.35
                +
                ((double)col.G / 255) * 2.65
                +
                ((double)col.B / 255)
                ) / 5.0;
        }

        public static Color BackColorToTextColor(Color col)
        {
            return ColorToValue(col) < 0.53 ? Colors.White : Colors.Black;
        }

        public static double ColorToStrength(Color col)
        {
            return ((((int)col.A) << 24) + (((int)col.R) << 16) + (((int)col.G) << 8) + ((int)col.B));
        }

        public static double ColorToHue(Color col)
        {
            double max = 0;
            double min = 255;
            if (max < col.R)
                max = col.R;
            if (max < col.G)
                max = col.G;
            if (max < col.B)
                max = col.B;
            if (min > col.R)
                min = col.R;
            if (min > col.G)
                min = col.G;
            if (min > col.B)
                min = col.B;
            double ret = 0;
            if (col.B <= col.G && col.B <= col.R)
            {
                ret = 60.0 * ((col.G - col.R) / (max - min)) + 60;
            }
            if (col.R <= col.G && col.R <= col.B)
            {
                ret = 60.0 * ((col.B - col.G) / (max - min)) + 180;
            }
            if (col.G <= col.R && col.G <= col.B)
            {
                ret = 60.0 * ((col.R - col.B) / (max - min)) + 300;
            }
            return ret;
        }

        public static double ColorToSaturation(Color col)
        {
            double max = 0;
            double min = 255;
            if (max < col.R)
                max = col.R;
            if (max < col.G)
                max = col.G;
            if (max < col.B)
                max = col.B;
            if (min > col.R)
                min = col.R;
            if (min > col.G)
                min = col.G;
            if (min > col.B)
                min = col.B;
            return (max - min) / max;
        }

        public static double ColorToLightness(Color col)
        {
            double max = 0;
            double min = 255;
            if (max < col.R)
                max = col.R;
            if (max < col.G)
                max = col.G;
            if (max < col.B)
                max = col.B;
            if (min > col.R)
                min = col.R;
            if (min > col.G)
                min = col.G;
            if (min > col.B)
                min = col.B;
            return (double)(max + min) / 255 / 2;
        }

        public static double ColorToBrightness(Color col)
        {
            double max = 0;
            if (max < col.R)
                max = col.R;
            if (max < col.G)
                max = col.G;
            if (max < col.B)
                max = col.B;
            return max / 255;
        }

        public class ColorPlus : BindableBase
        {
            public ColorPlus()
            {
                Color = Colors.White;
                TextColor = Colors.Black;
                Name = "White";
                Hue = ColorToHue(Colors.White);
                Saturation = ColorToSaturation(Colors.White);
                Brightness = ColorToBrightness(Colors.White);
                Lightness = ColorToLightness(Colors.White);
            }
            private Color color;
            public Color Color
            {
                get { return color; }
                set { SetProperty(ref color, value); }
            }
            private Color textColor;
            public Color TextColor
            {
                get { return textColor; }
                set { SetProperty(ref textColor, value); }
            }
            public override bool Equals(object obj)
            {
                ColorPlus v = obj as ColorPlus;
                return v != null && Color.Equals(v.Color);
            }
            public override int GetHashCode()
            {
                return Color.GetHashCode();
            }
            private string name;
            public string Name
            {
                get { return name; }
                set { SetProperty(ref name, value); }
            }
            private string textColorName;
            public string TextColorName
            {
                get { return textColorName; }
                set { SetProperty(ref textColorName, value); }
            }
            private double hue;
            public double Hue
            {
                get { return hue; }
                set { SetProperty(ref hue, value); }
            }
            private double saturation;
            public double Saturation
            {
                get { return saturation; }
                set { SetProperty(ref saturation, value); }
            }
            private double brightness;
            public double Brightness
            {
                get { return brightness; }
                set { SetProperty(ref brightness, value); }
            }
            private double lightness;
            public double Lightness
            {
                get { return lightness; }
                set { SetProperty(ref lightness, value); }
            }
            public static implicit operator ColorPlus(Color value)
            {
                var colList = typeof(Colors).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Where(x => value == (Color)x.GetValue(null, null)).FirstOrDefault();
                return new ColorPlus {
                    Color = value,
                    Name = colList?.Name ?? "NoName",
                    TextColor = BackColorToTextColor(value),
                    TextColorName = BackColorToTextColor(value).ToString(),
                    Hue = ColorToHue(value),
                    Saturation = ColorToSaturation(value),
                    Brightness = ColorToBrightness(value),
                    Lightness = ColorToLightness(value)
                };
            }
            public static implicit operator Color(ColorPlus value)
            {
                return value.Color;
            }
            public static implicit operator string(ColorPlus value)
            {
                return value.Name;
            }
        }
    }

    //public class ColorPlusToColorTypeConverter : TypeConverter
    //{
    //    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    //    {
    //        return destinationType == typeof(ColorSelector.ColorPlus);
    //    }

    //    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    //    {
    //        var val = (ColorSelector.ColorPlus)value;
    //        return (Color)val;
    //    }

    //    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    //    {
    //        return sourceType == typeof(Color);
    //    }

    //    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    //    {
    //        var val = (Color)value;
    //        ColorSelector.ColorPlus ret = val;
    //        return ret;
    //    }
    //}

    [ValueConversion(typeof(ColorSelector.ColorPlus), typeof(string))]
    public class ColorPlusToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = (string)(ColorSelector.ColorPlus)value;
            return val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(ColorSelector.ColorPlus), typeof(Color))]
    public class ColorPlusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = (Color)value;
            return val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(ColorSelector.ColorPlus), typeof(Brush))]
    public class ColorPlusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = new SolidColorBrush((Color)(ColorSelector.ColorPlus)value);
            return val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(ColorSelector.ColorPlus), typeof(Brush))]
    public class ColorPlusToTextBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = new SolidColorBrush(((ColorSelector.ColorPlus)value).TextColor);
            return val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
