using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Globalization;
using System.Web.SessionState;
using System.Security;
using System.Security.Cryptography;
using System.IO;
using System.Threading;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Net.Mail;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Xml;
using System.Data.SqlClient;
using System.Data;
using System.Data.OleDb;
using System.ComponentModel;
using System.Reflection;
using System.Net;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// 通用数据处理类
    /// </summary>
    public class Common
    {
        #region 与枚举相关
        /// <summary>
        /// 从枚举类型和它的特性读出并返回一个键值对
        /// </summary>
        /// <param name="enumType">Type,该参数的格式为typeof(需要读的枚举类型)</param>
        /// <returns>键值对</returns>
        public static Dictionary<int, string> GetEnumSource(Type enumType)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            Type typeDescription = typeof(DescriptionAttribute);
            System.Reflection.FieldInfo[] fields = enumType.GetFields();
            string strText = string.Empty;
            int val = 0;
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum == true)
                {
                    val = (int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null);
                    object[] arr = field.GetCustomAttributes(typeDescription, true);
                    if (arr.Length > 0)
                    {
                        DescriptionAttribute aa = (DescriptionAttribute)arr[0];
                        strText = aa.Description;
                    }
                    else
                    {
                        strText = field.Name;
                    }
                    dic.Add(val, strText);
                }
            }
            return dic;
        }

        /// <summary>
        /// 根据枚举的value获取描述
        /// </summary>
        /// <param name="enumType">Type,该参数的格式为typeof(需要读的枚举类型)</param>
        /// <param name="value">value</param>
        /// <returns>描述(Text)</returns>
        public static string GetEnumText(Type enumType, int value)
        {
            string text = string.Empty;
            Type typeDescription = typeof(DescriptionAttribute);
            System.Reflection.FieldInfo[] fields = enumType.GetFields();
            string strText = string.Empty;
            int val = 0;
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum == true)
                {
                    val = (int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null);
                    object[] arr = field.GetCustomAttributes(typeDescription, true);
                    if (arr.Length > 0)
                    {
                        DescriptionAttribute aa = (DescriptionAttribute)arr[0];
                        strText = aa.Description;
                    }
                    else
                    {
                        strText = field.Name;
                    }
                    if (val == value)
                    {
                        text = strText;
                        break;
                    }
                }
            }
            return text;
        }
        #endregion

        #region "返回字符串在字符串中出现的次数"
        /// <summary>
        /// 返回字符串在字符串中出现的次数
        /// </summary>
        /// <param name="Char">要检测出现的字符</param>
        /// <param name="String">要检测的字符串</param>
        /// <returns>出现次数</returns>
        public static int GetCharInStringCount(string Char, string String)
        {
            string str = String.Replace(Char, "");
            return (String.Length - str.Length) / Char.Length;

        }
        #endregion

        #region "获得物理路径"
        /// <summary>
        /// 获得物理路径
        /// </summary>
        /// <param name="a">路径</param>
        /// <returns>物理路径</returns>
        public static string GetFullPath(string a)
        {
            string AppDir = System.AppDomain.CurrentDomain.BaseDirectory;
            if (a.IndexOf(":") < 0)
            {
                string str = a.Replace("..\\", "");
                if (str != a)
                {
                    int Num = (a.Length - str.Length) / ("..\\").Length + 1;
                    for (int i = 0; i < Num; i++)
                    {
                        AppDir = AppDir.Substring(0, AppDir.LastIndexOf("\\"));
                    }
                    str = "\\" + str;

                }
                a = AppDir + str;
            }
            return a;
        }
        #endregion

        #region "获得日志文件存放目录"
        /// <summary>
        /// 获得日志文件存放目录
        /// </summary>
        public static string LogDir
        {
            get
            {
                string LogFilePath = GetFullPath(ConfigurationManager.AppSettings["LogDir"]);
                if (!Directory.Exists(LogFilePath))
                    Directory.CreateDirectory(LogFilePath);
                return LogFilePath;
            }
        }
        #endregion

        /*#region "获取用户Form提交字段值"
        /// <summary>
        /// 获取post和get提交值
        /// </summary>
        /// <param name="InputName">字段名</param>
        /// <param name="Method">post和get</param>
        /// <param name="MaxLen">最大允许字符长度 0为不限制</param>
        /// <param name="MinLen">最小字符长度 0为不限制</param>
        /// <param name="DataType">字段数值类型 int 和str和dat不限为为空</param>
        /// <returns></returns>
        public static object sink(string InputName, MethodType Method, int MaxLen, int MinLen, DataType DataType)
        {
            HttpContext rq = HttpContext.Current;
            string TempValue = "";

            #region "获取提交字段数据TempValue"
            if (Method == MethodType.Post)
            {
                if (rq.Request.Form[InputName] != null)
                {
                    TempValue = rq.Request.Form[InputName].ToString();
                }

            }
            else if (Method == MethodType.Get)
            {
                if (rq.Request.QueryString[InputName] != null)
                {
                    TempValue = rq.Request.QueryString[InputName].ToString();
                }
            }
            else
            {
                MessBox("提交数据方式不是post和get!", "?", rq);
                EventMessage.MessageBox(2, "获取数据失败", string.Format("{0}字段提交数据方式不是post和get!", InputName), Icon_Type.Error, "history.back();", UrlType.JavaScript);
            }
            #endregion

            #region "检测最大允许长度"
            if (MaxLen != 0)
            {
                if (TempValue.Length > MaxLen)
                {
                    EventMessage.MessageBox(2, "输入数据格式验证失败", string.Format("{0}字段值：{1}超过系统允许长度{2}!", InputName, TempValue, MaxLen), Icon_Type.Error, "history.back();", UrlType.JavaScript);
                }
            }
            #endregion

            #region "检测最小允许长度"
            if (MinLen != 0)
            {
                if (TempValue.Length < MinLen)
                {
                    EventMessage.MessageBox(2, "输入数据格式验证失败", string.Format("{0}字段值：{1}低于系统允许长度{2}!", InputName, TempValue, MinLen), Icon_Type.Error, "history.back();", UrlType.JavaScript);
                }
            }
            #endregion

            #region "检测数据类型"
            if (TempValue != "")
            {

                switch (DataType)
                {
                    case DataType.Int:
                        int IntTempValue = 0;
                        if (!int.TryParse(TempValue, out IntTempValue))
                            EventMessage.MessageBox(2, "输入数据格式验证失败", string.Format("{0}字段值：{1}数据类型必需为Int型!", InputName, TempValue), Icon_Type.Error, "history.back();", UrlType.JavaScript);
                        return IntTempValue;
                    case DataType.Dat:
                        DateTime DateTempValue = DateTime.MinValue;
                        if (!DateTime.TryParse(TempValue, out DateTempValue))
                            EventMessage.MessageBox(2, "输入数据格式验证失败", string.Format("{0}字段值：{1}数据类型必需为日期型!", InputName, TempValue), Icon_Type.Error, "history.back();", UrlType.JavaScript);
                        return DateTempValue;
                    case DataType.Long:
                        long LongTempValue = long.MinValue;
                        if (!long.TryParse(TempValue, out LongTempValue))
                            EventMessage.MessageBox(2, "输入数据格式验证失败", string.Format("{0}字段值：{1}数据类型必需为Log型!", InputName, TempValue), Icon_Type.Error, "history.back();", UrlType.JavaScript);
                        return LongTempValue;
                    case DataType.Double:
                        double DoubleTempValue = double.MinValue;
                        if (!double.TryParse(TempValue, out DoubleTempValue))
                            EventMessage.MessageBox(2, "输入数据格式验证失败", string.Format("{0}字段值：{1}数据类型必需为Double型!", InputName, TempValue), Icon_Type.Error, "history.back();", UrlType.JavaScript);
                        return DoubleTempValue;
                    case DataType.CharAndNum:
                        if (!CheckRegEx(TempValue, "^[A-Za-z0-9]+$"))
                            EventMessage.MessageBox(2, "输入数据格式验证失败", string.Format("{0}字段值：{1}数据类型必需为英文或数字!", InputName, TempValue), Icon_Type.Error, "history.back();", UrlType.JavaScript);
                        return TempValue;
                    case DataType.CharAndNumAndChinese:
                        if (!CheckRegEx(TempValue, "^[A-Za-z0-9\u00A1-\u2999\u3001-\uFFFD]+$"))
                            EventMessage.MessageBox(2, "输入数据格式验证失败", string.Format("{0}字段值：{1}数据类型必需为英文或数字或中文!", InputName, TempValue), Icon_Type.Error, "history.back();", UrlType.JavaScript);
                        return TempValue;
                    case DataType.Email:
                        if (!CheckRegEx(TempValue, "\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*"))
                            EventMessage.MessageBox(2, "输入数据格式验证失败", string.Format("{0}字段值：{1}数据类型必需为邮件地址!", InputName, TempValue), Icon_Type.Error, "history.back();", UrlType.JavaScript);
                        return TempValue;
                    default:
                        return TempValue;
                }

            }
            else
            {
                switch (DataType)
                {
                    case DataType.Int:
                        return 0;
                    case DataType.Dat:
                        return null;
                    case DataType.Long:
                        return long.MinValue;
                    case DataType.Double:
                        return 0.0f;
                    default:
                        return TempValue;
                }
            }

            #endregion
        }

        #endregion*/

        #region "js信息提示框"
        /// <summary>
        /// js信息提示框
        /// </summary>
        /// <param name="Message">提示信息文字</param>
        /// <param name="ReturnUrl">返回地址</param>
        /// <param name="rq"></param>
        public static void MessBox(string Message, string ReturnUrl, HttpContext rq)
        {
            System.Text.StringBuilder msgScript = new System.Text.StringBuilder();
            msgScript.Append("<script language=JavaScript>\n");
            msgScript.Append("alert(\"" + Message + "\");\n");
            msgScript.Append("parent.location.href='" + ReturnUrl + "';\n");
            msgScript.Append("</script>\n");
            rq.Response.Write(msgScript.ToString());
            rq.Response.End();
        }

        /// <summary>
        /// 弹出Alert信息窗
        /// </summary>
        /// <param name="Message">信息内容</param>
        public static void MessBox(string Message)
        {
            System.Text.StringBuilder msgScript = new System.Text.StringBuilder();
            msgScript.Append("<script language=JavaScript>\n");
            msgScript.Append("alert(\"" + Message + "\");\n");
            msgScript.Append("</script>\n");
            HttpContext.Current.Response.Write(msgScript.ToString());
        }

        #endregion

        #region 格式化字符串,符合SQL语句
        /// <summary>
        /// 格式化字符串,符合SQL语句
        /// </summary>
        /// <param name="formatStr">需要格式化的字符串</param>
        /// <returns>字符串</returns>
        public static string inSQL(string formatStr)
        {
            string rStr = formatStr;
            if (formatStr != null && formatStr != string.Empty)
            {
                rStr = rStr.Replace("'", "''");
                //rStr = rStr.Replace("\"", "\"\"");
            }
            return rStr;
        }
        /// <summary>
        /// 格式化字符串,是inSQL的反向
        /// </summary>
        /// <param name="formatStr"></param>
        /// <returns></returns>
        public static string outSQL(string formatStr)
        {
            string rStr = formatStr;
            if (rStr != null)
            {
                rStr = rStr.Replace("''", "'");
                rStr = rStr.Replace("\"\"", "\"");
            }
            return rStr;
        }

        /// <summary>
        /// 查询SQL语句,删除一些SQL注入问题
        /// </summary>
        /// <param name="formatStr">需要格式化的字符串</param>
        /// <returns></returns>
        public static string querySQL(string formatStr)
        {
            string rStr = formatStr;
            if (rStr != null && rStr != "")
            {
                rStr = rStr.Replace("'", "");
            }
            return rStr;
        }
        #endregion

        #region 截取字符串
        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="str_value"></param>
        /// <param name="str_len"></param>
        /// <returns></returns>
        public static string leftx(string str_value, int str_len)
        {
            int p_num = 0;
            int i;
            string New_Str_value = "";

            if (str_value == "")
            {
                New_Str_value = "";
            }
            else
            {
                int Len_Num = str_value.Length;
                for (i = 0; i <= Len_Num - 1; i++)
                {
                    if (i > Len_Num) break;
                    char c = Convert.ToChar(str_value.Substring(i, 1));
                    if (((int)c > 255) || ((int)c < 0))
                        p_num = p_num + 2;
                    else
                        p_num = p_num + 1;



                    if (p_num >= str_len)
                    {

                        New_Str_value = str_value.Substring(0, i + 1);
                        break;
                    }
                    else
                    {
                        New_Str_value = str_value;
                    }

                }

            }
            return New_Str_value;
        }
        #endregion

        #region 检测用户提交页面
        /// <summary>
        /// 检测用户提交页面
        /// </summary>
        /// <param name="rq"></param>
        public static void Check_Post_Url(HttpContext rq)
        {
            string WebHost = "";
            if (rq.Request.ServerVariables["SERVER_NAME"] != null)
            {
                WebHost = rq.Request.ServerVariables["SERVER_NAME"].ToString();
            }

            string From_Url = "";
            if (rq.Request.UrlReferrer != null)
            {
                From_Url = rq.Request.UrlReferrer.ToString();
            }

            if (From_Url == "" || WebHost == "")
            {
                rq.Response.Write("禁止外部提交数据!");
                rq.Response.End();
            }
            else
            {
                WebHost = "HTTP://" + WebHost.ToUpper();
                From_Url = From_Url.ToUpper();
                int a = From_Url.IndexOf(WebHost);
                if (From_Url.IndexOf(WebHost) < 0)
                {
                    rq.Response.Write("禁止外部提交数据!");
                    rq.Response.End();
                }
            }

        }
        #endregion

        #region 日期处理
        /// <summary>
        /// 格式化日期为2006-12-22
        /// </summary>
        /// <param name="dTime"></param>
        /// <returns></returns>
        public static string formatDate(DateTime dTime)
        {
            string rStr;
            rStr = dTime.Year + "-" + dTime.Month + "-" + dTime.Day;
            return rStr;
        }

        /// <summary>
        /// 获取日期
        /// </summary>
        /// <param name="sDate"></param>
        /// <returns></returns>
        public static string getWeek(DateTime sDate)
        {
            Calendar myCal = CultureInfo.InvariantCulture.Calendar;


            string rStr = "";
            switch (myCal.GetDayOfWeek(sDate).ToString())
            {
                case "Sunday":
                    rStr = "星期日";
                    break;
                case "Monday":
                    rStr = "星期一";
                    break;
                case "Tuesday":
                    rStr = "星期二";
                    break;
                case "Wednesday":
                    rStr = "星期三";
                    break;
                case "Thursday":
                    rStr = "星期四";
                    break;
                case "Friday":
                    rStr = "星期五";
                    break;
                case "Saturday":
                    rStr = "星期六";
                    break;
            }
            return rStr;
        }

        /// <summary>
        /// 转换json的日期（）
        /// </summary>
        /// <param name="jsonDate">日期字符。格式：Date(1242357713797+0800)</param>
        /// <returns></returns>
        public static DateTime JsonToDateTime(string jsonDate)
        {
            string value = jsonDate.Substring(6, jsonDate.Length - 8);
            DateTimeKind kind = DateTimeKind.Utc;
            int index = value.IndexOf('+', 1);
            if (index == -1)
                index = value.IndexOf('-', 1);
            if (index != -1)
            {
                kind = DateTimeKind.Local;
                value = value.Substring(0, index);
            }
            long javaScriptTicks = long.Parse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
            long InitialJavaScriptDateTicks = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;
            DateTime utcDateTime = new DateTime((javaScriptTicks * 10000) + InitialJavaScriptDateTicks, DateTimeKind.Utc);
            DateTime dateTime;
            switch (kind)
            {
                case DateTimeKind.Unspecified:
                    dateTime = DateTime.SpecifyKind(utcDateTime.ToLocalTime(), DateTimeKind.Unspecified);
                    break;
                case DateTimeKind.Local:
                    dateTime = utcDateTime.ToLocalTime();
                    break;
                default:
                    dateTime = utcDateTime;
                    break;
            }
            return dateTime;
        }

        /// <summary>
        /// 日期转换
        /// </summary>
        /// <param name="time">类似"Mon Oct 18 12:28:54 +0800 2010"的时间</param>
        /// <returns>DateTime</returns>
        public static DateTime ConverDateTime(string time)
        {
            string[] cx = time.Split(' ');
            System.Globalization.DateTimeFormatInfo g = new System.Globalization.DateTimeFormatInfo();
            g.LongDatePattern = "dd MMMM yyyy";
            DateTime DT = DateTime.Parse(string.Format("{0} {1} {2} {3}", cx[2], cx[1], cx[5], cx[3]), g);
            return DT;
        }

        /// <summary>
        /// 获取间隔秒数
        /// </summary>
        /// <param name="dt">需要比较的时间</param>
        /// <returns>Int64</returns>
        public static Int64 DateTimeIntervalSeconds(DateTime dt)
        {
            DateTime currDt = DateTime.Now;
            return (currDt - dt).Seconds;
        }

        #endregion

        #region 随机颜色数据

        /// <summary>
        /// 随机颜色数据
        /// </summary>
        /// <returns></returns>
        public static string getStrColor()
        {
            int length = 6;
            byte[] random = new Byte[length / 2];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetNonZeroBytes(random);

            StringBuilder sb = new StringBuilder(length);
            int i;
            for (i = 0; i < random.Length; i++)
            {
                sb.Append(String.Format("{0:X2}", random[i]));
            }
            return sb.ToString();
        }
        #endregion

        #region "隐藏IP地址最后一位用*号代替"
        /// <summary>
        /// 隐藏IP地址最后一位用*号代替
        /// </summary>
        /// <param name="Ipaddress">IP地址:192.168.34.23</param>
        /// <returns></returns>
        public static string HidenLastIp(string Ipaddress)
        {
            return Ipaddress.Substring(0, Ipaddress.LastIndexOf(".")) + ".*";
        }
        #endregion

        #region "防刷新检测"
        /// <summary>
        /// 防刷新检测
        /// </summary>
        /// <param name="Second">访问间隔秒</param>
        /// <param name="UserSession"></param>
        public static bool CheckRefurbish(int Second, HttpSessionState UserSession)
        {

            bool i = true;
            if (UserSession["RefTime"] != null)
            {
                DateTime d1 = Convert.ToDateTime(UserSession["RefTime"]);
                DateTime d2 = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
                TimeSpan d3 = d2.Subtract(d1);
                if (d3.Seconds < Second)
                {
                    i = false;
                }
                else
                {
                    UserSession["RefTime"] = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
                }
            }
            else
            {
                UserSession["RefTime"] = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            }

            return i;
        }
        #endregion

        #region "判断是否是Decimal类型"
        /// <summary>
        /// 判断是否是Decimal类型
        /// </summary>
        /// <param name="TBstr0">判断数据字符</param>
        /// <returns>true是false否</returns>
        public static bool IsDecimal(string TBstr0)
        {
            bool IsBool = false;
            string Intstr0 = "1234567890";
            string IntSign0, StrInt, StrDecimal;
            int IntIndex0, IntSubstr, IndexInt;
            int decimalbool = 0;
            int db = 0;
            bool Bf, Bl;
            if (TBstr0.Length > 2)
            {
                IntIndex0 = TBstr0.IndexOf(".");
                if (IntIndex0 != -1)
                {
                    string StrArr = ".";
                    char[] CharArr = StrArr.ToCharArray();
                    string[] NumArr = TBstr0.Split(CharArr);
                    IndexInt = NumArr.GetUpperBound(0);
                    if (IndexInt > 1)
                    {
                        decimalbool = 1;
                    }
                    else
                    {
                        StrInt = NumArr[0].ToString();
                        StrDecimal = NumArr[1].ToString();
                        //--- 整数部分－－－－－
                        if (StrInt.Length > 0)
                        {
                            if (StrInt.Length == 1)
                            {
                                IntSubstr = Intstr0.IndexOf(StrInt);
                                if (IntSubstr != -1)
                                {
                                    Bf = true;
                                }
                                else
                                {
                                    Bf = false;
                                }
                            }
                            else
                            {
                                for (int i = 0; i <= StrInt.Length - 1; i++)
                                {
                                    IntSign0 = StrInt.Substring(i, 1).ToString();
                                    IntSubstr = Intstr0.IndexOf(IntSign0);
                                    if (IntSubstr != -1)
                                    {
                                        db = db + 0;
                                    }
                                    else
                                    {
                                        db = i + 1;
                                        break;
                                    }
                                }

                                if (db == 0)
                                {
                                    Bf = true;
                                }
                                else
                                {
                                    Bf = false;
                                }
                            }
                        }
                        else
                        {
                            Bf = true;
                        }
                        //----小数部分－－－－
                        if (StrDecimal.Length > 0)
                        {
                            for (int j = 0; j <= StrDecimal.Length - 1; j++)
                            {
                                IntSign0 = StrDecimal.Substring(j, 1).ToString();
                                IntSubstr = Intstr0.IndexOf(IntSign0);
                                if (IntSubstr != -1)
                                {
                                    db = db + 0;
                                }
                                else
                                {
                                    db = j + 1;
                                    break;
                                }
                            }
                            if (db == 0)
                            {
                                Bl = true;
                            }
                            else
                            {
                                Bl = false;
                            }
                        }
                        else
                        {
                            Bl = false;
                        }
                        if ((Bf && Bl) == true)
                        {
                            decimalbool = 0;
                        }
                        else
                        {
                            decimalbool = 1;
                        }

                    }

                }
                else
                {
                    decimalbool = 1;
                }

            }
            else
            {
                decimalbool = 1;
            }

            if (decimalbool == 0)
            {
                IsBool = true;
            }
            else
            {
                IsBool = false;
            }

            return IsBool;
        }
        #endregion

        #region "获取随机数"
        /// <summary>
        /// 获取随机数
        /// </summary>
        /// <param name="length">随机数长度</param>
        /// <returns></returns>
        public static string GetRandomPassword(int length)
        {
            byte[] random = new Byte[length / 2];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetNonZeroBytes(random);

            StringBuilder sb = new StringBuilder(length);
            int i;
            for (i = 0; i < random.Length; i++)
            {
                sb.Append(String.Format("{0:X2}", random[i]));
            }
            return sb.ToString();
        }
        #endregion

        #region "获取用户IP地址"
        /// <summary>
        /// 获取用户IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetIPAddress()
        {

            string user_IP = string.Empty;
            //if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
            //{
            //    if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            //    {
            //        user_IP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            //    }
            //    else
            //    {
            //        user_IP = System.Web.HttpContext.Current.Request.UserHostAddress;
            //    }
            //}
            //else
            //{
            //    user_IP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            //}
            user_IP = System.Web.HttpContext.Current.Request.UserHostAddress;
            return user_IP;
        }
        #endregion

        #region 字符串截取补字符函数
        /// <summary>
        /// 字符串截取补字符函数
        /// </summary>
        /// <param name="s">要处理的字符串</param>
        /// <param name="len">长度</param>
        /// <param name="b">补充的字符</param>
        /// <returns>处理后字符</returns>
        public static string splitStringLen(string s, int len, char b)
        {
            if (string.IsNullOrEmpty(s))
                return "";
            if (s.Length >= len)
                return s.Substring(0, len);
            return s.PadRight(len, b);
        }
        #endregion

        #region "3des加密字符串"
        /// <summary>
        /// 3des加密函数(ECB加密模式,PaddingMode.PKCS7,无IV)
        /// </summary>
        /// <param name="encryptValue">加密字符</param>
        /// <param name="key">加密key(24字符)</param>
        /// <returns>加密后Base64字符</returns>
        public static string EncryptString(string encryptValue, string key)
        {
            string enstring = "加密出错!";
            ICryptoTransform ct; //需要此接口才能在任何服务提供程序上调用 CreateEncryptor 方法，服务提供程序将返回定义该接口的实际 encryptor 对象。
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;
            SymmetricAlgorithm des3 = SymmetricAlgorithm.Create("TripleDES");
            des3.Mode = CipherMode.ECB;
            des3.Key = Encoding.UTF8.GetBytes(splitStringLen(key, 24, '0'));
            //des3.KeySize = 192;
            des3.Padding = PaddingMode.PKCS7;

            ct = des3.CreateEncryptor();

            byt = Encoding.UTF8.GetBytes(encryptValue);//将原始字符串转换成字节数组。大多数 .NET 加密算法处理的是字节数组而不是字符串。

            //创建 CryptoStream 对象 cs 后，现在使用 CryptoStream 对象的 Write 方法将数据写入到内存数据流。这就是进行实际加密的方法，加密每个数据块时，数据将被写入 MemoryStream 对象。

            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            try
            {
                cs.Write(byt, 0, byt.Length);
                cs.FlushFinalBlock();
                enstring = Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                enstring = ex.ToString();
            }
            finally
            {
                cs.Close();
                cs.Dispose();
                ms.Close();
                ms.Dispose();
                des3.Clear();
                ct.Dispose();
            }
            enstring = Convert.ToBase64String(ms.ToArray());
            return enstring;
        }
        #endregion

        #region "3des解密字符串"
        /// <summary>
        /// 3des解密函数(ECB加密模式,PaddingMode.PKCS7,无IV)
        /// </summary>
        /// <param name="decryptString">解密字符</param>
        /// <param name="key">解密key(24字符)</param>
        /// <returns>解密后字符</returns>
        public static string DecryptString(string decryptString, string key)
        {
            string destring = "解密字符失败!";
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;

            SymmetricAlgorithm des3 = SymmetricAlgorithm.Create("TripleDES");
            des3.Mode = CipherMode.ECB;
            des3.Key = Encoding.UTF8.GetBytes(splitStringLen(key, 24, '0'));
            //des3.KeySize = 192;
            des3.Padding = PaddingMode.PKCS7;

            ct = des3.CreateDecryptor();

            byt = Convert.FromBase64String(decryptString);

            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
            try
            {
                cs.Write(byt, 0, byt.Length);
                cs.FlushFinalBlock();
                destring = Encoding.UTF8.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                destring = ex.ToString();
            }
            finally
            {
                ms.Close();
                cs.Close();
                ms.Dispose();
                cs.Dispose();
                ct.Dispose();
                des3.Clear();
            }
            return destring;
        }
        #endregion

        #region "MD5加密"
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="str">加密字符</param>
        /// <param name="code">加密位数16/32</param>
        /// <returns></returns>
        public static string md5(string str, int code)
        {
            string strEncrypt = string.Empty;
            if (code == 16)
            {
                strEncrypt = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").Substring(8, 16);
            }

            if (code == 32)
            {
                strEncrypt = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
            }

            return strEncrypt;
        }
        #endregion

        #region "按当前日期和时间生成随机数"
        /// <summary>
        /// 按当前日期和时间生成随机数
        /// </summary>
        /// <param name="Num">附加随机数长度</param>
        /// <returns></returns>
        public static string sRndNum(int Num)
        {
            string sTmp_Str = System.DateTime.Today.Year.ToString() + System.DateTime.Today.Month.ToString("00") + System.DateTime.Today.Day.ToString("00") + System.DateTime.Now.Hour.ToString("00") + System.DateTime.Now.Minute.ToString("00") + System.DateTime.Now.Second.ToString("00");
            return sTmp_Str + RndNum(Num);
        }
        #endregion

        #region 生成0-9随机数
        /// <summary>
        /// 生成0-9随机数
        /// </summary>
        /// <param name="VcodeNum">生成长度</param>
        /// <returns></returns>
        public static string RndNum(int VcodeNum)
        {
            StringBuilder sb = new StringBuilder(VcodeNum);
            Random rand = new Random();
            for (int i = 1; i < VcodeNum + 1; i++)
            {
                int t = rand.Next(9);
                sb.AppendFormat("{0}", t);
            }
            return sb.ToString();

        }
        #endregion

        #region "通过RNGCryptoServiceProvider 生成随机数 0-9"
        /// <summary>
        /// 通过RNGCryptoServiceProvider 生成随机数 0-9 
        /// </summary>
        /// <param name="length">随机数长度</param>
        /// <returns></returns>
        public static string RndNumRNG(int length)
        {
            byte[] bytes = new byte[16];
            RNGCryptoServiceProvider r = new RNGCryptoServiceProvider();
            StringBuilder sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                r.GetBytes(bytes);
                sb.AppendFormat("{0}", (int)((decimal)bytes[0] / 256 * 10));
            }
            return sb.ToString();

        }
        #endregion

        #region "在当前路径上创建日期格式目录(20060205)"
        /// <summary>
        /// 在当前路径上创建日期格式目录(20060205)
        /// </summary>
        /// <param name="sPath">返回目录名</param>
        /// <returns></returns>
        public static string CreateDir(string sPath)
        {
            string sTemp = System.DateTime.Today.Year.ToString() + System.DateTime.Today.Month.ToString("00") + System.DateTime.Today.Day.ToString("00");
            sPath += sTemp;
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(@sPath); //构造函数创建目录
            if (di.Exists == false)
            {
                di.Create();
            }

            return sTemp;
        }
        #endregion

        #region "检测是否为有效邮件地址格式"
        /// <summary>
        /// 检测是否为有效邮件地址格式
        /// </summary>
        /// <param name="strIn">输入邮件地址</param>
        /// <returns></returns>
        public static bool IsValidEmail(string strIn)
        {
            return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }
        #endregion

        #region "邮件发送"
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="strto">接收邮件地址</param>
        /// <param name="strSubject">主题</param>
        /// <param name="strBody">内容</param>
        public static void SendSMTPEMail(string strto, string strSubject, string strBody)
        {
            string SMTPHost = ConfigurationManager.AppSettings["SMTPHost"];
            string SMTPPort = ConfigurationManager.AppSettings["SMTPPort"];
            string SMTPUser = ConfigurationManager.AppSettings["SMTPUser"];
            string SMTPPassword = ConfigurationManager.AppSettings["SMTPPassword"];
            string MailFrom = ConfigurationManager.AppSettings["MailFrom"];
            string MailSubject = ConfigurationManager.AppSettings["MailSubject"];

            SmtpClient client = new SmtpClient(SMTPHost);
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(SMTPUser, SMTPPassword);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            MailMessage message = new MailMessage(SMTPUser, strto, strSubject, strBody);
            message.BodyEncoding = System.Text.Encoding.GetEncoding("GB2312");
            message.IsBodyHtml = true;

            client.Send(message);
        }
        #endregion

        #region "转换编码"
        /// <summary>
        /// 转换编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Encode(string str)
        {
            if (str == null)
            {
                return "";
            }
            else
            {

                return System.Web.HttpUtility.UrlEncode(Encoding.GetEncoding(54936).GetBytes(str));
            }
        }
        #endregion

        #region "设置页面不被缓存"
        /// <summary>
        /// 设置页面不被缓存
        /// </summary>
        public static void SetPageNoCache()
        {

            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.ExpiresAbsolute = System.DateTime.Now.AddSeconds(-1);
            HttpContext.Current.Response.Expires = 0;
            HttpContext.Current.Response.CacheControl = "no-cache";
            HttpContext.Current.Response.AddHeader("Pragma", "No-Cache");
        }
        #endregion

        #region "获取页面url"
        /// <summary>
        /// 获取当前访问页面地址
        /// </summary>
        public static string GetScriptName
        {
            get
            {
                return HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"].ToString();
            }
        }

        /// <summary>
        /// 检测当前url是否包含指定的字符
        /// </summary>
        /// <param name="sChar">要检测的字符</param>
        /// <returns></returns>
        public static bool CheckScriptNameChar(string sChar)
        {
            bool rBool = false;
            if (GetScriptName.ToLower().LastIndexOf(sChar) >= 0)
                rBool = true;
            return rBool;
        }

        /// <summary>
        /// 获取当前页面的扩展名
        /// </summary>
        public static string GetScriptNameExt
        {
            get
            {
                return GetScriptName.Substring(GetScriptName.LastIndexOf(".") + 1);
            }
        }

        /// <summary>
        /// 获取当前访问页面地址参数
        /// </summary>
        public static string GetScriptNameQueryString
        {
            get
            {
                return HttpContext.Current.Request.ServerVariables["QUERY_STRING"].ToString();
            }
        }

        /// <summary>
        /// 获得页面文件名和参数名
        /// </summary>
        public static string GetScriptNameUrl
        {
            get
            {
                string Script_Name = Common.GetScriptName;
                Script_Name = Script_Name.Substring(Script_Name.LastIndexOf("/") + 1);
                Script_Name += "?" + GetScriptNameQueryString;
                return Script_Name;
            }
        }

        /// <summary>
        /// 获取当前访问页面Url
        /// </summary>
        public static string GetScriptUrl
        {
            get
            {
                return Common.GetScriptNameQueryString == "" ? Common.GetScriptName : string.Format("{0}?{1}", Common.GetScriptName, Common.GetScriptNameQueryString);
            }
        }

        /// <summary>
        /// 返回当前页面目录的url
        /// </summary>
        /// <param name="FileName">文件名</param>
        /// <returns></returns>
        public static string GetHomeBaseUrl(string FileName)
        {
            string Script_Name = Common.GetScriptName;
            return string.Format("{0}/{1}", Script_Name.Remove(Script_Name.LastIndexOf("/")), FileName);
        }

        /// <summary>
        /// 返回当前网站网址
        /// </summary>
        /// <returns></returns>
        public static string GetHomeUrl()
        {
            return HttpContext.Current.Request.Url.Authority;
        }

        /// <summary>
        /// 获取当前访问文件物理目录
        /// </summary>
        /// <returns>路径</returns>
        public static string GetScriptPath
        {
            get
            {
                string Paths = HttpContext.Current.Request.ServerVariables["PATH_TRANSLATED"].ToString();
                return Paths.Remove(Paths.LastIndexOf("\\"));
            }
        }
        #endregion

        #region "按字符串位数补0"
        /// <summary>
        /// 按字符串位数补0
        /// </summary>
        /// <param name="CharTxt">字符串</param>
        /// <param name="CharLen">字符长度</param>
        /// <returns></returns>
        public static string FillZero(string CharTxt, int CharLen)
        {
            if (CharTxt.Length < CharLen)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < CharLen - CharTxt.Length; i++)
                {
                    sb.Append("0");
                }
                sb.Append(CharTxt);
                return sb.ToString();
            }
            else
            {
                return CharTxt;
            }
        }

        #endregion

        #region "替换JS中特殊字符"
        /// <summary>
        /// 将JS中的特殊字符替换
        /// </summary>
        /// <param name="str">要替换字符</param>
        /// <returns></returns>
        public static string ReplaceJs(string str)
        {

            if (str != null)
            {
                str = str.Replace("\"", "&quot;");
                str = str.Replace("(", "&#40;");
                str = str.Replace(")", "&#41;");
                str = str.Replace("%", "&#37;");
            }

            return str;

        }
        #endregion

        #region "正式表达式验证"
        /// <summary>
        /// 正式表达式验证
        /// </summary>
        /// <param name="C_Value">验证字符</param>
        /// <param name="C_Str">正式表达式</param>
        /// <returns>符合true不符合false</returns>
        public static bool CheckRegEx(string C_Value, string C_Str)
        {
            Regex objAlphaPatt;
            objAlphaPatt = new Regex(C_Str, RegexOptions.Compiled);


            return objAlphaPatt.Match(C_Value).Success;
        }
        #endregion

        #region "检测当前字符是否在以,号分开的字符串中(xx,sss,xaf,fdsf)"
        /// <summary>
        /// 检测当前字符是否在以,号分开的字符串中(xx,sss,xaf,fdsf)
        /// </summary>
        /// <param name="TempChar">需检测字符</param>
        /// <param name="TempStr">待检测字符串</param>
        /// <returns>存在true,不存在false</returns>
        public static bool Check_Char_Is(string TempChar, string TempStr)
        {
            bool rBool = false;
            if (TempChar != null && TempStr != null)
            {
                string[] TempStrArray = TempStr.Split(',');
                for (int i = 0; i < TempStrArray.Length; i++)
                {
                    if (TempChar == TempStrArray[i].Trim())
                    {
                        rBool = true;
                        break;
                    }
                }
            }
            return rBool;
        }
        #endregion

        #region "产生GUID"
        /// <summary>
        /// 获取一个GUID字符串
        /// </summary>
        public static Guid GetGUID
        {
            get
            {
                return Guid.NewGuid();
            }
        }
        #endregion

        #region "获取服务器IP"
        /// <summary>
        /// 获取服务器IP
        /// </summary>
        public static string GetServerIp
        {
            get
            {
                return HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"].ToString();
            }
        }
        #endregion

        #region "获取服务器操作系统"
        /// <summary>
        /// 获取服务器操作系统
        /// </summary>
        public static string GetServerOS
        {
            get
            {
                return Environment.OSVersion.VersionString;
            }
        }
        #endregion

        #region "获取服务器域名"
        /// <summary>
        /// 获取服务器域名
        /// </summary>
        public static string GetServerHost
        {
            get
            {
                return HttpContext.Current.Request.ServerVariables["SERVER_NAME"].ToString();
            }
        }
        #endregion

        #region "根据文件扩展名获取当前目录下的文件列表"
        /// <summary>
        /// 根据文件扩展名获取当前目录下的文件列表
        /// </summary>
        /// <param name="FileExt">文件扩展名</param>
        /// <returns>返回文件列表</returns>
        public static List<string> GetDirFileList(string FileExt)
        {
            List<string> FilesList = new List<string>();
            string[] Files = Directory.GetFiles(GetScriptPath, string.Format("*.{0}", FileExt));
            foreach (string var in Files)
            {
                FilesList.Add(System.IO.Path.GetFileName(var).ToLower());
            }
            return FilesList;
        }
        #endregion

        #region "根据文件扩展名获得文件的content-type"
        /// <summary>
        /// 根据文件扩展名获得文件的content-type
        /// </summary>
        /// <param name="fileextension">文件扩展名如.gif</param>
        /// <returns>文件对应的content-type 如:application/gif</returns>
        public static string GetFileMIME(string fileextension)
        {
            //set the default content-type
            const string DEFAULT_CONTENT_TYPE = "application/unknown";

            RegistryKey regkey, fileextkey;
            string filecontenttype;

            //the file extension to lookup


            try
            {
                //look in HKCR
                regkey = Registry.ClassesRoot;

                //look for extension
                fileextkey = regkey.OpenSubKey(fileextension);

                //retrieve Content Type value
                filecontenttype = fileextkey.GetValue("Content Type", DEFAULT_CONTENT_TYPE).ToString();

                //cleanup
                fileextkey = null;
                regkey = null;
            }
            catch
            {
                filecontenttype = DEFAULT_CONTENT_TYPE;
            }

            return filecontenttype;
        }
        #endregion

        #region 文件处理
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="FilePath">删除的文件路径</param>
        /// <param name="PathType">删除文件路径类型</param>
        /// <returns>成功/失败</returns>
        public static bool DeleteFile(string FilePath, DeleteFilePathType PathType)
        {
            if (FilePath == null || FilePath == string.Empty)
                return false;
            bool rBool = false;
            switch (PathType)
            {
                case DeleteFilePathType.DummyPath:
                    FilePath = HttpContext.Current.Server.MapPath(FilePath);
                    break;
                case DeleteFilePathType.NowDirectoryPath:
                    FilePath = HttpContext.Current.Server.MapPath(FilePath);
                    break;
                case DeleteFilePathType.PhysicsPath:
                    break;
            }
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
                rBool = true;
            }
            return rBool;
        }



        /// <summary>
        /// 删除文件夹以文件夹中所有文件
        /// </summary>
        /// <param name="FilePath">删除的文件夹名称</param>
        /// <param name="PathType">删除文件夹路径类型</param>
        public static void DeleteFolder(string folderPath, DeleteFilePathType PathType)
        {
            try
            {
                switch (PathType)
                {
                    case DeleteFilePathType.DummyPath:
                        folderPath = HttpContext.Current.Server.MapPath(folderPath);
                        break;
                    case DeleteFilePathType.NowDirectoryPath:
                        folderPath = HttpContext.Current.Server.MapPath(folderPath);
                        break;
                    case DeleteFilePathType.PhysicsPath:
                        break;
                }
            }
            catch { }

            if (Directory.Exists(folderPath))
            {
                // 循环文件夹里面的内容        
                foreach (string f in Directory.GetFileSystemEntries(folderPath))
                {
                    if (File.Exists(f))
                    {
                        FileInfo fi = new FileInfo(f);
                        if (fi.Attributes.ToString().IndexOf("Readonly") != 1)
                        {
                            fi.Attributes = FileAttributes.Normal;
                        }
                        File.Delete(f);
                    }
                    else
                    {
                        DeleteFolder(f, PathType);
                    }
                }
                // 删除已空文件夹        
                Directory.Delete(folderPath);
            }
        }

        /// <summary>  
        /// 复制文件  
        /// </summary>  
        /// <param name="oldFileUrlList">待复制的文件Url集合</param>
        /// <param name="newFileUrlList">文件复制目标路径集合</param>
        public static void CopyFiles(List<string> oldFileUrlList, List<string> newFileUrlList)
        {
            //传递的数据如果有误  则不执行复制
            if (oldFileUrlList.Count != newFileUrlList.Count) return;

            int fileCount = oldFileUrlList.Count;
            for (int i = 0; i < fileCount; i++)
            {
                string oldFileUrl = oldFileUrlList[i];
                string newFileUrl = newFileUrlList[i];
                if (oldFileUrl != null && newFileUrl != null)
                {
                    if (oldFileUrl.Trim() != string.Empty && newFileUrl != string.Empty)
                    {
                        if (File.Exists(oldFileUrl))
                        {
                            string newFilePath = newFileUrl.Replace(Path.GetFileName(newFileUrl), string.Empty);
                            if (!Directory.Exists(newFilePath))
                                Directory.CreateDirectory(newFilePath);
                            File.Copy(oldFileUrl, newFileUrl, true);
                        }
                    }
                }
            }
        }
        #endregion

        #region "获得操作系统"
        /// <summary>
        /// 获得操作系统
        /// </summary>
        /// <returns>操作系统名称</returns>
        public static string GetSystem
        {
            get
            {
                string s = HttpContext.Current.Request.UserAgent.Trim().Replace("(", "").Replace(")", "");
                string[] sArray = s.Split(';');
                switch (sArray[2].Trim())
                {
                    case "Windows 4.10":
                        s = "Windows 98";
                        break;
                    case "Windows 4.9":
                        s = "Windows Me";
                        break;
                    case "Windows NT 5.0":
                        s = "Windows 2000";
                        break;
                    case "Windows NT 5.1":
                        s = "Windows XP";
                        break;
                    case "Windows NT 5.2":
                        s = "Windows 2003";
                        break;
                    case "Windows NT 6.0":
                        s = "Windows Vista";
                        break;
                    default:
                        s = "Other";
                        break;
                }
                return s;
            }
        }


        #endregion

        #region "获得sessionid"
        /// <summary>
        /// 获得sessionid
        /// </summary>
        public static string GetSessionID
        {
            get
            {
                return HttpContext.Current.Session.SessionID;
            }
        }
        #endregion

        #region "进行base64编码"
        /// <summary>
        /// 进行base64编码
        /// </summary>
        /// <param name="s">字符</param>
        /// <returns></returns>
        public static string EnBase64(string s)
        {
            return Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(s));
        }
        #endregion

        #region "进行Base64解码"
        /// <summary>
        /// 进行Base64解码
        /// </summary>
        /// <param name="s">字符</param>
        /// <returns></returns>
        public static string DeBase64(string s)
        {
            return System.Text.Encoding.Default.GetString(Convert.FromBase64String(s));
        }
        #endregion

        #region "将日期类型转换成字符"
        /// <summary>
        /// 将日期类型转换成字符
        /// </summary>
        /// <param name="s">日期</param>
        /// <returns>字符</returns>
        public static string ConvertDate(DateTime? s)
        {
            if (s.HasValue)
                return s.Value.ToString("yyyy/MM/dd");
            else
                return "";
        }
        #endregion

        #region "格式化TextArea输入内容为html显示"
        /// <summary>
        /// 格式化TextArea输入内容为html显示
        /// </summary>
        /// <param name="s">要格式化内容</param>
        /// <returns>完成内容</returns>
        public static string FormatTextArea(string s)
        {
            s = s.Replace("\n", "<br>");
            s = s.Replace("\x20", "&nbsp;");
            return s;
        }
        #endregion

        #region "检测Ip地址是否正确"
        /// <summary>
        /// 检测Ip地址是否正确
        /// </summary>
        /// <param name="ip">ip字符串</param>
        /// <returns>正确/不正确</returns>
        public static bool CheckIp(string ip)
        {
            System.Net.IPAddress ipa;
            if (System.Net.IPAddress.TryParse(ip, out ipa))
            {
                ipa = null;
                return true;
            }
            else
            {
                ipa = null;
                return false;
            }
        }
        #endregion

        #region "格式化日期24小时制为字符串如:2008/12/12 21:22:33"
        /// <summary>
        /// 格式化日期24小时制为字符串如:2008/12/12 21:22:33
        /// </summary>
        /// <param name="d">日期</param>
        /// <returns>字符</returns>
        public static string FormatDateToString(DateTime d)
        {
            return d.ToString("yyyy/MM/dd HH:mm:ss");
        }
        #endregion

        #region "格式化日期显示为字符"
        /// <summary>
        /// 格式化日期显示为字符
        /// </summary>
        /// <param name="d">日期</param>
        /// <returns></returns>
        public static string FormatDateToDispString(DateTime d)
        {
            return d.ToString("yyyy/MM/dd HH:mm:ss");
        }
        #endregion

        #region "格式化为UTC时间"
        /// <summary>
        /// 格式化为UTC时间
        /// </summary>
        /// <param name="d">时间</param>
        /// <returns>格式化日期</returns>
        public static DateTime FormatDateToUTC(DateTime d)
        {
            return d.ToUniversalTime();
        }

        /// <summary>
        /// 格式化为UTC时间
        /// </summary>
        /// <param name="d">时间字符</param>
        /// <returns>时间</returns>
        public static DateTime FormatDateToUTC(string d)
        {
            return Convert.ToDateTime(d).ToUniversalTime();
        }
        #endregion


        /// <summary>
        /// 根据指定字符拆分字符串到数组
        /// </summary>
        /// <param name="str"></param>
        /// <param name="splitStr"></param>
        /// <param name="isRemoveEmptyEntries"></param>
        /// <returns></returns>
        public static string[] SplitStr(string str, string splitStr = ",", bool isRemoveEmptyEntries = true)
        {
            if (str == string.Empty) { return new string[] { }; }
            else
            {
                if (isRemoveEmptyEntries)
                {
                    return str.Split(new string[] { splitStr }, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    return str.Split(new string[] { splitStr }, StringSplitOptions.None);
                }
            }
        }

        /// <summary>
        /// 获取字符串中所有url连链接
        /// </summary>
        /// <param name="content">需要查询的字符串</param>
        /// <returns></returns>
        public static string[] GetUrls(string content)
        {
            List<string> listUrl = new List<string>();

            string pattern = @"(((ht|f)tps?://)|(www\.))([\w-]+\.)+[\w-:]+(/[\w- ./?%#;&=]*)?";
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(content);

            System.Collections.IEnumerator enu = matches.GetEnumerator();
            while (enu.MoveNext() && enu.Current != null)
            {
                Match match = (Match)(enu.Current);
                listUrl.Add(match.Value);
            }
            return listUrl.ToArray();
        }
        /// <summary>
        /// 将文件转换成字符串,常用于读取网站模板
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isSpace"></param>
        /// <returns></returns>
        public static string GetTempleContent(string path)
        {
            string result = string.Empty;
            string sFileName = HttpContext.Current.Server.MapPath(path);
            if (File.Exists(sFileName))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(sFileName))
                    {
                        result = sr.ReadToEnd();
                    }
                }
                catch
                {
                    result = "读取模板文件(" + path + ")出错";
                }
            }
            else
            {
                result = "找不到模板文件：" + path;
            }
            return result;
        }

        /// <summary>
        /// 读取,添加，修改xml文件
        /// </summary>
        /// <param name="Xmlpath">Xml路径</param>
        /// <param name="Node">新的子节点名称</param>
        /// <param name="Value">新节点对应的值</param>
        /// <param name="flag">1：读取，否则为 修改或者添加</param>
        /// <returns>1：修改添加成功，为空字符串表示修改添加成功，否则是读取成功</returns>
        public static string getXML(string Xmlpath, string Node, string Value, int flag)
        {
            try
            {
                string filepath = HttpContext.Current.Server.MapPath(Xmlpath);
                XmlDocument xmlDoc = new XmlDocument();
                if (!File.Exists(filepath))
                {
                    XmlDeclaration xn = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
                    XmlElement root = xmlDoc.CreateElement("rss");
                    XmlElement root1 = xmlDoc.CreateElement("item");

                    root.AppendChild(root1);
                    xmlDoc.AppendChild(xn);
                    xmlDoc.AppendChild(root);
                    xmlDoc.Save(filepath);//本地路径名字
                }
                xmlDoc.Load(filepath);//你的xml文件
                string ReStr = string.Empty;
                XmlElement xmlObj = xmlDoc.DocumentElement;

                XmlNodeList xmlList = xmlDoc.SelectSingleNode(xmlObj.Name.ToString()).ChildNodes;

                foreach (XmlNode xmlNo in xmlList)
                {
                    if (xmlNo.NodeType != XmlNodeType.Comment)//判断是不是注释类型
                    {
                        XmlElement xe = (XmlElement)xmlNo;
                        {
                            if (xe.Name == xmlObj.FirstChild.Name)
                            {
                                XmlNodeList xmlNList = xmlObj.FirstChild.ChildNodes;

                                foreach (XmlNode xmld in xmlNList)
                                {
                                    XmlElement xe1 = (XmlElement)xmld;
                                    {
                                        if (xe1.Name == Node)
                                        {
                                            if (flag == 1)//读取值
                                            {
                                                if (xmld.InnerText != null && xmld.InnerText != "")
                                                {
                                                    ReStr = xmld.InnerText;
                                                }
                                            }
                                            else//修改值
                                            {
                                                xmld.InnerText = Value;//给节点赋值
                                                xmlDoc.Save(filepath);
                                                ReStr = Value.Trim();
                                            }
                                        }
                                    }
                                }
                                if (ReStr == string.Empty)// 添加节点
                                {
                                    XmlNode newNode;
                                    newNode = xmlDoc.CreateNode("element", Node, Value);//创建节点
                                    newNode.InnerText = Value;//给节点赋值
                                    xe.AppendChild(newNode);//把节点添加到doc
                                    xmlDoc.Save(filepath);
                                    ReStr = Value.Trim();
                                }
                            }
                        }
                    }
                }
                return ReStr;
            }
            catch
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// 取得文件扩展名
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>扩展名</returns>
        public static string GetFileEXT(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return "";
            }
            if (filename.IndexOf(@".") == -1)
            {
                return "";
            }
            int pos = -1;
            if (!(filename.IndexOf(@"\") == -1))
            {
                pos = filename.LastIndexOf(@"\");
            }
            string[] s = filename.Substring(pos + 1).Split('.');
            return s[1];
        }



        /// <summary>
        /// 去掉结尾，
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string LostDot(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            else
            {
                if (input.IndexOf(",") > -1)
                {
                    int intLast = input.LastIndexOf(",");
                    if ((intLast + 1) == input.Length)
                    {
                        return input.Remove(intLast);
                    }
                    else
                    {
                        return input;
                    }
                }
                else
                {
                    return input;
                }
            }
        }

        /// <summary>
        /// 去掉首尾逗号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ClearComma(string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length == Common.GetCharInStringCount(",", input))
            {
                return string.Empty;
            }

            string startStr = string.Empty;
            do
            {
                startStr = input.Substring(0, 1);
                if (startStr == ",")
                {
                    input = input.Substring(1, input.Length - 1);
                }
            } while (startStr == ",");

            string endStr = string.Empty;
            do
            {
                endStr = input.Substring(input.Length - 1, 1);
                if (endStr == ",")
                {
                    input = input.Substring(0, input.Length - 1);
                }
            } while (endStr == ",");

            return input;
        }


        /// <summary>
        /// 生成任意位数的随机数
        /// </summary>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <returns></returns>
        public int GetRandom(int minValue, int maxValue)
        {
            Random ri = new Random(unchecked((int)DateTime.Now.Ticks));
            int k = ri.Next(minValue, maxValue);
            return k;
        }

        /// <summary>
        /// 判断输入是否为日期类型
        /// </summary>
        /// <param name="s">待检查数据</param>
        /// <returns></returns>
        public static bool IsDate(string s)
        {
            if (s == null)
            {
                return false;
            }
            else
            {
                try
                {
                    DateTime d = DateTime.Parse(s);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }


        /// <summary>
        /// MD5加密字符串处理
        /// </summary>
        /// <param name="Half">加密是16位还是32位；如果为true为16位</param>
        public static string MD5(string Input, bool Half)
        {
            string output = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(Input, "MD5").ToLower();
            if (Half) output = output.Substring(8, 16);
            return output;
        }

        /// <summary>
        /// MD5加密字符串处理
        /// </summary>
        /// <param name="Input">要加密的字符串</param>
        public static string MD5(string Input)
        {
            return MD5(Input, true);
        }



        /// <summary>
        /// RSA加密函数
        /// </summary>
        /// <param name="xmlPublicKey">说明KEY必须是XML的行式,返回的是字符串</param>
        /// <param name="EncryptString"></param>
        /// <returns></returns>
        public string RSAEncrypt(string xmlPublicKey, string EncryptString)
        {
            byte[] PlainTextBArray;
            byte[] CypherTextBArray;
            string Result;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPublicKey);
            PlainTextBArray = (new UnicodeEncoding()).GetBytes(EncryptString);
            CypherTextBArray = rsa.Encrypt(PlainTextBArray, false);
            Result = Convert.ToBase64String(CypherTextBArray);
            return Result;
        }

        /// <summary>
        /// RSA解密函数
        /// </summary>
        /// <param name="xmlPrivateKey"></param>
        /// <param name="DecryptString"></param>
        /// <returns></returns>
        public string RSADecrypt(string xmlPrivateKey, string DecryptString)
        {
            byte[] PlainTextBArray;
            byte[] DypherTextBArray;
            string Result;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xmlPrivateKey);
            PlainTextBArray = Convert.FromBase64String(DecryptString);
            DypherTextBArray = rsa.Decrypt(PlainTextBArray, false);
            Result = (new UnicodeEncoding()).GetString(DypherTextBArray);
            return Result;
        }

        /// <summary>
        /// 产生RSA的密钥
        /// </summary>
        /// <param name="xmlKeys">私钥</param>
        /// <param name="xmlPublicKey">公钥</param>
        public void RSAKey(out string xmlKeys, out string xmlPublicKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            xmlKeys = rsa.ToXmlString(true);
            xmlPublicKey = rsa.ToXmlString(false);
        }

        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        /// <param name="strValue">过期时间(分钟)</param>
        public static void WriteCookie(string strName, string strValue, int expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Value = strValue;
            cookie.Expires = DateTime.Now.AddMinutes(expires);
            HttpContext.Current.Response.AppendCookie(cookie);
        }
        /// <summary>
        /// 读cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <returns>cookie值</returns>
        public static string GetCookie(string strName)
        {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[strName] != null)
            {
                return HttpContext.Current.Request.Cookies[strName].Value.ToString();
            }
            return "";
        }

        /// <summary> 
        /// 取单个字符的拼音声母 
        /// </summary> 
        /// <param name="c">要转换的单个汉字</param> 
        /// <returns>拼音声母</returns> 
        public static string GetPYChar(string c)
        {
            byte[] array = new byte[2];
            array = System.Text.Encoding.Default.GetBytes(c);
            int i = (short)(array[0] - '\0') * 256 + ((short)(array[1] - '\0'));
            if (i < 0xB0A1) return "*";
            if (i < 0xB0C5) return "A";
            if (i < 0xB2C1) return "B";
            if (i < 0xB4EE) return "C";
            if (i < 0xB6EA) return "D";
            if (i < 0xB7A2) return "E";
            if (i < 0xB8C1) return "F";
            if (i < 0xB9FE) return "G";
            if (i < 0xBBF7) return "H";
            if (i < 0xBFA6) return "G";
            if (i < 0xC0AC) return "K";
            if (i < 0xC2E8) return "L";
            if (i < 0xC4C3) return "M";
            if (i < 0xC5B6) return "N";
            if (i < 0xC5BE) return "O";
            if (i < 0xC6DA) return "P";
            if (i < 0xC8BB) return "Q";
            if (i < 0xC8F6) return "R";
            if (i < 0xCBFA) return "S";
            if (i < 0xCDDA) return "T";
            if (i < 0xCEF4) return "W";
            if (i < 0xD1B9) return "X";
            if (i < 0xD4D1) return "Y";
            if (i < 0xD7FA) return "Z";
            return "*";
        }
        //变量.ToString()

        //   //字符型转换 转为字符串  
        //   12345.ToString("n");        //生成   12,345.00  
        //   12345.ToString("C");        //生成 ￥12,345.00  
        //   12345.ToString("e");        //生成 1.234500e+004  
        //   12345.ToString("f4");        //生成 12345.0000  
        //   12345.ToString("x");         //生成 3039  (16进制)  
        //   12345.ToString("p");         //生成 1,234,500.00%  
        //时间的处理

        //DateTime dt = DateTime.Now;
        //Label1.Text = dt.ToString();//2005-11-5 13:21:25
        //Label2.Text = dt.ToFileTime().ToString();//127756416859912816
        //Label3.Text = dt.ToFileTimeUtc().ToString();//127756704859912816
        //Label4.Text = dt.ToLocalTime().ToString();//2005-11-5 21:21:25
        //Label5.Text = dt.ToLongDateString().ToString();//2005年11月5日
        //Label6.Text = dt.ToLongTimeString().ToString();//13:21:25
        //Label7.Text = dt.ToOADate().ToString();//38661.5565508218
        //Label8.Text = dt.ToShortDateString().ToString();//2005-11-5
        //Label9.Text = dt.ToShortTimeString().ToString();//13:21
        //Label10.Text = dt.ToUniversalTime().ToString();//2005-11-5 5:21:25
        //?2005-11-5 13:30:28.4412864
        //Label1.Text = dt.Year.ToString();//2005
        //Label2.Text = dt.Date.ToString();//2005-11-5 0:00:00
        //Label3.Text = dt.DayOfWeek.ToString();//Saturday
        //Label4.Text = dt.DayOfYear.ToString();//309
        //Label5.Text = dt.Hour.ToString();//13
        //Label6.Text = dt.Millisecond.ToString();//441
        //Label7.Text = dt.Minute.ToString();//30
        //Label8.Text = dt.Month.ToString();//11
        //Label9.Text = dt.Second.ToString();//28
        //Label10.Text = dt.Ticks.ToString();//632667942284412864
        //Label11.Text = dt.TimeOfDay.ToString();//13:30:28.4412864
        //Label1.Text = dt.ToString();//2005-11-5 13:47:04
        //Label2.Text = dt.AddYears(1).ToString();//2006-11-5 13:47:04
        //Label3.Text = dt.AddDays(1.1).ToString();//2005-11-6 16:11:04
        //Label4.Text = dt.AddHours(1.1).ToString();//2005-11-5 14:53:04
        //Label5.Text = dt.AddMilliseconds(1.1).ToString();//2005-11-5 13:47:04
        //Label6.Text = dt.AddMonths(1).ToString();//2005-12-5 13:47:04
        //Label7.Text = dt.AddSeconds(1.1).ToString();//2005-11-5 13:47:05
        //Label8.Text = dt.AddMinutes(1.1).ToString();//2005-11-5 13:48:10
        //Label9.Text = dt.AddTicks(1000).ToString();//2005-11-5 13:47:04
        //Label10.Text = dt.CompareTo(dt).ToString();//0
        ////Label11.Text = dt.Add(?).ToString();//问号为一个时间段
        //Label1.Text = dt.Equals("2005-11-6 16:11:04").ToString();//False
        //Label2.Text = dt.Equals(dt).ToString();//True
        //Label3.Text = dt.GetHashCode().ToString();//1474088234
        //Label4.Text = dt.GetType().ToString();//System.DateTime
        //Label5.Text = dt.GetTypeCode().ToString();//DateTime

        //Label1.Text = dt.GetDateTimeFormats('s')[0].ToString();//2005-11-05T14:06:25
        //Label2.Text = dt.GetDateTimeFormats('t')[0].ToString();//14:06
        //Label3.Text = dt.GetDateTimeFormats('y')[0].ToString();//2005年11月
        //Label4.Text = dt.GetDateTimeFormats('D')[0].ToString();//2005年11月5日
        //Label5.Text = dt.GetDateTimeFormats('D')[1].ToString();//2005 11 05
        //Label6.Text = dt.GetDateTimeFormats('D')[2].ToString();//星期六 2005 11 05
        //Label7.Text = dt.GetDateTimeFormats('D')[3].ToString();//星期六 2005年11月5日
        //Label8.Text = dt.GetDateTimeFormats('M')[0].ToString();//11月5日
        //Label9.Text = dt.GetDateTimeFormats('f')[0].ToString();//2005年11月5日 14:06
        //Label10.Text = dt.GetDateTimeFormats('g')[0].ToString();//2005-11-5 14:06
        //Label11.Text = dt.GetDateTimeFormats('r')[0].ToString();//Sat, 05 Nov 2005 14:06:25 GMT

        //Label1.Text =? string.Format("{0:d}",dt);//2005-11-5
        //Label2.Text =? string.Format("{0:D}",dt);//2005年11月5日
        //Label3.Text =? string.Format("{0:f}",dt);//2005年11月5日 14:23
        //Label4.Text =? string.Format("{0:F}",dt);//2005年11月5日 14:23:23
        //Label5.Text =? string.Format("{0:g}",dt);//2005-11-5 14:23
        //Label6.Text =? string.Format("{0:G}",dt);//2005-11-5 14:23:23
        //Label7.Text =? string.Format("{0:M}",dt);//11月5日
        //Label8.Text =? string.Format("{0:R}",dt);//Sat, 05 Nov 2005 14:23:23 GMT
        //Label9.Text =? string.Format("{0:s}",dt);//2005-11-05T14:23:23
        //Label10.Text = string.Format("{0:t}",dt);//14:23
        //Label11.Text = string.Format("{0:T}",dt);//14:23:23
        //Label12.Text = string.Format("{0:u}",dt);//2005-11-05 14:23:23Z
        //Label13.Text = string.Format("{0:U}",dt);//2005年11月5日 6:23:23
        //Label14.Text = string.Format("{0:Y}",dt);//2005年11月
        //Label15.Text = string.Format("{0}",dt);//2005-11-5 14:23:23?
        //Label16.Text = string.Format("{0:yyyyMMddHHmmssffff}",dt); //yyyymm等可以设置,比如Label16.Text = string.Format("{0:yyyyMMdd}",dt);
        //获得ip和mac地址



        [DllImport("Iphlpapi.dll")]
        private static extern int SendARP(Int32 dest, Int32 host, ref Int64 mac, ref Int32 length);
        [DllImport("Ws2_32.dll")]
        private static extern Int32 inet_addr(string ip);

        public string GetMACAddress()
        {
            string userip = HttpContext.Current.Request.UserHostAddress;
            string strClientIP = HttpContext.Current.Request.UserHostAddress.ToString().Trim();
            Int32 ldest = inet_addr(strClientIP); //目的地的ip 
            Int32 lhost = inet_addr("");   //本地服务器的ip 
            Int64 macinfo = new Int64();
            Int32 len = 6;
            int res = SendARP(ldest, 0, ref macinfo, ref len);
            string mac_src = macinfo.ToString("X");
            if (mac_src == "0")
            {
                return string.Empty;
            }

            while (mac_src.Length < 12)
            {
                mac_src = mac_src.Insert(0, "0");
            }

            string mac_dest = "";

            for (int i = 0; i < 11; i++)
            {
                if (0 == (i % 2))
                {
                    if (i == 10)
                    {
                        mac_dest = mac_dest.Insert(0, mac_src.Substring(i, 2));
                    }
                    else
                    {
                        mac_dest = "-" + mac_dest.Insert(0, mac_src.Substring(i, 2));
                    }
                }
            }
            return mac_dest;

        }


        /// <summary>
        /// 获取客户端IP
        /// </summary>
        /// <returns></returns>
        private string GetClientIP()
        {
            string result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            } if (null == result || result == String.Empty)
            {
                result = HttpContext.Current.Request.UserHostAddress;
            }
            return result;
        }

        /// <summary>
        /// 调用Win32 Api函数(MessageBox)，非托管DLL
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="text"></param>
        /// <param name="caption"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "MessageBox", ExactSpelling = false)]
        public static extern int MessageBox(int hWnd, string text, string caption, uint type);




        /// <summary>
        /// 保存文件到SQL Server数据库中
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="tableName">数据库表名</param>
        /// <param name="fieldName">数据库字段名</param>
        public void FileToSql(string fileName, string tableName, string fieldName)
        {
            SqlConnection cn = new SqlConnection();
            FileInfo fi = new FileInfo(fileName);
            FileStream fs = fi.OpenRead();
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
            SqlCommand cm = new SqlCommand();
            cm.Connection = cn;
            cm.CommandType = CommandType.Text;
            if (cn.State == 0) cn.Open();
            cm.CommandText = "insert into " + tableName + "(" + fieldName + ") values(@file)";
            SqlParameter spFile = new SqlParameter("@file", SqlDbType.Image);
            spFile.Value = bytes;
            cm.Parameters.Add(spFile);
            cm.ExecuteNonQuery();
        }
        /// <summary>
        /// 保存文件到Access数据库中
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="tableName">数据库表名</param>
        /// <param name="fieldName">数据库字段名</param>
        public void FileToAccess(string fileName, string tableName, string fieldName)
        {
            OleDbConnection cn = new OleDbConnection();
            FileInfo fi = new FileInfo(fileName);
            FileStream fs = fi.OpenRead();
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
            OleDbCommand cm = new OleDbCommand();
            cm.Connection = cn;
            cm.CommandType = CommandType.Text;
            if (cn.State == 0) cn.Open();
            cm.CommandText = "insert into " + tableName + "(" + fieldName + ") values(@file)";
            OleDbParameter spFile = new OleDbParameter("@file", OleDbType.Binary);
            spFile.Value = bytes;
            cm.Parameters.Add(spFile);
            cm.ExecuteNonQuery();
        }



        //public void Loading()
        //    {
        //        HttpContext hc = HttpContext.Current;
        //        //创建一个页面居中的div
        //        hc.Response.Write("<div id='loading'style='position: absolute; height: 100px; text-align: center;z-index: 9999; left: 50%; top: 50%; margin-top: -50px; margin-left: -175px;'> ");
        //        hc.Response.Write("<br />页面正在加载中，请稍候<br /><br /> ");
        //        hc.Response.Write("<table border='0' cellpadding='0' cellspacing='0' style='background-image: url(images/Progress/plan-bg.gif);text-align: center; width: 300px;'> ");
        //        hc.Response.Write("<tr><td style='height: 20px; text-align: center'><marquee direction='right' scrollamount='30' width='290px'> <img height='10' src='images/Progress/plan-wait.gif' width='270' />");
        //        hc.Response.Write("</marquee></td></tr></table></div>");
        //        //hc.Response.Write("<script>mydiv.innerText = '';</script>");
        //        hc.Response.Write("<script type=text/javascript>");
        //        //最重要是这句了,重写文档的onreadystatechange事件,判断文档是否加载完毕
        //        hc.Response.Write("function document.onreadystatechange()");
        //        hc.Response.Write(@"{ try  
        //                                   {
        //                                    if (document.readyState == 'complete') 
        //                                    {
        //                                         delNode('loading');
        //                                        
        //                                    }
        //                                   }
        //                                 catch(e)
        //                                    {
        //                                        alert('页面加载失败');
        //                                    }
        //                                                        } 
        //
        //                            function delNode(nodeId)
        //                            {   
        //                                try
        //                                {   
        //                                      var div =document.getElementById(nodeId); 
        //                                      if(div !==null)
        //                                      {
        //                                          div.parentNode.removeChild(div);   
        //                                          div=null;    
        //                                          CollectGarbage(); 
        //                                      } 
        //                                }
        //                                catch(e)
        //                                {   
        //                                   alert('删除ID为'+nodeId+'的节点出现异常');
        //                                }   
        //                            }
        //
        //                            ");

        //        hc.Response.Write("</script>");
        //        hc.Response.Flush();
        //    }


        /// <summary>
        /// 通过文件流判断文件编码
        /// </summary>
        /// <param name="stream">文件流</param>
        /// <returns></returns>
        public static System.Text.Encoding GetFileEncode(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream, Encoding.Default);
            byte[] bb = br.ReadBytes(3);
            br.Close();

            //通过头的前三位判断文件的编码
            if (bb[0] >= 0xFF)
            {
                if (bb[0] == 0xEF && bb[1] == 0xBB && bb[2] == 0xBF)
                {
                    return Encoding.UTF8;
                }
                else if (bb[0] == 0xFE && bb[1] == 0xFF)
                {
                    return Encoding.BigEndianUnicode;
                }
                else if (bb[0] == 0xFF && bb[1] == 0xFE)
                {
                    return Encoding.Unicode;
                }
                else
                {
                    return Encoding.Default;
                }
            }
            else
            {
                return Encoding.Default;
            }
        }

        /// <summary>
        /// 数据转换
        /// </summary>
        /// <typeparam name="T">
        /// 数据类型（暂支持：int,string Guid,DateTime,Byte）,
        /// 转换失将讲返回（
        /// int---0,
        /// string---"", 
        /// Guid---Guid.Empty,
        /// DateTime---DateTime.MinValue,
        /// Byte---0
        /// Decimal---0）</typeparam>
        /// <param name="val">需转换的值</param>
        /// <returns></returns>
        public static T ConvertValue<T>(string val)
        {
            if (typeof(T).Name.ToLower() == "int32")
            {
                int value = 0;
                int.TryParse(val, out value);
                return (T)(object)value;
            }
            else if (typeof(T).Name.ToLower() == "string")
            {
                string value = val != null ? val : string.Empty;
                return (T)(object)value;
            }
            else if (typeof(T).Name.ToLower() == "guid")
            {
                Guid rGuid = Guid.Empty;
                if (val != null && val != string.Empty)
                {
                    try
                    {
                        rGuid = new Guid(val.Trim());
                    }
                    catch { }
                }
                return (T)(object)rGuid;
            }
            else if (typeof(T).Name.ToLower() == "datetime")
            {
                DateTime value = DateTime.MinValue;
                if (!DateTime.TryParse(val, out value))
                    value = DateTime.MinValue;
                return (T)(object)value;
            }
            else if (typeof(T).Name.ToLower() == "boolean")
            {
                bool value = false;
                if (!bool.TryParse(val, out value))
                {
                    int intValue = 0;
                    if (int.TryParse(val, out intValue))
                    {
                        if (intValue == 0) value = false;
                        else if (intValue == 1) value = true;
                    }
                }
                return (T)(object)value;
            }
            else if (typeof(T).Name.ToLower() == "byte")
            {
                byte value = 0;
                byte.TryParse(val, out value);
                return (T)(object)value;
            }
            else if (typeof(T).Name.ToLower() == "double")
            {
                double value = 0;
                double.TryParse(val, out value);
                return (T)(object)value;
            }
            else if (typeof(T).Name.ToLower() == "single")
            {
                float value = 0;
                float.TryParse(val, out value);
                return (T)(object)value;
            }
            else if (typeof(T).Name.ToLower() == "int64")
            {
                Int64 value = 0;
                Int64.TryParse(val, out value);
                return (T)(object)value;
            }
            else if (typeof(T).Name.ToLower() == "long")
            {
                long value = 0;
                long.TryParse(val, out value);
                return (T)(object)value;
            }
            else if (typeof(T).Name.ToLower() == "decimal")
            {
                decimal value = 0;
                if (val != null)
                    decimal.TryParse(val.ToString(), out value);
                return (T)(object)value;
            }
            else if (typeof(T).Name.ToLower() == "nullable`1")
            {
                string typeName = Nullable.GetUnderlyingType(typeof(T)).Name.ToLower();
                if (typeName == "datetime")
                {
                    DateTime value = DateTime.MinValue;
                    if (DateTime.TryParse(val, out value)) { return (T)(object)value; }
                }
                if (typeName == "int32")
                {
                    int value = 0;
                    if (int.TryParse(val, out value)) { return (T)(object)value; }
                }
                if (typeName == "int64")
                {
                    Int64 value = 0;
                    if (Int64.TryParse(val, out value)) { return (T)(object)value; }
                }
                if (typeName == "long")
                {
                    long value = 0;
                    if (long.TryParse(val, out value)) { return (T)(object)value; }
                }
                if (typeName == "double")
                {
                    double value = 0;
                    if (double.TryParse(val, out value)) { return (T)(object)value; }
                }
                if (typeName == "byte")
                {
                    byte value = 0;
                    if (byte.TryParse(val, out value)) { return (T)(object)value; }
                }
            }
            return default(T);
        }

        /// <summary>
        /// 输出信息
        /// </summary>
        /// <param name="msg">需要利用Response.Write输出的信息</param>
        public static void ResponseWrite(string msg)
        {
            HttpContext.Current.Response.Write(msg);
        }

        /// <summary>
        /// 获取小图Url
        /// </summary>
        /// <param name="picUrl"></param>
        /// <returns></returns>
        public static string FileNameAddFolder(string folder, object picUrl)
        {
            if (picUrl != null && picUrl.ToString().Trim() != "")
            {
                string path = Path.GetDirectoryName(picUrl.ToString());
                string name = Path.GetFileName(picUrl.ToString());
                string url = string.Format("{0}{1}{2}/{3}{4}",
                    HttpContext.Current.Request.Url.Authority,
                    HttpContext.Current.Request.ApplicationPath,
                    path, folder, name);
                url = url.Replace("//", "/");
                url = url.Replace(@"\", "/");
                url = "http://" + url;
                return url;

            }
            return string.Empty;
        }

        #region 根据 mime 类型，返回编码器
        private static System.Drawing.Imaging.ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            System.Drawing.Imaging.ImageCodecInfo result = null;

            //检索已安装的图像编码解码器的相关信息。
            System.Drawing.Imaging.ImageCodecInfo[] encoders =
                System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
            for (int i = 0; i < encoders.Length; i++)
            {
                if (encoders[i].MimeType == mimeType)
                {
                    result = encoders[i];
                    break;
                }
            }
            return result;
        }

        #endregion

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="oldpath">原图片地址</param>
        /// <param name="newpath">新图片地址</param>
        /// <param name="tWidth">缩略图的宽</param>
        /// <param name="tHeight">缩略图的高</param>
        public static void CreateMiniImage(string oldPath, string newPath, int tWidth, int tHeight)
        {
            try
            {
                oldPath = oldPath.ToLower();
                string fileName = Path.GetFileName(oldPath);

                Image image = Image.FromFile(oldPath);

                #region 等比缩放
                int imgWidth = image.Width;
                int imgHeight = image.Height;
                int newWidth = 0;
                int newHeight = 0;
                if (imgWidth > tWidth)
                {
                    newWidth = tWidth;
                    newHeight = tWidth * imgHeight / imgWidth;
                    if (newHeight > tHeight)
                    {
                        newWidth = tHeight * newWidth / newHeight;
                        newHeight = tHeight;
                    }
                }
                else if (imgHeight > tHeight)
                {
                    newHeight = tHeight;
                    newWidth = tHeight * imgWidth / imgHeight;
                    if (newWidth > tWidth)
                    {
                        newHeight = tWidth * newHeight / newWidth;
                        newWidth = tWidth;
                    }
                }
                else
                {
                    newWidth = imgWidth;
                    newHeight = imgHeight;
                }
                #endregion

                Bitmap b = new Bitmap(image, newWidth, newHeight);
                //释放image资源
                image.Dispose();

                #region 降低图片质量
                System.Drawing.Imaging.ImageCodecInfo encoder = GetEncoderInfo("image/jpeg");
                System.Drawing.Imaging.EncoderParameters encoderParams = new System.Drawing.Imaging.EncoderParameters(1);
                encoderParams.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)95);
                #endregion
                b.Save(newPath, encoder, encoderParams);
                //b.Save(newPath);
                b.Dispose();
            }
            catch
            {
            }
        }

        /// <summary>
        /// 设置请求不被缓存
        /// </summary>
        public static void SetRequestNoCache()
        {
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.ExpiresAbsolute = System.DateTime.Now.AddSeconds(-1);
            HttpContext.Current.Response.Expires = 0;
            HttpContext.Current.Response.CacheControl = "no-cache";
            HttpContext.Current.Response.AppendHeader("Pragma", "No-Cache");
        }

        /// <summary>
        /// 是否为图片文件
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        ///   <c>true</c> if [is image file] [the specified path]; otherwise, <c>false</c>.
        /// </returns>
        /// Author: zhangpan
        /// Date: 2013/2/16 15:39
        public static bool IsImageFile(string path)
        {
            if (string.IsNullOrEmpty(path)) { return false; }
            string extension = Path.GetExtension(path);
            if (extension == ".bmp" ||
                extension == ".gif" ||
                extension == ".jpg" ||
                extension == ".png" ||
                extension == ".jpeg" ||
                extension == ".ico" ||
                extension == ".tif")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool FileIsExists(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) { return false; }
            return File.Exists(filePath);
        }

        /// <summary>
        /// 文件夹是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool DirectoryIsExists(string path)
        {
            if (string.IsNullOrEmpty(path)) { return false; }
            return Directory.Exists(path);
        }

        #region 下载文件到本地
        /// <summary>
        /// 下载文件到本地
        /// </summary>
        /// <param name="fileUrl">需下载的文件的地址</param>
        /// <param name="savePath">本地保存路径</param>
        public static bool DownFile(string fileUrl, string savePath)
        {
            try
            {
                WebRequest request = WebRequest.Create(fileUrl);
                WebResponse response = request.GetResponse();
                return SaveBinaryFile(response, savePath);
            }
            catch { return false; }
        }

        /// <summary>
        /// 保存文件到磁盘
        /// </summary>
        /// <param name="response">文件流</param>
        /// <param name="response">保存的文件名</param>
        // 将二进制文件保存到磁盘
        private static bool SaveBinaryFile(WebResponse response, string fileName)
        {
            string path = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            bool Value = true;
            byte[] buffer = new byte[1024];
            try
            {
                if (System.IO.File.Exists(fileName))
                    System.IO.File.Delete(fileName);
                Stream outStream = System.IO.File.Create(fileName);
                Stream inStream = response.GetResponseStream();
                int l;
                do
                {
                    l = inStream.Read(buffer, 0, buffer.Length);
                    if (l > 0)
                        outStream.Write(buffer, 0, l);
                }
                while (l > 0);

                outStream.Close();
                inStream.Close();
                response.Close();
            }
            catch
            {
                Value = false;
            }
            return Value;
        }
        #endregion
    }

    #region "枚举类型"
    /// <summary>
    /// 删除文件路径类型
    /// </summary>
    public enum DeleteFilePathType
    {
        /// <summary>
        /// 物理路径
        /// </summary>
        PhysicsPath = 1,
        /// <summary>
        /// 虚拟路径
        /// </summary>
        DummyPath = 2,
        /// <summary>
        /// 当前目录
        /// </summary>
        NowDirectoryPath = 3
    }

    /// <summary>
    /// 获取方式
    /// </summary>
    public enum MethodType
    {
        /// <summary>
        /// Post方式
        /// </summary>
        Post = 1,
        /// <summary>
        /// Get方式
        /// </summary>
        Get = 2
    }

    /// <summary>
    /// 获取数据类型
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// 字符
        /// </summary>
        Str = 1,
        /// <summary>
        /// 日期
        /// </summary>
        Dat = 2,
        /// <summary>
        /// 整型
        /// </summary>
        Int = 3,
        /// <summary>
        /// 长整型
        /// </summary>
        Long = 4,
        /// <summary>
        /// 双精度小数
        /// </summary>
        Double = 5,
        /// <summary>
        /// 只限字符和数字
        /// </summary>
        CharAndNum = 6,
        /// <summary>
        /// 只限邮件地址
        /// </summary>
        Email = 7,
        /// <summary>
        /// 只限字符和数字和中文
        /// </summary>
        CharAndNumAndChinese = 8

    }
    #endregion
}