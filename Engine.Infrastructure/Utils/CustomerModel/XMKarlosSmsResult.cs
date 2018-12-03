using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class XMKarlosSmsResult
    {

        private XMKarlosSmsResult() { }

        public XMKarlosSmsResult(string xml)
        {
            this.XmlContent = xml;
            LoadFromXml(xml);
        }

        /// <summary>
        /// Gets the content of the XML.
        /// </summary>
        public string XmlContent
        {
            get;
            private set;
        }

        /// <summary>
        /// 返回状态值：成功返回Success 失败返回：Faild
        /// </summary>
        public string Status
        {
            get;
            set;
        }

        /// <summary>
        /// 返回信息
        /// ok: 提交成功
        /// 用户名或密码不能为空: 提交的用户名或密码为空
        /// 发送内容包含sql注入字符: 包含sql注入字符
        /// 用户名或密码错误: 表示用户名或密码错误
        /// 短信号码不能为空: 提交的被叫号码为空
        /// 短信内容不能为空: 发送内容为空
        /// 
        /// </summary>
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// 返回余额
        /// </summary>
        public decimal RemainPoint
        {
            get;
            set;
        }

        /// <summary>
        /// 返回本次任务的序列ID
        /// </summary>
        public string TaskID
        {
            get;
            set;
        }

        /// <summary>
        /// 成功短信数：当成功后返回提交成功短信数
        /// </summary>
        public int SuccessCounts
        {
            get;
            set;
        }

        /// <summary>
        /// 从XML加载数据
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public void LoadFromXml(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlElement rootDoc = doc.DocumentElement;
            XmlNode statusNode = rootDoc.SelectSingleNode("returnstatus");
            XmlNode messageNode = rootDoc.SelectSingleNode("message");
            XmlNode remainpointNode = rootDoc.SelectSingleNode("remainpoint");
            XmlNode taskIDNode = rootDoc.SelectSingleNode("taskID");
            XmlNode successCountsNode = rootDoc.SelectSingleNode("successCounts");

            if (statusNode != null)
            {
                this.Status = statusNode.InnerText.Trim();
            }
            if (messageNode != null)
            {
                this.Message = messageNode.InnerText.Trim();
            }
            if (remainpointNode != null)
            {
                this.RemainPoint = ConvertHelper.ToDecimal(messageNode.InnerText.Trim(), -1);
            }
            if (taskIDNode != null)
            {
                this.TaskID = taskIDNode.InnerText.Trim();
            }
            if (successCountsNode != null)
            {
                this.SuccessCounts = ConvertHelper.ToInt32(successCountsNode.InnerText.Trim(), -1);
            }

        }

    }
}
