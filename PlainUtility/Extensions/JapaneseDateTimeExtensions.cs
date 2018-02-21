using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class JapaneseDateTimeExtensions
    {
        private class JpYear
        {
            public DateTime startDay;
            public DateTime endTommorow;
            public int offsetOfYear;
            public string longJpYear;
            public string middleJpYear;
            public string shortJpYear;
        }
        private static string[] jpShortWeek = new string[] { "日", "月", "火", "水", "木", "金", "土" };
        private static string[] jpLongWeek = new string[] { "日曜日", "月曜日", "火曜日", "水曜日", "木曜日", "金曜日", "土曜日" };
        private static List<JpYear> jpYears = new List<JpYear>(new []{
            new JpYear { startDay = new DateTime(), endTommorow = new DateTime(), offsetOfYear = 0, longJpYear = "明治", middleJpYear = "明", shortJpYear = "M" },
            new JpYear { startDay = new DateTime(), endTommorow = new DateTime(), offsetOfYear = 0, longJpYear = "大正", middleJpYear = "大", shortJpYear = "T" },
            new JpYear { startDay = new DateTime(), endTommorow = new DateTime(), offsetOfYear = 0, longJpYear = "昭和", middleJpYear = "昭", shortJpYear = "S" },
            new JpYear { startDay = new DateTime(), endTommorow = new DateTime(9999, 1, 1), offsetOfYear = 0, longJpYear = "平成", middleJpYear = "平", shortJpYear = "H" }
        });
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startDay"></param>
        /// <param name="offsetOfYear"></param>
        /// <param name="longJpYear"></param>
        /// <param name="middleJpYear"></param>
        /// <param name="shortJpYear"></param>
        public static void NewJpYear(DateTime startDay, int offsetOfYear, string longJpYear, string middleJpYear, string shortJpYear)
        {
            jpYears.Last().endTommorow = startDay;
            jpYears.Add(new JpYear { startDay = startDay, endTommorow = new DateTime(9999, 1, 1), offsetOfYear = offsetOfYear, longJpYear = longJpYear, middleJpYear = middleJpYear, shortJpYear = shortJpYear });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns>日曜日 ～ 土曜日</returns>
        public static string ToLongJpWeekDay(DateTime dateTime)
        {
            return jpLongWeek[(int)dateTime.DayOfWeek];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns>日 ～ 土</returns>
        public static string ToShortJpWeekDay(DateTime dateTime)
        {
            return jpShortWeek[(int)dateTime.DayOfWeek];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToLongJpYearString(this DateTime dateTime)
        {
            return jpYears.Where(x => dateTime >= x.startDay && dateTime < x.endTommorow).Select(x => x.longJpYear.ToString() + (dateTime.Year - x.offsetOfYear) + "年").Single() + dateTime.ToString("M月d日");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToMiddleJpYearString(this DateTime dateTime)
        {
            return jpYears.Where(x => dateTime >= x.startDay && dateTime < x.endTommorow).Select(x => x.middleJpYear.ToString() + (dateTime.Year - x.offsetOfYear) + "年").Single() + dateTime.ToString("M月d日");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="separater"></param>
        /// <returns></returns>
        public static string ToShortJpYearString(this DateTime dateTime, string separater = "・")
        {
            return jpYears.Where(x => dateTime >= x.startDay && dateTime < x.endTommorow).Select(x => x.shortJpYear.ToString() + (dateTime.Year - x.offsetOfYear)).Single() + separater + dateTime.ToString("M") + separater + dateTime.ToString("d");
        }
    }
}
