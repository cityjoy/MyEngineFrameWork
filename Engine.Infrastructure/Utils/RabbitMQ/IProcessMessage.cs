using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Infrastructure.Utils.RabbitMQ
{
    public interface IProcessMessage
    {
        void ProcessMsg(MyMessage msg);
    }
}
