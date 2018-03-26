
using ImageService.Logging.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Logging
{
    public class LoggingService : ILoggingService
    {
        public event EventHandler<MessageRecievedEventArgs> m_msg_rcv_event;
        public void Log(string message, MessageTypeEnum type)
        {
            MessageRecievedEventArgs msg_rcv_args = new MessageRecievedEventArgs
            {
                Message = message,
                Status = type
            };

            m_msg_rcv_event?.Invoke(this, msg_rcv_args);
        }
    }
}
