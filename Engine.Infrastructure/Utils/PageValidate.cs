
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace  Engine.Infrastructure.Utils
{
    public class PageValidate
    {
        #region 构造函数
        public PageValidate()
        {

        }
        #endregion

        #region 正则表达式

        private static Regex RegPhone = new Regex("^1[34578]\\d{9}$"); 

        private static Regex RegNumber = new Regex("^[0-9]+$"); 

        private static Regex RegNumberSign = new Regex("^[+-]?[0-9]+$"); 

        private static Regex RegDecimal = new Regex("^[0-9]+[.]?[0-9]+$"); 

        private static Regex RegDecimalSign = new Regex("^[+-]?[0-9]+[.]?[0-9]+$"); //等价于^[+-]?\d+[.]?\d+$ 

        private static Regex RegEmail = new Regex("^[\\w-]+@[\\w-]+\\.(com|net|org|edu|mil|tv|biz|info)$");//w 英文字母或数字的字符串，和 [a-zA-Z0-9] 语法一样 

        private static Regex RegCHZN = new Regex("[\u4e00-\u9fa5]");

        private static Regex RegENStr = new Regex("^([a-zA-Z]*?)((?:[0-9]+(?:[0-9]|[a-zA-Z])*)*)$");

        #endregion

        #region 检查是否为英文/数字结合的字符串
        /// <summary>
        /// 检查是否为英文/数字结合的字符串
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static bool IsEnStr(string inputData)
        {
            Match m = RegENStr.Match(inputData);
            return m.Success;
        }
        #endregion

        #region 数字字符串检查
        /// <summary>
        /// 数字字符串检查  
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
    
        public static bool IsPhone(string inputData) 
        { 
            Match m = RegPhone.Match(inputData); 
            return m.Success; 
        }
        #endregion

        #region 检查Request查询字符串的键值，是否是数字，最大长度限制
        /// <summary> 
        /// 检查Request查询字符串的键值，是否是数字，最大长度限制 
        /// </summary> 
        /// <param name="req">Request</param> 
        /// <param name="inputKey">Request的键值</param> 
        /// <param name="maxLen">最大长度</param> 
        /// <returns>返回Request查询字符串</returns> 
        public static string FetchInputDigit(HttpRequest req, string inputKey, int maxLen) 
        { 
            string retVal = string.Empty; 
            if(inputKey != null && inputKey != string.Empty) 
            { 
                retVal = req.QueryString[inputKey]; 
                if(null == retVal) 
                    retVal = req.Form[inputKey]; 
                if(null != retVal) 
                { 
                    retVal = SqlText(retVal, maxLen); 
                    if(!IsNumber(retVal)) 
                        retVal = string.Empty; 
                } 
            } 
            if(retVal == null) 
                retVal = string.Empty; 
            return retVal; 
        }
        #endregion

        #region 是否数字字符串
        /// <summary> 
        /// 是否数字字符串 
        /// </summary> 
        /// <param name="inputData">输入字符串</param> 
        /// <returns></returns> 
        public static bool IsNumber(string inputData) 
        {
            Match m = RegNumber.Match(inputData); 
            return m.Success; 
        }
        #endregion 

        #region 是否数字字符串 可带正负号
        /// <summary> 
        /// 是否数字字符串 可带正负号 
        /// </summary> 
        /// <param name="inputData">输入字符串</param> 
        /// <returns></returns> 
        public static bool IsNumberSign(string inputData) 
        { 
            Match m = RegNumberSign.Match(inputData); 
            return m.Success; 
        }
        #endregion

        #region 是否是浮点数
        /// <summary> 
        /// 是否是浮点数 
        /// </summary> 
        /// <param name="inputData">输入字符串</param> 
        /// <returns></returns> 
        public static bool IsDecimal(string inputData) 
        { 
            Match m = RegDecimal.Match(inputData); 
            return m.Success; 
        }
        #endregion

        #region 是否是浮点数 可带正负号
        /// <summary> 
        /// 是否是浮点数 可带正负号 
        /// </summary> 
        /// <param name="inputData">输入字符串</param> 
        /// <returns></returns> 
        public static bool IsDecimalSign(string inputData) 
        { 
            Match m = RegDecimalSign.Match(inputData); 
            return m.Success; 
        }
        #endregion

        #region 检测是否有中文字符
        /// <summary> 
        /// 检测是否有中文字符 
        /// </summary> 
        /// <param name="inputData"></param> 
        /// <returns></returns> 
        public static bool IsHasCHZN(string inputData) 
        { 
            Match m = RegCHZN.Match(inputData); 
            return m.Success; 
        }
        #endregion

        #region 检测邮件地址
        /// <summary> 
        /// 检测邮件地址
        /// </summary> 
        /// <param name="inputData">输入字符串</param> 
        /// <returns></returns> 
        public static bool IsEmail(string inputData) 
        { 
            Match m = RegEmail.Match(inputData); 
            return m.Success; 
        }
        #endregion

        #region 检查字符串最大长度，返回指定长度的串
        /// <summary> 
        /// 检查字符串最大长度，返回指定长度的串 
        /// </summary> 
        /// <param name="sqlInput">输入字符串</param> 
        /// <param name="maxLength">最大长度</param> 
        /// <returns></returns>             
        public static string SqlText(string sqlInput, int maxLength) 
        {             
            if(sqlInput != null && sqlInput != string.Empty) 
            { 
                sqlInput = sqlInput.Trim();                             
                if(sqlInput.Length > maxLength)//按最大长度截取字符串 
                    sqlInput = sqlInput.Substring(0, maxLength); 
            } 
            return sqlInput; 
        }
        #endregion

        #region 字符串编码
        /// <summary> 
        /// 字符串编码 
        /// </summary> 
        /// <param name="inputData"></param> 
        /// <returns></returns> 
        public static string HtmlEncode(string inputData) 
        { 
            return HttpUtility.HtmlEncode(inputData); 
        }
        #endregion

        #region 字符串清理
        /// <summary>
        /// 字符串清理
        /// </summary>
        /// <param name="inputString">字符串</param>
        /// <param name="maxLength">字符串限制的长度</param>
        /// <returns></returns>
 
        public static string InputText(string inputString, int maxLength) 
        {             
            StringBuilder retVal = new StringBuilder(); 
            // 检查是否为空 
            if ((inputString != null) && (inputString != String.Empty)) 
            { 
                inputString = inputString.Trim(); 
                //检查长度 
                if (inputString.Length > maxLength) 
                    inputString = inputString.Substring(0, maxLength);                  
                //替换危险字符 
                for (int i = 0; i < inputString.Length; i++) 
                { 
                    switch (inputString[i]) 
                    { 
                        case '"': 
                            retVal.Append("&quot;"); 
                            break; 
                        case '<': 
                            retVal.Append("&lt;"); 
                            break; 
                        case '>': 
                            retVal.Append("&gt;"); 
                            break; 
                        default: 
                            retVal.Append(inputString[i]); 
                            break; 
                    } 
                }                 
                retVal.Replace("'", " ");// 替换单引号 
            } 
            return retVal.ToString(); 
        }
        #endregion

        #region 转换成 HTML code
        /// <summary> 
        /// 转换成 HTML code 
        /// </summary> 
        /// <param name="str">string</param> 
        /// <returns>string</returns> 
        public static string Encode(string str) 
        {             
            str = str.Replace("&","&amp;"); 
            str = str.Replace("'","''"); 
            str = str.Replace("\"","&quot;"); 
            str = str.Replace(" ","&nbsp;"); 
            str = str.Replace("<","&lt;"); 
            str = str.Replace(">","&gt;"); 
            str = str.Replace("\n","<br>"); 
            return str; 
        }
        #endregion

        #region 解析html成 普通文本
        /// <summary> 
        ///解析html成 普通文本 
        /// </summary> 
        /// <param name="str">string</param> 
        /// <returns>string</returns> 
        public static string Decode(string str) 
        {             
            str = str.Replace("<br>","\n"); 
            str = str.Replace("&gt;",">"); 
            str = str.Replace("&lt;","<"); 
            str = str.Replace("&nbsp;"," "); 
            str = str.Replace("&quot;","\""); 
            return str; 
        }
        #endregion

        #region 检测sql语句是否包含敏感字符
        /// <summary>
        /// 检测sql语句是否包含敏感字符
        /// </summary>
        /// <param name="sqlText">sql字符串</param>
        /// <returns></returns>
        public static string SqlTextClear(string sqlText) 
        { 
            if (sqlText == null) 
            { 
                return null; 
            } 

            if (sqlText == "") 
            { 
                return ""; 
            } 
            sqlText = sqlText.Replace(",", "");//去除, 
            sqlText = sqlText.Replace("<", "");//去除< 
            sqlText = sqlText.Replace(">", "");//去除> 
            sqlText = sqlText.Replace("--", "");//去除-- 
            sqlText = sqlText.Replace("'", "");//去除' 
            sqlText = sqlText.Replace("\"", "");//去除" 
            sqlText = sqlText.Replace("=", "");//去除= 
            sqlText = sqlText.Replace("%", "");//去除% 
            sqlText = sqlText.Replace(" ", "");//去除空格 
            return sqlText; 
        }
        #endregion

        #region 是否由特定字符组成
        /// <summary>
        /// 是否由特定字符组成 
        /// </summary>
        /// <param name="strInput">检测的字符串</param>
        /// <returns></returns>
        public static bool isContainSameChar(string strInput) 
        { 
            string charInput = string.Empty; 
            if (!string.IsNullOrEmpty(strInput)) 
            { 
                charInput = strInput.Substring(0, 1); 
            } 
            return isContainSameChar(strInput, charInput, strInput.Length); 
        }
        /// <summary>
        /// 是否由特定字符组成 
        /// </summary>
        /// <param name="strInput">检测的字符串</param>
        /// <returns></returns>
        public static bool isContainSameChar(string strInput, string charInput, int lenInput) 
        { 
            if (string.IsNullOrEmpty(charInput)) 
            { 
                return false; 
            } 
            else 
            { 
                Regex RegNumber = new Regex(string.Format("^([{0}])+$", charInput)); 
                //Regex RegNumber = new Regex(string.Format("^([{0}]{{1}})+$", charInput,lenInput)); 
                Match m = RegNumber.Match(strInput); 
                return m.Success; 
            } 
        }
        #endregion

        #region 检查输入的参数是不是某些定义好的特殊字符：这个方法目前用于密码输入的安全检查
        /// <summary>
        /// 检查输入的参数是不是某些定义好的特殊字符：这个方法目前用于密码输入的安全检查 
        /// </summary>
        /// <param name="strInput">检测的字符串</param>
        /// <returns></returns>
        public static bool isContainSpecChar(string strInput) 
        { 
            string[] list = new string[] { "123456", "654321" }; 
            bool result = new bool(); 
            for (int i = 0; i < list.Length; i++) 
            { 
                if (strInput == list[i]) 
                { 
                    result = true; 
                    break; 
                } 
            } 
            return result;
        }
        #endregion
    }
}
