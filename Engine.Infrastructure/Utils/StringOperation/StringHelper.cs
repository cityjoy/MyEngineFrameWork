using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// 字符操作助手
    /// </summary>

    public class StringHelper
    {
        #region Comparison

        /// <summary>
        /// 忽略大小写的字符串比较
        /// </summary>
        /// <param name="s1">字符串1</param>
        /// <param name="s2">字符串2</param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2))
            {
                return true;
            }
            if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
            {
                return false;
            }

            if (s2.Length != s1.Length)
            {
                return false;
            }

            return (0 == string.Compare(s1, 0, s2, 0, s2.Length, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 是否以指定字符开头
        /// </summary>
        /// <param name="text">搜索的文本</param>
        /// <param name="lookfor">指定字符</param>
        /// <returns></returns>
        public static bool StartsWith(string text, char lookfor)
        {
            return (text.Length > 0 && text[0] == lookfor);
        }

        /// <summary>
        /// 是否以指定字符串开头
        /// </summary>
        /// <param name="target">搜索的文本</param>
        /// <param name="lookfor">指定字符串</param>
        /// <returns></returns>
        public static bool StartsWith(string target, string lookfor)
        {
            if (string.IsNullOrEmpty(target) || string.IsNullOrEmpty(lookfor))
            {
                return false;
            }

            if (lookfor.Length > target.Length)
            {
                return false;
            }

            return (0 == string.Compare(target, 0, lookfor, 0, lookfor.Length, StringComparison.Ordinal));
        }

        /// <summary>
        /// 是否以指定字符串开头(忽略大小写)
        /// </summary>
        /// <param name="text">搜索的文本</param>
        /// <param name="lookfor">指定字符串</param>
        /// <returns></returns>
        public static bool StartsWithIgnoreCase(string text, string lookfor)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(lookfor))
            {
                return false;
            }

            if (lookfor.Length > text.Length)
            {
                return false;
            }
            return (0 == string.Compare(text, 0, lookfor, 0, lookfor.Length, StringComparison.OrdinalIgnoreCase));
        }


        /// <summary>
        /// 是否以指定字符结尾
        /// </summary>
        /// <param name="text">搜索的文本</param>
        /// <param name="lookfor">指定字符</param>
        /// <returns></returns>
        public static bool EndsWith(string text, char lookfor)
        {
            return (text.Length > 0 && text[text.Length - 1] == lookfor);
        }

        /// <summary>
        /// 是否以指定字符串结尾
        /// </summary>
        /// <param name="text">搜索的文本</param>
        /// <param name="lookfor">指定字符串</param>
        /// <returns></returns>
        public static bool EndsWith(string text, string lookfor)
        {
            int indexA = text.Length - lookfor.Length;

            if (indexA < 0)
            {
                return false;
            }

            return (0 == string.Compare(text, indexA, lookfor, 0, lookfor.Length, StringComparison.Ordinal));
        }

        /// <summary>
        /// 是否以指定字符串结尾(忽略大小写)
        /// </summary>
        /// <param name="text">搜索的文本</param>
        /// <param name="lookfor">指定字符串</param>
        /// <returns></returns>
        public static bool EndsWithIgnoreCase(string text, string lookfor)
        {
            int indexA = text.Length - lookfor.Length;

            if (indexA < 0)
            {
                return false;
            }

            return (0 == string.Compare(text, indexA, lookfor, 0, lookfor.Length, StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        #region Parse

        /// <summary>
        /// 尝试解析字符串,支持可为空的值类型,枚举类型,Guid
        /// </summary>
        /// <param name="input">字符串</param>
        /// <param name="type">所要解析成的类型</param>
        /// <param name="result">解析结果，解析失败将返回null</param>
        /// <returns>是否解析成功</returns>
        public static bool TryParse(string input, Type type, out object result)
        {
            result = null;

            if (input == null)
            {
                return false;
            }

            bool succeed = false;
            object parsedValue = null;

            bool isNullable = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

            if (isNullable)
            {
                type = type.GetGenericArguments()[0];
            }

            if (type.IsEnum)
            {
                #region 解析枚举

                foreach (string name in Enum.GetNames(type))
                {
                    if (EqualsIgnoreCase(name, input))
                    {
                        parsedValue = Enum.Parse(type, input, true);
                        succeed = true;
                    }
                }

                if (parsedValue == null)
                {
                    return false;
                }

                #endregion
            }
            else if (type == typeof(Guid))
            {
                #region 解析Guid

                try
                {
                    parsedValue = new Guid(input);
                    succeed = true;
                }
                catch
                {
                    return false;
                }

                #endregion
            }
            else
            {
                #region 解析基础类型

                TypeCode typeCode = Type.GetTypeCode(type);

                switch (typeCode)
                {
                    case TypeCode.String:
                        parsedValue = input;
                        succeed = true;
                        break;
                    case TypeCode.Boolean:
                        {
                            Boolean temp;
                            succeed = Boolean.TryParse(input, out temp);

                            if (succeed)
                                parsedValue = temp;
                            else
                                return false;
                        }
                        break;
                    case TypeCode.Byte:
                        {
                            Byte temp;
                            succeed = Byte.TryParse(input, out temp);

                            if (succeed)
                                parsedValue = temp;
                            else
                                return false;
                        }
                        break;
                    case TypeCode.Decimal:
                        {
                            Decimal temp;
                            succeed = Decimal.TryParse(input, out temp);

                            if (succeed)
                                parsedValue = temp;
                            else
                                return false;
                        }
                        break;
                    case TypeCode.Double:
                        {
                            Double temp;
                            succeed = Double.TryParse(input, out temp);

                            if (succeed)
                                parsedValue = temp;
                            else
                                return false;
                        }
                        break;
                    case TypeCode.Int16:
                        {
                            Int16 temp;
                            succeed = Int16.TryParse(input, out temp);

                            if (succeed)
                                parsedValue = temp;
                            else
                                return false;
                        }
                        break;
                    case TypeCode.Int32:
                        {
                            Int32 temp;
                            succeed = Int32.TryParse(input, out temp);

                            if (succeed)
                                parsedValue = temp;
                            else
                                return false;
                        }
                        break;
                    case TypeCode.Int64:
                        {
                            Int64 temp;
                            succeed = Int64.TryParse(input, out temp);

                            if (succeed)
                                parsedValue = temp;
                            else
                                return false;
                        }
                        break;
                    case TypeCode.SByte:
                        {
                            SByte temp;
                            succeed = SByte.TryParse(input, out temp);

                            if (succeed)
                                parsedValue = temp;
                            else
                                return false;
                        }
                        break;
                    case TypeCode.Single:
                        {
                            Single temp;
                            succeed = Single.TryParse(input, out temp);

                            if (succeed)
                                parsedValue = temp;
                            else
                                return false;
                        }
                        break;
                    case TypeCode.UInt16:
                        {
                            UInt16 temp;
                            succeed = UInt16.TryParse(input, out temp);

                            if (succeed)
                                parsedValue = temp;
                            else
                                return false;
                        }
                        break;
                    case TypeCode.UInt32:
                        {
                            UInt32 temp;
                            succeed = UInt32.TryParse(input, out temp);

                            if (succeed)
                                parsedValue = temp;
                            else
                                return false;
                        }
                        break;
                    case TypeCode.UInt64:
                        {
                            UInt64 temp;
                            succeed = UInt64.TryParse(input, out temp);

                            if (succeed)
                                parsedValue = temp;
                            else
                                return false;
                        }
                        break;
                    case TypeCode.DateTime:
                        {
                            DateTime temp;
                            succeed = DateTime.TryParse(input, out temp);

                            if (succeed)
                                parsedValue = temp;
                            else
                                return false;
                        }
                        break;
                    default:
                        return false;
                }

                #endregion
            }

            result = parsedValue;

            return succeed;
        }

        /// <summary>
        /// 尝试解析字符串,如果解析失败,使用传递进来的默认值,支持可为空的值类型,枚举类型,Guid
        /// </summary>
        /// <param name="input">字符串</param>
        /// <param name="type">所要解析成的类型</param>
        /// <param name="defaultValue">如果解析失败要使用的默认值</param>
        /// <returns>解析成的对象结果</returns>
        public static object TryParse(string input, Type type, object defaultValue)
        {
            object result;
            bool success = TryParse(input, type, out result);
            if (success)
            {
                return result;
            }
            return defaultValue;
        }

        /// <summary>
        /// 尝试解析字符串,如果解析失败,使用传递进来的默认值,支持可为空的值类型,枚举类型,Guid
        /// </summary>
        /// <typeparam name="T">支持可为空的值类型,枚举类型,Guid</typeparam>
        /// <param name="input">字符串</param>
        /// <param name="defaultValue">如果解析失败要使用的默认值</param>
        /// <returns></returns>
        public static T TryParse<T>(string input, T defaultValue)
        {
            Type type = typeof(T);
            object result;
            bool success = TryParse(input, type, out result);
            if (success)
            {
                return (T)result;
            }
            return defaultValue;
        }

        /// <summary>
        /// 尝试解析字符串,如果解析失败,使用类型的默认值,支持可为空的值类型,枚举类型,Guid
        /// </summary>
        /// <typeparam name="T">支持可为空的值类型,枚举类型,Guid</typeparam>
        /// <param name="input">字符串</param>
        /// <returns></returns>
        public static T TryParse<T>(string input)
        {
            Type type = typeof(T);
            object result;
            bool success = TryParse(input, type, out result);
            if (success)
            {
                return (T)result;
            }
            return default(T);
        }

        #endregion

        #region Split

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T[] Split<T>(string input)
        {
            return Split<T>(input, StringSplitOptions.RemoveEmptyEntries, ',');
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="seperators"></param>
        /// <returns></returns>
        public static T[] Split<T>(string input, params char[] seperators)
        {
            return Split<T>(input, StringSplitOptions.RemoveEmptyEntries, seperators);
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="stringSplitOptions"></param>
        /// <param name="seperators"></param>
        /// <returns></returns>
        public static T[] Split<T>(string input, StringSplitOptions stringSplitOptions, params char[] seperators)
        {
            if (string.IsNullOrEmpty(input))
                return new T[] { };

            string[] strs = input.Split(seperators, stringSplitOptions);
            int strsLen = strs.Length;

            T[] result = new T[strsLen];

            for (int i = 0; i < strsLen; i++)
            {
                T element = TryParse<T>(strs[i].Trim());
                result[i] = element;
            }

            return result;
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="seperators"></param>
        /// <returns></returns>
        public static T[] Split<T>(string input, params string[] seperators)
        {
            return Split<T>(input, StringSplitOptions.RemoveEmptyEntries, seperators);
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="stringSplitOptions"></param>
        /// <param name="seperators"></param>
        /// <returns></returns>
        public static T[] Split<T>(string input, StringSplitOptions stringSplitOptions, params string[] seperators)
        {
            if (string.IsNullOrEmpty(input))
                return new T[] { };

            string[] strs = input.Split(seperators, stringSplitOptions);
            int strsLen = strs.Length;

            T[] result = new T[strsLen];

            for (int i = 0; i < strsLen; i++)
            {
                T element = TryParse<T>(strs[i].Trim());
                result[i] = element;
            }

            return result;
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<T> Explode<T>(string input)
        {
            return Explode<T>(input, StringSplitOptions.RemoveEmptyEntries, ',');
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="seperators"></param>
        /// <returns></returns>
        public static List<T> Explode<T>(string input, params char[] seperators)
        {
            return Explode<T>(input, StringSplitOptions.RemoveEmptyEntries, seperators);
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="stringSplitOptions"></param>
        /// <param name="seperators"></param>
        /// <returns></returns>
        public static List<T> Explode<T>(string input, StringSplitOptions stringSplitOptions, params char[] seperators)
        {
            if (string.IsNullOrEmpty(input))
                return new List<T>();

            string[] strs = input.Split(seperators, stringSplitOptions);
            int strsLen = strs.Length;

            List<T> result = new List<T>(strsLen);

            foreach (string str in strs)
            {
                T element = TryParse<T>(str.Trim());
                result.Add(element);
            }

            return result;
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="seperators"></param>
        /// <returns></returns>
        public static List<T> Explode<T>(string input, params string[] seperators)
        {
            return Explode<T>(input, StringSplitOptions.RemoveEmptyEntries, seperators);
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="stringSplitOptions"></param>
        /// <param name="seperators"></param>
        /// <returns></returns>
        public static List<T> Explode<T>(string input, StringSplitOptions stringSplitOptions, params string[] seperators)
        {
            if (string.IsNullOrEmpty(input))
                return new List<T>();

            string[] strs = input.Split(seperators, stringSplitOptions);
            int strsLen = strs.Length;

            List<T> result = new List<T>(strsLen);

            foreach (string str in strs)
            {
                string tempStr = str.Trim();

                T element = TryParse<T>(tempStr);
                result.Add(element);
            }

            return result;
        }

        #endregion

        #region Join

        /// <summary>
        /// 将指定列表所有项以指定的分隔符连接成字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string Join<T>(IEnumerable<T> list)
        {
            return Join<T>(list, ",", false);
        }
         /// <summary>
        /// 将指定列表所有项以指定的分隔符连接成字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="joinSign"></param>
        /// <returns></returns>
        public static string Join<T>(IEnumerable<T> list, string joinSign)
        {
            return Join<T>(list, joinSign, false);
        }
        /// <summary>
        /// 将指定列表所有项以指定的分隔符连接成字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="joinSign"></param>
        /// <param name="singleQuotes"></param>
        /// <returns></returns>
        public static string Join<T>(IEnumerable<T> list, string joinSign, bool singleQuotes)
        {
            if (list == null) { return string.Empty; }

            string result = string.Empty;
            bool isStr = typeof(T) == typeof(string);

            if (!string.IsNullOrEmpty(joinSign))
            {
                foreach(T s in list)
                {
                    if (singleQuotes)
                    {
                        result = string.Concat(result, joinSign, "'", s.ToString(), "'");
                    }
                    else
                    {
                        result = string.Concat(result, joinSign, s.ToString());
                    }
                }

                if (result.Length > joinSign.Length)
                {
                    result = result.Substring(joinSign.Length);
                }
            }
            else
            {
                foreach (T s in list)
                {
                    if (singleQuotes)
                    {
                        result = string.Concat(result, "'", s.ToString(), "'");
                    }
                    else
                    {
                        result = string.Concat(result, s.ToString());
                    }
                }
            }
            return result;
        }

        #endregion

        /// <summary>
        /// 检查是否纯数字
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool CheckIsNumberFormat(string input)
        {
            return Regex.IsMatch(input, @"^\d+(\.\d+)?$", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
        }

        /// <summary>
        /// 检查是否纯汉字
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool CheckIsChinese(string input)
        {
            return Regex.IsMatch(input, @"^[\u4e00-\u9fa5]+$", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
        }

        /// <summary>
        /// 检查是否手机号码格式(注：这里没有严格匹配，将10位以上数字视为手机号码)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool CheckIsPhoneFormat(string input)
        {
            return Regex.IsMatch(input, @"^\d{10,}$", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
        }

        /// <summary>
        /// 检查是否邮箱地址格式
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool CheckIsEmailFormat(string input)
        {
            return Regex.IsMatch(input, @"^[a-zA-Z0-9_]+?(\@|\#)[a-zA-Z0-9_]+(\.[a-zA-Z]+)+$", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
        }

        /// <summary>
        /// 检查是否超链接地址格式
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool CheckIsSuperLinkFormat(string input)
        {
            return Regex.IsMatch(input, @"^\b(([\w-]+://?|www[.])[^\s()<>]+(?:\([\w\d]+\)|([^[:punct:]\s]|/)))$", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
        }

        
        /// <summary>
        /// 给文本中某一段加星号隐藏保护
        /// </summary>
        /// <param name="text">要保护的文本</param>
        /// <param name="startIndex">保护开始的位置</param>
        /// <param name="length">保护的长度</param>
        /// <returns></returns>
        public static string StarredProtectText(string text, int startIndex, int length)
        {
            return StarredProtectText(text, "*", startIndex, length);
        }
        /// <summary>
        /// 给文本中某一段加星号隐藏保护
        /// </summary>
        /// <param name="text">要保护的文本</param>
        /// <param name="sign">保护符号</param>
        /// <param name="startIndex">保护开始的位置</param>
        /// <param name="length">保护的长度</param>
        /// <returns></returns>
        public static string StarredProtectText(string text, string sign, int startIndex, int length)
        {
            string result;
            string protectedText = string.Empty;
            
            int endIndex = startIndex + length;
            if (text.Length > startIndex && text.Length > endIndex)
            {
                for (int i = 0; i < length; i++)
                {
                    protectedText += sign;
                }
                result = text.Substring(0, startIndex) + protectedText + text.Substring(endIndex);
            }
            else
            {
                if (text.Length > startIndex && text.Length <= endIndex)
                {
                    for (int i = 0; i < length; i++)
                    {
                        protectedText += sign;
                    }
                    result = text.Substring(0, startIndex) + protectedText;
                }
                else if (text.Length <= startIndex)
                {
                    result = text.Substring(0, startIndex);
                }
                else
                {
                    result = text;
                }
            }
            return result;
        }

        /// <summary>
        /// 获取字符串的字节长度，默认自动尝试用GB2312编码获取，
        /// 如果当前运行环境支持GB2312编码，英文字母将被按1字节计算，中文字符将被按2字节计算
        /// 如果尝试使用GB2312编码失败，将采用当前系统的默认编码，此时得到的字节长度根据具体运行环境默认编码而定
        /// </summary>
        /// <param name="text">字符串</param>
        /// <returns>字符串的字节长度</returns>
        public static int GetByteLength(string text)
        {
            return Encoding.UTF8.GetByteCount(text);
        }
        /// <summary>
        /// 获取字符串的字节长度，默认自动尝试用GB2312编码获取，
        /// 如果当前运行环境支持GB2312编码，英文字母将被按1字节计算，中文字符将被按2字节计算
        /// 如果尝试使用GB2312编码失败，将采用当前系统的默认编码，此时得到的字节长度根据具体运行环境默认编码而定
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="text">字符串</param>
        /// <returns>字符串的字节长度</returns>
        public static int GetByteLength(Encoding encoding, string text)
        {
            return encoding.GetByteCount(text);
        }
        
        /// <summary>
        /// 按最大长度裁剪字符串
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxLen">最大长度(字数)</param>
        /// <returns></returns>
        public static string CutString(string text, int maxLen)
        {
            return CutString(text, maxLen, "...");
        }
        /// <summary>
        /// 按最大长度裁剪字符串
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxLen">最大长度(字数)</param>
        /// <param name="sign">补足符号</param>
        /// <returns></returns>
        public static string CutString(string text, int maxLen, string sign)
        {
            if (text == null)
            {
                return string.Empty;
            }
            int textLen = text.Length;
            if (textLen > maxLen)
            {
                string cuttedText = string.Concat(text.Substring(0, maxLen), sign);
                return cuttedText;
            }
            else
            {
                return text;
            }
        }

        
        /// <summary>
        /// 按最大长度裁剪字符串[加强版](两个字母相当于一个汉字)
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxLen">最大长度(字节)</param>
        /// <returns></returns>
        public static string XCutString(string text, int maxLen)
        {
            return XCutString(text, maxLen, "...");
        }
        /// <summary>
        /// 按最大长度裁剪字符串[加强版](两个字母相当于一个汉字)
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxLen">最大长度(字节)</param>
        /// <param name="sign">补足符号</param>
        /// <returns></returns>
        public static string XCutString(string text, int maxLen, string sign)
        {
            if (text == null)
            {
                return string.Empty;
            }
            int byteLen = 0;
            StringBuilder sb = new StringBuilder(string.Empty);
            bool needSign = false;
            foreach (char word in text)
            {
                if (byteLen >= maxLen)
                {
                    needSign = true;
                    break;
                }
                int wordCode = Convert.ToInt32(word);
                if (wordCode >= 65 && wordCode <= 122)
                {
                    byteLen += 1;
                }
                else
                {
                    byteLen += 2;
                }
                sb.Append(word);
            }
            if (needSign)
            {
                sb.Append(sign);
            }
            return sb.ToString();
        }


        /// <summary>
        /// 加亮字符串中指定的关键词，不区分大小写
        /// </summary>
        /// <param name="text"></param>
        /// <param name="keywords">要加亮的关键词</param>
        /// <param name="tag">使用的html标签名</param>
        /// <param name="css">使用的css样式名</param>
        /// <returns></returns>
        public static string TagKeywords(string text, string keywords, string tag, string css)
        {
            if (string.IsNullOrEmpty(keywords))
            {
                return text;
            }

            StringBuilder sbOrgKw = new StringBuilder();

            int startPos = -1;
            int index = 0;
            bool isMatched = false;
            for (int i = 0; i < text.Length; i++)
            {
                int textCode = Convert.ToInt32(text[i]);
                int kwCode = Convert.ToInt32(keywords[index]);
                if (textCode >= 65 && textCode <= 122)
                {
                    if (kwCode == textCode || (kwCode + 32) == textCode || kwCode == (textCode + 32))
                    {
                        isMatched = true;
                        sbOrgKw.Append((char)textCode);
                        index++;
                    }
                    else
                    {
                        if (isMatched == true)
                        {
                            index = 0;
                            sbOrgKw.Remove(0, sbOrgKw.Length);
                            isMatched = false;
                        }
                    }
                }
                else
                {
                    if (kwCode == textCode)
                    {
                        isMatched = true;
                        sbOrgKw.Append((char)textCode);
                        index++;
                    }
                    else
                    {
                        if (isMatched == true)
                        {
                            index = 0;
                            sbOrgKw.Remove(0, sbOrgKw.Length);
                            isMatched = false;
                        }
                    }
                }

                if (index >= keywords.Length)
                {
                    startPos = i + 1 - keywords.Length;
                    break;
                }
            }
            if (startPos < 0 || index != keywords.Length)
            {
                return text;
            }

            int endPos = startPos + keywords.Length;
            string orgKw = sbOrgKw.ToString();
            string highlightText = string.Concat("<", tag, " class=\"", css, "\">", orgKw, "</", tag, ">");
            string endPartStr = TagKeywords(text.Substring(endPos), keywords, tag, css);
            string resultText = string.Concat(text.Substring(0, startPos), highlightText, endPartStr);

            return resultText;
        }

        /// <summary>
        /// 文件大小友好格式字符串
        /// </summary>
        /// <param name="byteSize"></param>
        /// <returns></returns>
        public static string FormatSize(long byteSize)
        {
            if (byteSize <= 0)
                return "0 K";

            string unit = "BKMG";
            if (byteSize >= 1024)
            {
                double size = byteSize;
                int indx = 0;
                do
                {
                    size = size / 1024.00;
                    indx++;
                } while (size >= 1024 && indx < unit.Length - 1);
                return Math.Round(size, 1).ToString() + " " + unit[indx];
            }
            else
            {
                if (byteSize > 1)
                {
                    return byteSize.ToString() + " Bytes";
                }
                else
                {
                    return byteSize.ToString() + " Byte";
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static string BuildRangeNumberString(int min, int max)
        {
            return BuildRangeNumberString(min, max, 1, ",", null);
        }

        /// <summary>
        /// 创建连续数字字符串
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="defaultMax"></param>
        /// <returns></returns>
        public static string BuildRangeNumberString(int min, int max, int? defaultMax)
        {
            return BuildRangeNumberString(min, max, 1, ",", defaultMax);
        }
        /// <summary>
        /// 创建连续数字字符串，以指定sign符号间隔
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="step">每个数字间隔</param>
        /// <param name="sign">以什么符号间隔</param>
        /// <param name="defaultMax">如果给定的最大值小于最小值，则使用此默认值作为最大值</param>
        /// <returns></returns>
        public static string BuildRangeNumberString(int min, int max, int step, string sign, int? defaultMax)
        {
            StringBuilder sbd = new StringBuilder(string.Empty);
            if (max < min && defaultMax != null)
            {
                max = defaultMax.Value;
            }
            for (int i = min; i <= max; i += step)
            {
                sbd.Append(i + sign);
            }
            if (sbd.Length > 0)
            {
                sbd.Remove(sbd.Length - 1, 1);
            }

            return sbd.ToString();
        }

        /// <summary>
        /// 过滤不安全的sql
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string FilterUnsafeSql(string text)
        {
            string sqlText = text;

            //单引号替换成两个单引号
            sqlText = sqlText.Replace("'", "''");
            //半角封号替换为全角封号，防止多语句执行
            //sqlText = sqlText.Replace(";", "；");

            return sqlText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string FormatSqlDateTime(DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 创建连接串
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static string CreateLinkQueryString(NameValueCollection collection)
        {
            return CreateLinkQueryString(collection, Encoding.UTF8);
        }
        /// <summary>
        /// 创建连接串
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string CreateLinkQueryString(NameValueCollection collection, Encoding encoding)
        {
            StringBuilder stb = new StringBuilder(string.Empty);

            foreach (string key in collection.Keys)
            {
                string value = collection[key];
                stb.AppendFormat("{0}={1}&", WebHelper.UrlEncode(key), WebHelper.UrlEncode(value));
            }
            if (stb.Length > 0)
            {
                stb.Remove(stb.Length - 1, 1);
            }

            return stb.ToString();
        }

        /// <summary>
        /// 去除HTML
        /// </summary>
        /// <param name="htmlstring"></param>
        /// <returns></returns>
        public static string RemoveHTML(string htmlstring)
        {
            if (string.IsNullOrEmpty(htmlstring))
            {
                return string.Empty;
            }
            //删除脚本  
            htmlstring = Regex.Replace(htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML  
            htmlstring = Regex.Replace(htmlstring, @"<\s*br\s*/?>", "  ", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"([\r\n])[\s]+", " ", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);

            htmlstring = Regex.Replace(htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);

            htmlstring = htmlstring.Replace("<", "");
            htmlstring = htmlstring.Replace(">", "");
            htmlstring = htmlstring.Replace("\r\n", " ");
            htmlstring = htmlstring.Replace("\n\r", " ");
            htmlstring = htmlstring.Replace("\n", " ");
            htmlstring = htmlstring.Replace("\r", " ");
            htmlstring = WebHelper.HtmlEncode(htmlstring).Trim();

            return htmlstring;
        }

        /// <summary>
        /// 补全不够长度的数字字符串，如将110填充为0000110
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <param name="len"></param>
        /// <param name="fillStr">为空则使用随机字母</param>
        /// <returns></returns>
        public static string FillText<T>(T text, int len, string fillStr)
        {
            string textStr = text.ToString();
            if (textStr.Length < len)
            {
                int l = len - textStr.Length;

                if (string.IsNullOrEmpty(fillStr))
                {
                    string[] randStr = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
                    Random rand = new Random();

                    for (int i = 0; i < l; i++)
                    {
                        int randnum = rand.Next(1, randStr.Length);

                        textStr = string.Concat(randStr[randnum], textStr);
                    }
                }
                else
                {
                    for (int i = 0; i < l; i++)
                    {
                        textStr = string.Concat(fillStr, textStr);
                    }
                }
            }
            return textStr;
        }

        private static string[] cstr = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };   //定义数组
        private static string[] wstr = { "", "", "十" };
        /// <summary>
        /// 阿拉伯数字转中文
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string ConvertIntToChinese(int num)
        {
            string str = num.ToString();
            int len = str.Length;    //获取输入文本的长度值
            string tmpstr = "";      //定义字符串
            string rstr = "";
            for (int i = 1; i <= len; i++)
            {
                tmpstr = str.Substring(len - i, 1);    //截取输入文本 再将值赋给新的字符串
                rstr = string.Concat(cstr[Int32.Parse(tmpstr)] + wstr[i], rstr);  //将两个数组拼接在一起形成新的字符串
            }
            rstr = rstr.Replace("十零", "十");      //将新的字符串替换
            rstr = rstr.Replace("一十", "十");
            return rstr;                            //返回新的字符串
        }

        /// <summary>
        /// 转换\n\r为HTML换行br标签
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ConvertTextLineToHtmlBreak(string text)
        {
            text = Regex.Replace(text, @"\n\r", "<br />");
            text = Regex.Replace(text, @"\r\n", "<br />");
            text = Regex.Replace(text, @"\n", "<br />");
            text = Regex.Replace(text, @"\r", "<br />");

            return text;
        }


    }
}
