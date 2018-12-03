using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// 邮件处理器
    /// </summary>

    public class Email
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="emailType"></param>
        /// <param name="email"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool SendEmail(string emailType, string email, string title, string content)
        {
            bool result = false;

            try
            {
                bool sendResult = EmailHelper.SendEmail(Constants.EMAIL_SERV_HOST, Constants.EMAIL_SERV_PORT, Constants.EMAIL_SERV_ACCOUNT, Constants.EMAIL_SERV_PASSWORD, Constants.EMAIL_FROM_ACCOUNT, Constants.EMAIL_SENDER_NAME, email, title, content);

                result = sendResult;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex);

                result = false;
            }

            return result;
        }

        /// <summary>
        /// 获取当前会话的邮件动态码
        /// </summary>
        /// <param name="emailType"></param>
        /// <returns></returns>
        public static string GetEmailCode(string sessionId, string emailType, string email)
        {
            string skey = "SEND_EMAIL_CODE_MESSAGE_" + emailType + "_" + email;

            WebSession session = new WebSession(sessionId);
            string code = session.Get<string>(skey);
            return code;
        }

        /// <summary>
        /// 清除当前会话的邮件动态码
        /// </summary>
        /// <param name="emailType"></param>
        public static void ClearEmailCode(string sessionId, string emailType, string email)
        {
            string skey = "SEND_EMAIL_CODE_MESSAGE_" + emailType + "_" + email;

            WebSession session = new WebSession(sessionId);
            session.Remove(skey);
        }



    }
}
