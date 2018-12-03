using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web.Mail;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// SSLEmailHelper使用465端口
    /// </summary>

    public class SSLEmailHelper
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


            MailMessage mmsg = new MailMessage();
            //验证  
            mmsg.Subject = title;// "zhuti1";//邮件主题

            mmsg.BodyFormat = MailFormat.Html;
            mmsg.Body = content;//邮件正文
            mmsg.BodyEncoding = Encoding.UTF8;//正文编码
            mmsg.Priority = MailPriority.High;//优先级
            mmsg.From = from;//发件者邮箱地址
            mmsg.To = to;//收件人收箱地址
            mmsg.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", "1");
            //登陆名  
            mmsg.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", account);
            //登陆密码  
            mmsg.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", password);
            mmsg.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserverport", 465);//端口 
            mmsg.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpusessl", "true");
            System.Web.Mail.SmtpMail.SmtpServer = host ;    //企业账号用smtp.exmail.qq.com 
            if (attachFilePaths != null)
            {
                foreach (string attachFilePath in attachFilePaths)
                {

                    MailAttachment oAttch = new MailAttachment(attachFilePath, MailEncoding.Base64);

                    mmsg.Attachments.Add(oAttch);
                }
            }

            try
            {
                SmtpMail.Send(mmsg);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex);
                LogHelper.WriteLog("发送邮件报错:"+ex.Message.ToString());
                return false;
            }
        }

    }
}
