using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// 短信处理器
    /// </summary>
    public class Sms
    {

        /// <summary>
        /// 发送手机短信
        /// </summary>
        /// <param name="mobiles">手机号码，多个以英文逗号隔开</param>
        /// <param name="text"></param>
        /// <param name="sendTime">指定时间发送，为空则立即发送</param>
        /// <returns>0:失败,>0:成功</returns>
        public static XMKarlosSmsResult SendMessage(string mobiles, string text, DateTime? sendTime)
        {
            if (text.LastIndexOf("【") < 0 || text.LastIndexOf("】") < 0)
            {
                text = string.Concat(text, "【51选校网】");
            }

            try
            {
                string apiUrl = "http://121.199.50.122:8888/sms.aspx";

                XWebRequest webRequest = new XWebRequest(apiUrl);
                webRequest.Encoding = Encoding.UTF8;
                webRequest.Method = "POST";
                webRequest.AddPostData("userid", "1115");
                webRequest.AddPostData("account", "XMCYBD");
                webRequest.AddPostData("password", "cybd2016cybd2016");
                webRequest.AddPostData("mobile", mobiles);
                webRequest.AddPostData("content", text);
                if (sendTime == null)
                {
                    webRequest.AddPostData("sendTime", ""); //为空则立即发送
                }
                else
                {
                    webRequest.AddPostData("sendTime", sendTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                webRequest.AddPostData("action", "send");
                webRequest.AddPostData("extno", "");

                string xmlResult = webRequest.ApplyForm();
                XMKarlosSmsResult smsResult = new XMKarlosSmsResult(xmlResult);

                return smsResult;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex);
                return null;
            }
        }

        /// <summary>
        /// 获取当前会话的手机动态码短信
        /// </summary>
        /// <param name="smsType"></param>
        /// <returns></returns>
        public static string GetCodeMessage(string sessionId, string smsType, long phone)
        {
            string skey = "SEND_MOBILE_CODE_MESSAGE_" + smsType + "_" + phone;
            WebSession session = new WebSession(sessionId);
            string code = session.Get<string>(skey);
            return code;
        }

        /// <summary>
        /// 清除当前会话的手机动态码短信
        /// </summary>
        /// <param name="smsType"></param>
        public static void ClearCodeMessage(string sessionId, string smsType, long phone)
        {
            string skey = "SEND_MOBILE_CODE_MESSAGE_" + smsType + "_" + phone;
            WebSession session = new WebSession(sessionId);
            session.Remove(skey);
        }


    }
}
