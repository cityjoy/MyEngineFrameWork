using System;
using System.Collections.Generic;
using System.Text;
using Engine.Infrastructure.Utils;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// 验证码助手
    /// </summary>
    public class ValidateCoder
    {


        /// <summary>
        /// 存储验证码答案
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <param name="codeResult"></param>
        public static void SetValidateCodeResult(string sessionId, string type, string key, string codeResult)
        {
            string sessionKey = string.Concat("VALIDATE_CODE_", type.ToString(), "_", key);

            WebSession session = new WebSession(sessionId);
            session.Set<string>(sessionKey, codeResult);
           // LogHelper.WriteLog(type.ToString() + "会话"+sessionId+"验证码存入缓存"+sessionKey +":"+ codeResult);
            
        }

        /// <summary>
        /// 获取验证码答案
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValidateCodeResult(string sessionId, string type, string key)
        {
            string sessionKey = string.Concat("VALIDATE_CODE_", type.ToString(), "_", key);

            WebSession session = new WebSession(sessionId);
            string codeResult = session.Get<string>(sessionKey);
            //LogHelper.WriteLog(type.ToString() + "获取会话" + sessionId + "存入缓存的验证码" + sessionKey + ":" + codeResult);

            return codeResult;
        }

        /// <summary>
        /// 清除验证码答案
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="type"></param>
        /// <param name="key"></param>
        public static void ClearValidateCodeResult(string sessionId, string type, string key)
        {
            string sessionKey = string.Concat("VALIDATE_CODE_", type.ToString(), "_", key);

            WebSession session = new WebSession(sessionId);
            session.Remove(sessionKey);
        }

        /// <summary>
        /// 创建验证码图片文件流
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="type">类型</param>
        /// <param name="key">键值，用来保存验证码答案</param>
        /// <param name="validationResult"></param>
        /// <returns></returns>
      
        public static byte[] CreateCodeImage(string sessionId, string type, string key, out string validationResult)
        {
            return CreateCodeImage(sessionId, type, key, out validationResult, null);
        }
        /// <summary>
        /// 创建验证码图片文件流
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="type">类型</param>
        /// <param name="key">键值，用来保存验证码答案</param>
        /// <param name="validationResult"></param>
        /// <param name="codeTypes">在指定类型里面随机</param>
        /// <returns></returns>
      
        public static byte[] CreateCodeImage(string sessionId, string type, string key, out string validationResult, params int[] codeTypes)
        {
            ValidateCodeBase validateCode;
            byte[] bytes;

            Random rand = new Random();
            int randNum;
            if (codeTypes != null)
            {
                int cc = codeTypes.Length;
                int posRand = rand.Next(0, cc - 1);
                randNum = codeTypes[posRand];
            }
            else
            {
                randNum = rand.Next(1, 12);
            }

            switch (randNum)
            {
                case 1:
                default:
                    validateCode = ValidateCodeManager.CreateValidateCode<ValidateCode001>();
                    bytes = validateCode.CreateImage(out validationResult);

                    break;
                case 2:
                    validateCode = ValidateCodeManager.CreateValidateCode<ValidateCode002>();
                    bytes = validateCode.CreateImage(out validationResult);

                    break;
                case 3:
                    validateCode = ValidateCodeManager.CreateValidateCode<ValidateCode003>();
                    bytes = validateCode.CreateImage(out validationResult);

                    break;
                case 4:
                    validateCode = ValidateCodeManager.CreateValidateCode<ValidateCode004>();
                    bytes = validateCode.CreateImage(out validationResult);

                    break;
                case 5:
                    validateCode = ValidateCodeManager.CreateValidateCode<ValidateCode005>();
                    bytes = validateCode.CreateImage(out validationResult);

                    break;
                case 6:
                    validateCode = ValidateCodeManager.CreateValidateCode<ValidateCode006>();
                    bytes = validateCode.CreateImage(out validationResult);

                    break;
                case 7:
                    validateCode = ValidateCodeManager.CreateValidateCode<ValidateCode007>();
                    bytes = validateCode.CreateImage(out validationResult);

                    break;
                case 8:
                    validateCode = ValidateCodeManager.CreateValidateCode<ValidateCode008>();
                    bytes = validateCode.CreateImage(out validationResult);

                    break;
                case 9:
                    validateCode = ValidateCodeManager.CreateValidateCode<ValidateCode009>();
                    bytes = validateCode.CreateImage(out validationResult);

                    break;
                case 10:
                    validateCode = ValidateCodeManager.CreateValidateCode<ValidateCode010>();
                    bytes = validateCode.CreateImage(out validationResult);

                    break;
                case 11:
                    validateCode = ValidateCodeManager.CreateValidateCode<ValidateCode011>();
                    bytes = validateCode.CreateImage(out validationResult);

                    break;
                case 12:
                    validateCode = ValidateCodeManager.CreateValidateCode<ValidateCode012>();
                    bytes = validateCode.CreateImage(out validationResult);

                    break;
            }

            SetValidateCodeResult(sessionId, type, key, validationResult);

            return bytes;
        }

        /// <summary>
        /// 检查验证码
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <param name="input"></param>
        /// <returns>1:正确,-1:验证码错误,-2:验证码错误或已经过期</returns>
        public static int CheckValidateCode(string sessionId, string type, string key, string input)
        {
            string codeResult = GetValidateCodeResult(sessionId, type, key);

            if (codeResult == null)
            {
                //验证码错误或已经过期
                return -2;
            }

            if (string.Compare(codeResult, input, true) == 0)
            {
                return 1;
            }
            else
            {
                //验证码错误
                return -1;
            }
        }

    }
}
