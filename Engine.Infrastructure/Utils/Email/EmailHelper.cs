using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// EmailHelper
    /// </summary>
 
    public class EmailHelper
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="from"></param>
        /// <param name="senderName"></param>
        /// <param name="to"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool SendEmail(string host, int port, string account, string password, string from, string senderName, string to, string title, string content)
        {
            return SendEmail(host, port, account, password, from, senderName, to, title, content, MailPriority.Normal, null);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="host">邮件服务主机</param>
        /// <param name="port">邮件服务端口，小于0则使用默认</param>
        /// <param name="account">邮件服务账号</param>
        /// <param name="password">邮件服务密码</param>
        /// <param name="from">发送者邮箱地址</param>
        /// <param name="senderName"></param>
        /// <param name="to">接收者邮箱地址</param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="priority">优先级</param>
        /// <param name="attachFilePaths">附件文件路径集</param>
        /// <returns></returns>
        public static bool SendEmail(string host, int port, string account, string password, string from,  string senderName, string to, string title, string content, MailPriority priority, params string[] attachFilePaths)
        {
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式
            smtpClient.Host = host;//指定SMTP服务器
            if (port > 0)
            {
                smtpClient.Port = port;
            }
            else
            {
                smtpClient.Port = 25;
            }
            smtpClient.Credentials = new NetworkCredential(account, password);//用户名和密码

            MailAddress fromAddress = new MailAddress(from, senderName, Encoding.UTF8);
            MailAddress toAddress = new MailAddress(to);
            MailMessage mailMessage = new MailMessage(fromAddress, toAddress);
            mailMessage.Subject = title;//主题
            mailMessage.SubjectEncoding = Encoding.UTF8;//标题编码
            mailMessage.Body = content;//内容
            mailMessage.BodyEncoding = Encoding.UTF8;//正文编码
            mailMessage.IsBodyHtml = true;//设置为HTML格式
            mailMessage.Priority = priority;//优先级
            //if (!string.IsNullOrEmpty(replyTo))
            //{
            //    MailAddress replyToMailAddress = new MailAddress(replyTo);
            //    mailMessage.ReplyTo = replyToMailAddress;
            //}

            if (attachFilePaths != null)
            {
                foreach (string attachFilePath in attachFilePaths)
                {
                    Attachment attachItem = new Attachment(attachFilePath);
                    mailMessage.Attachments.Add(attachItem);
                }
            }

            try
            {
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex);
                return false;
            }
        }

    }
}
