using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Utilitys
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks><a href="http://www.losttechnology.jp/WebDesign/colorlist.html">さまざまな色一覧</a></remarks>
    public class ColorPlus : NotifyBase
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
            return new ColorPlus
            {
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
        public static implicit operator double(ColorPlus value)
        {
            return ((((int)value.Color.A) << 24) + (((int)value.Color.R) << 16) + (((int)value.Color.G) << 8) + ((int)value.Color.B));
        }

        public static double ColorStrength(Color col)
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
            return ColorStrength(col) < 0.53 ? Colors.White : Colors.Black;
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
    }
}
