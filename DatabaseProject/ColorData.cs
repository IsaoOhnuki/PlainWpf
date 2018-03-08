using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseUtility
{
    public class ColorData
    {
        public ColorData()
        {
            unchecked
            {
                color = (Int32)0xFFFFFFFF;
            }
        }
        [DefaultValue(DefaultValue = "0xFFFFFFFF")]
        public Int32 color { get; set; }
        public static implicit operator ColorData(System.Windows.Media.Color value)
        {
            return new ColorData { color = (((Int32)value.A) << 24) + (((Int32)value.R) << 16) + (((Int32)value.G) << 8) + ((Int32)value.B) };
        }
        public static implicit operator System.Windows.Media.Color(ColorData value)
        {
            if (value == null)
                value = System.Windows.Media.Colors.White;
            return System.Windows.Media.Color.FromArgb((byte)((value.color >> 24) & 0xff), (byte)((value.color >> 16) & 0xff), (byte)((value.color >> 8) & 0xff), (byte)(value.color & 0xff));
        }
    }
}
