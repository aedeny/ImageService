using System;
using ImageService.Logger.Model;

namespace ImageService.Logger
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
