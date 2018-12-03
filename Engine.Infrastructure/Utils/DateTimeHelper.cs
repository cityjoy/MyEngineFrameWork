using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// 图片操作助手
    /// </summary>

    public sealed class DateTimeHelper
    {

        /// <summary>
        /// 日期转换成unix时间戳(秒)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ConvertToUnixTimestamp(DateTime dateTime)
        {
            DateTime start = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long timestamp = Convert.ToInt64((dateTime - start).TotalSeconds);
            return timestamp;
        }

        /// <summary>
        /// unix时间戳转换成日期
        /// </summary>
        /// <param name="timestamp">时间戳（秒）</param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(long timestamp)
        {
            DateTime start = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime dt = start.AddSeconds(timestamp);
            return dt;
        }

        /// <summary>
        /// 日期转换成unix时间戳(毫秒)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ConvertToUnixTimestampMS(DateTime dateTime)
        {
            DateTime start = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long timestamp = Convert.ToInt64((dateTime - start).TotalMilliseconds);
            return timestamp;
        }

        /// <summary>
        /// unix时间戳转换成日期
        /// </summary>
        /// <param name="timestamp">时间戳（毫秒）</param>
        /// <returns></returns>
        public static DateTime ConvertToDateTimeMS(long timestamp)
        {
            DateTime start = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime dt = start.AddMilliseconds(timestamp);
            return dt;
        }

        /// <summary>
        /// 转换为ISO8601规范的UTC格式，标准格式应当是“YYYY-MM-DDTHH:MM:SS+时区”
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string DateTimeToISO8601String(DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-ddTHH:mm:sszzzz", System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }

    }
}
