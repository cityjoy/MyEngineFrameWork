using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// 字符操作
    /// </summary>
    public class StringOperationHelper
    {
        /// <summary>
        /// 截取字符串长度
        /// </summary>
        /// <param name="str">要截取的字符串</param>
        /// <param name="length">要截取的长度</param>
        /// <returns></returns>
        public static string StringSubLength(string str,int length)
        {
            if (str.Length > length)
            {
                str = str.Substring(0, length);
            }
            return str;
        }
        /// <summary>
        /// 文件管理系统的授权验证
        /// </summary>
        /// <param name="accredit">授权字符串</param>
        /// <param name="type">加密类型</param>
        /// <returns></returns>
        public static bool CheckFileStoreAccredit(string accredit, string type)
        {
            bool accreditResult = false;
            try
            {
                accredit = accredit.Replace("捌", "+");
                switch (type)
                {
                    case "ASCII":
                        accredit = StringOperationHelper.ASCIIDecode(accredit);
                        if (Convert.ToDateTime(accredit) > DateTime.Now)
                        {
                            accreditResult = true;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {

            }
            
            return accreditResult;
        }

        #region 加密方法1
        /// <summary>
        /// 加密算法1
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns></returns>
        public static string Encode(string str)
        {
            string htext = "";

            for (int i = 0; i < str.Length; i++)
            {
                htext = htext + (char)(str[i] + 10 - 1 * 2);
            }
            return htext;
        }
        /// <summary>
        /// 解密算法1
        /// </summary>
        /// <param name="str">要解密的字符串</param>
        /// <returns></returns>
        public static string Decode(string str)
        {
            string dtext = "";

            for (int i = 0; i < str.Length; i++)
            {
                dtext = dtext + (char)(str[i] - 10 + 1 * 2);
            }
            return dtext;
        }
        #endregion

        #region 加密方法2
        //Ascii码加/解密自定的义字符串
        private const string KEY_64 = "51CArEeR";//注意了，是8个字符，64位
        private const string IV_64 = "51CArEeR";
        /// <summary>
        /// ASCII码加,（只支持8个字节的密钥）
        /// </summary>
        /// <param name="data">要加密的字符串</param>
        /// <returns></returns>
        public static string ASCIIEncode(string data)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.Unicode.GetBytes(data);
            //建立加密对象的密钥和偏移量 
            des.Key = ASCIIEncoding.ASCII.GetBytes(KEY_64);
            des.IV = ASCIIEncoding.ASCII.GetBytes(IV_64);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();

        }
        /// <summary>
        /// ASCII码解密
        /// </summary>
        /// <param name="data">要解密的字符串</param>
        /// <returns></returns>
        public static string ASCIIDecode(string data)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();

                byte[] inputByteArray = new byte[data.Length / 2];
                for (int x = 0; x < data.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(data.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }

                //建立加密对象的密钥和偏移量，此值重要，不能修改  
                des.Key = ASCIIEncoding.ASCII.GetBytes(KEY_64);
                des.IV = ASCIIEncoding.ASCII.GetBytes(IV_64);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                //建立StringBuild对象，CreateDecrypt使用的是流对象，必须把解密后的文本变成流对象  
                StringBuilder ret = new StringBuilder();
                string result = System.Text.Encoding.Unicode.GetString(ms.ToArray());
                return result;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region 数组转换成字符串
        /// <summary>
        /// 数组转换成字符串(排序)
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="arrayStr">数据</param>
        /// <param name="splitStr">数据分隔符 比如 ,</param>
        /// <returns></returns>
        public static string ArrayToStr(string arrayStr, char splitStr)
        {
            string result = string.Empty;
            string[] ids = arrayStr.Split(splitStr).OrderBy(m => m).ToArray();
            foreach (var id in ids)
            {
                result += id + ",";
            }
            if (!string.IsNullOrWhiteSpace(result))
            {
                result = result.Remove(result.Length - 1);
            }
            return result;
        }
        #endregion
    }
}
