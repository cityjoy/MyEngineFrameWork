using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace Engine.Infrastructure.Utils
{
     public class SecurityHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly string _KEY_64 = "TMyh!llj";
        /// <summary>
        /// 
        /// </summary>
        public static readonly string _IV_64 = "TMyhCode";

        /// <summary>
        /// 加密算法
        /// </summary>
        /// <param name="data"></param>
        public static string DESCryptoEncode(string data)
        {
            return DESCryptoEncode(_KEY_64, _IV_64, data);
        }
        /// <summary>
        /// 加密算法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="data"></param>
        public static string DESCryptoEncode(string key, string iv, string data)
        {
            byte[] byKey = ASCIIEncoding.ASCII.GetBytes(key);
            byte[] byIV = ASCIIEncoding.ASCII.GetBytes(iv);

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            int i = cryptoProvider.KeySize;
            MemoryStream ms = new MemoryStream();
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);

            using (StreamWriter sw = new StreamWriter(cst))
            {
                sw.Write(data);
                sw.Flush();
                cst.FlushFinalBlock();
                sw.Flush();
                string result = Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
                return result;
            }
        }

        /// <summary>
        /// 解密算法
        /// </summary>
        /// <param name="data"></param>
        public static string DESCryptoDecode(string data)
        {
            return DESCryptoDecode(_KEY_64, _IV_64, data);
        }
        /// <summary>
        /// 解密算法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="data"></param>
        public static string DESCryptoDecode(string key, string iv, string data)
        {
            byte[] byKey = ASCIIEncoding.ASCII.GetBytes(key);
            byte[] byIV = ASCIIEncoding.ASCII.GetBytes(iv);

            byte[] byEnc = Convert.FromBase64String(data);

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream(byEnc);
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
            using (StreamReader sr = new StreamReader(cst))
            {
                return sr.ReadToEnd();
            }
        }







        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="text">要加密的字符串</param>
        public static string MD5(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "00000000000000000000000000000000"; ;
            return FormsAuthentication.HashPasswordForStoringInConfigFile(text, "MD5");
        }

        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="text">要加密的字符串</param>
        public static string SHA1(string text)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(text, "SHA1");
        }

    }
}
