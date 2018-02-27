using Utilitys;
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
    /// <remarks><a href="http://www.losttechnology.jp/WebDesign/colorlist.html">さまざまな色一覧</a></remarks>
    public partial class ColorSelector : UserControl, INotifyPropertyChanged
    {
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
                    TextColor = ColorPlus.BackColorToTextColor(x.Color),
                    Name = x.Name,
                    TextColorName = ColorPlus.BackColorToTextColor(x.Color).ToString(),
                    Hue = ColorPlus.ColorToHue(x.Color),
                    Lightness = ColorPlus.ColorToLightness(x.Color) == 0 ? 0 : ColorPlus.ColorToLightness(x.Color) < 0.25 ? 0.25 : ColorPlus.ColorToLightness(x.Color) < 0.5 ? 0.49 : ColorPlus.ColorToLightness(x.Color) == 0.5 ? 0.5 : ColorPlus.ColorToLightness(x.Color) < 0.75 ? 0.75 : 1,
                    Brightness = ColorPlus.ColorToBrightness(x.Color) == 0 ? 0 : ColorPlus.ColorToBrightness(x.Color) < 0.2 ? 0.2 : ColorPlus.ColorToBrightness(x.Color) < 0.4 ? 0.4 : ColorPlus.ColorToBrightness(x.Color) < 0.6 ? 0.6 : ColorPlus.ColorToBrightness(x.Color) < 0.8 ? 0.8 : 1,
                    Saturation = ColorPlus.ColorToSaturation(x.Color) == 0 ? 0 : ColorPlus.ColorToSaturation(x.Color) < 0.2 ? 0.2 : ColorPlus.ColorToSaturation(x.Color) < 0.4 ? 0.4 : ColorPlus.ColorToSaturation(x.Color) < 0.6 ? 0.6 : ColorPlus.ColorToSaturation(x.Color) < 0.8 ? 0.8 : 1,
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
    }
}
