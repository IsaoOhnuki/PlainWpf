using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Globalization;
using System.Windows.Data;

namespace Extensions
{
    /// <summary>
    /// 日付時間のフォーマット出力
    /// </summary>
    /// <see cref="ToSumpleString">サンプル出力</see>
    public static class JapaneseDateTimeExtensions
    {
        private static CultureInfo jpCulture;
        /// <summary>
        /// Gets the jp culture.
        /// </summary>
        /// <value>
        /// The jp culture.
        /// </value>
        public static CultureInfo JpCulture
        {
            get
            {
                if (jpCulture == null)
                {
                    jpCulture = new CultureInfo("ja-JP", false);
                    jpCulture.DateTimeFormat.Calendar = new JapaneseCalendar();
                }
                return jpCulture;
            }
        }
        /// <summary>
        /// Jps the year string.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>元号XX年</returns>
        public static string ToJpYearString(this DateTime dateTime)
        {
            return dateTime.ToString("ggyy", JpCulture) + "年";
        }
        /// <summary>
        /// To the jp short year string.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>西暦XXXX年</returns>
        public static string ToUsYearString(this DateTime dateTime)
        {
            return dateTime.ToString("ggyyyy") + "年";
        }
        /// <summary>
        /// Jps the week string.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>Ⅹ曜日</returns>
        public static string ToJpWeekString(this DateTime dateTime)
        {
            return dateTime.ToString("dddd", JpCulture);
        }
        /// <summary>
        /// Jps the short week string.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>Ⅹ(曜日)</returns>
        public static string ToJpShortWeekString(this DateTime dateTime)
        {
            return dateTime.ToString("ddd", JpCulture);
        }
        /// <summary>
        /// To the jp month string.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>XX月</returns>
        public static string ToJpMonthString(this DateTime dateTime)
        {
            return dateTime.ToString("MM") + "月";
        }
        /// <summary>
        /// To the jp date string.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>XX日</returns>
        public static string ToJpDateString(this DateTime dateTime)
        {
            return dateTime.ToString("dd") + "日";
        }
        /// <summary>
        /// To the jp hour string.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>XX時(24時間制)</returns>
        public static string ToJpHourString(this DateTime dateTime)
        {
            return dateTime.ToString("HH") + "時";
        }
        /// <summary>
        /// To the jp short hour string.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>午前・午後XX時</returns>
        public static string ToJpShortHourString(this DateTime dateTime)
        {
            return dateTime.ToString("tthh", JpCulture) + "時";
        }
        /// <summary>
        /// To the jp minute string.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>XX分</returns>
        public static string ToJpMinuteString(this DateTime dateTime)
        {
            return dateTime.ToString("mm") + "分";
        }
        /// <summary>
        /// To the jp second string.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>XX秒</returns>
        public static string ToJpSecondString(this DateTime dateTime)
        {
            return dateTime.ToString("ss") + "秒";
        }
        /// <summary>
        /// To the format string.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static string ToFormatString(this DateTime dateTime, string format)
        {
            var now = dateTime;
            return format?
                .Replace("{JY}", now.ToJpYearString())
                .Replace("{UY}", now.ToUsYearString())
                .Replace("{JM}", now.ToJpMonthString())
                .Replace("{JD}", now.ToJpDateString())
                .Replace("{JW}", now.ToJpWeekString())
                .Replace("{jw}", now.ToJpShortWeekString())
                .Replace("{JH}", now.ToJpHourString())
                .Replace("{jh}", now.ToJpShortHourString())
                .Replace("{jm}", now.ToJpMinuteString())
                .Replace("{JS}", now.ToJpSecondString())
                .Replace("{YY}", now.ToString("yyyy"))
                .Replace("{yy}", now.ToString("yy"))
                .Replace("{MM}", now.ToString("MM"))
                .Replace("{dd}", now.ToString("dd"))
                .Replace("{hh}", now.ToString("HH"))
                .Replace("{mm}", now.ToString("mm"))
                .Replace("{ss}", now.ToString("ss"))
                .Replace("{FF}", now.ToString("fff"))
                .Replace("{ff}", now.ToString("ff"));
        }
        /// <summary>
        /// To the sumple string.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>サンプル出力</returns>
        public static string ToJpSumpleString(this DateTime dateTime)
        {
            string format = "{xx} <- format string replace.\r\n" +
                "xx : JY ({JY})" + "\r\n" +       // JY(平成30年)
                "xx : UY ({UY})" + "\r\n" +       // UY(西暦2018年)
                "xx : JM ({JM})" + "\r\n" +       // JM(02月)
                "xx : JD ({JD})" + "\r\n" +       // JD(23日)
                "xx : JW ({JW})" + "\r\n" +       // JW(金曜日)
                "xx : jw ({jw})" + "\r\n" +       // jw(金)
                "xx : JH ({JH})[24H]" + "\r\n" +  // JH(11時)[24H]
                "xx : jh ({jh})" + "\r\n" +       // jh(午前11時)
                "xx : jm ({jm})" + "\r\n" +       // jm(02分)
                "xx : JS ({JS})" + "\r\n" +       // JS(09秒)
                "xx : YY ({YY})" + "\r\n" +       // YY(2018)
                "xx : yy ({yy})" + "\r\n" +       // yy(18)
                "xx : MM ({MM})" + "\r\n" +       // MM(02)
                "xx : dd ({dd})" + "\r\n" +       // dd(23)
                "xx : hh ({hh})[24H]" + "\r\n" +  // hh(11)[24H]
                "xx : mm ({mm})" + "\r\n" +       // mm(21)
                "xx : ss ({ss})" + "\r\n" +       // ss(09)
                "xx : FF ({FF})" + "\r\n" +       // FF(495)
                "xx : ff ({ff})";                 // ff(49)
            return ToFormatString(dateTime, format);
        }
    }
}
