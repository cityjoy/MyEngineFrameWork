using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using EasyNetQ;

namespace Engine.Infrastructure.Utils.RabbitMQ
{
    public class MQHelper
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        public static void Publish(MyMessage msg, string connString)
        {
            //// 创建消息bus
            IBus bus = BusBuilder.CreateMessageBus(connString);

            try
            {
                bus.Publish(msg, x => x.WithTopic(msg.MessageRouter));
            }
            catch (EasyNetQException ex)
            {
                LogHelper.WriteLog("发送消息异常："+ex.ToString());
            }

            bus.Dispose();//与数据库connection类似，使用后记得销毁bus对象
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="msg"></param>
        public static void Subscribe(string connString,MyMessage msg, IProcessMessage ipro)
        {
            //// 创建消息bus
            IBus bus = BusBuilder.CreateMessageBus(connString);

            try
            {
                bus.Subscribe<MyMessage>(msg.MessageRouter, message => ipro.ProcessMsg(message), x => x.WithTopic(msg.MessageRouter));
            }
            catch (EasyNetQException ex)
            {
                LogHelper.WriteLog("接收消息异常：" + ex.ToString());
            }
        }
     
    }
}
