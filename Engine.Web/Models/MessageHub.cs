using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Engine.Web.Models
{
    /// <summary>
    /// SignalR 消息
    /// </summary>
    public class MessageHub : Hub
    {
        public string Send(int userId,string carrerSessionId)
        {
            string result = "";
            return result;
        }
    }
}