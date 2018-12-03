using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// 转换助手
    /// </summary>

    public sealed class ConvertHelper
    {
        /// <summary>
        /// 转换为字符串，防止空值
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToString(object input)
        {
            return ToString(input, string.Empty);
        }
        /// <summary>
        /// 转换为字符串，防止空值
        /// </summary>
        /// <param name="input"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string ToString(object input, string defaultValue)
        {
            if (input != null)
            {
                return input.ToString();
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 转换为32位整型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int ToInt32(string input)
        {
            return ToInt32(input, 0);
        }
        /// <summary>
        /// 转换为32位整型
        /// </summary>
        /// <param name="input"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt32(string input, int defaultValue)
        {
            int result;
            if (!int.TryParse(input, out result))
            {
                result = defaultValue;
            }
            return result;
        }

        /// <summary>
        /// 转换为64位整型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static long ToInt64(string input)
        {
            return ToInt64(input, 0);
        }
        /// <summary>
        /// 转换为64位整型
        /// </summary>
        /// <param name="input"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long ToInt64(string input, long defaultValue)
        {
            long result;
            if (!long.TryParse(input, out result))
            {
                result = defaultValue;
            }
            return result;
        }

        /// <summary>
        /// 转换为16位整型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static short ToInt16(string input)
        {
            return ToInt16(input, 0);
        }
        /// <summary>
        /// 转换为16位整型
        /// </summary>
        /// <param name="input"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static short ToInt16(string input, short defaultValue)
        {
            short result;
            if (!short.TryParse(input, out result))
            {
                result = defaultValue;
            }
            return result;
        }

        /// <summary>
        /// 转换为8位整型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte ToByte(string input)
        {
            return ToByte(input, 0);
        }
        /// <summary>
        /// 转换为8位整型
        /// </summary>
        /// <param name="input"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static byte ToByte(string input, byte defaultValue)
        {
            byte result;
            if (!byte.TryParse(input, out result))
            {
                result = defaultValue;
            }
            return result;
        }

        /// <summary>
        /// 转换为十进制数字
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static decimal ToDecimal(string input)
        {
            return ToDecimal(input, 0);
        }
        /// <summary>
        /// 转换为十进制数字
        /// </summary>
        /// <param name="input"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal ToDecimal(string input, decimal defaultValue)
        {
            decimal result;
            if (!decimal.TryParse(input, out result))
            {
                result = defaultValue;
            }
            return result;
        }

        /// <summary>
        /// 转换为双精度浮点数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static double ToDouble(string input)
        {
            return ToDouble(input, 0);
        }
        /// <summary>
        /// 转换为双精度浮点数
        /// </summary>
        /// <param name="input"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double ToDouble(string input, double defaultValue)
        {
            double result;
            if (!double.TryParse(input, out result))
            {
                result = defaultValue;
            }
            return result;
        }

        /// <summary>
        /// 转换为单精度浮点数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static float ToFloat(string input)
        {
            return ToFloat(input, 0);
        }
        /// <summary>
        /// 转换为单精度浮点数
        /// </summary>
        /// <param name="input"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static float ToFloat(string input, float defaultValue)
        {
            float result;
            if (!float.TryParse(input, out result))
            {
                result = defaultValue;
            }
            return result;
        }

        /// <summary>
        /// 转换为时间
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(string input)
        {
            return ToDateTime(input, DateTime.Now);
        }
        /// <summary>
        /// 转换为时间
        /// </summary>
        /// <param name="input"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(string input, DateTime defaultValue)
        {
            DateTime result;
            if (!DateTime.TryParse(input, out result))
            {
                result = defaultValue;
            }
            return result;
        }


        /// <summary>
        /// 将可为空的值转为字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nullable"></param>
        /// <returns></returns>
        public static string NullableToString<T>(Nullable<T> nullable) where T : struct
        {
            if (nullable.HasValue)
            {
                return nullable.Value.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 转换成常规的日期字符串，对可为空的值作了防空值处理
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ShowDate(DateTime? dt)
        {
            if (dt != null)
            {
                return ShowDate(dt.Value);
            }
            else
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 转换成常规的日期字符串
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ShowDate(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }
        /// <summary>
        /// 转换成常规的时间字符串，对可为空的值作了防空值处理
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ShowDateTime(DateTime? dt)
        {
            if (dt != null)
            {
                return ShowDateTime(dt.Value);
            }
            else
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 转换成常规的时间字符串
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ShowDateTime(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }


        /// <summary>
        /// 转到JS用的string
        /// </summary>
        /// <param name="text">文本</param>
        /// <returns></returns>
        public static string ToJavaScriptString(string text)
        {
            StringBuilder buffer = new StringBuilder(text);
            buffer.Replace("\\", @"\\");
            buffer.Replace("\t", @"\t");
            buffer.Replace("\n", @"\n");
            buffer.Replace("\r", @"\r");
            buffer.Replace("\"", @"\""");
            buffer.Replace("\'", @"\'");
            buffer.Replace("/", @"\/");
            return buffer.ToString();
        }

        /// <summary>
        /// Encodes a string to be represented as a string literal. The format
        /// is essentially a JSON string.
        /// 
        /// The string returned includes outer quotes 
        /// Example Output: "Hello \"Rick\"!\r\nRock on"
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string EncodeJavaScriptString(string s)
        {
            if (s == null)
            {
                s = string.Empty;
            }
            StringBuilder sb = new StringBuilder();

            foreach (char c in s)
            {
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
                        int i = (int)c;
                        if (i < 32 || i > 127)
                        {
                            sb.AppendFormat("\\u{0:X04}", i);
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }

            return sb.ToString();
        }


    }
}
