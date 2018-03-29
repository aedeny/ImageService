
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
        public event EventHandler<MessageRecievedEventArgs> MsgRecievedEvent;
        public void Log(string message, MessageTypeEnum type)
        {
            MessageRecievedEventArgs msgRecievedArgs = new MessageRecievedEventArgs
            {
                Message = message,
                Status = type
            };
            MsgRecievedEvent?.Invoke(this, msgRecievedArgs);
        }
    }
}
