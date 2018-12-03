using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Infrastructure.Utils.RabbitMQ
{
    public class MyMessage
    {
        public string MessageID { get; set; }
        
        public string MessageTitle { get; set; }

        public string MessageBody { get; set; }

        public string MessageRouter { get; set; }
    }
}
