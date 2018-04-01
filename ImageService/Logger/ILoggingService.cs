using ImageService.Logger.Model;
using System;

namespace ImageService.Logger
{
    public interface ILoggingService
    {
        event EventHandler<MessageRecievedEventArgs> MsgRecievedEvent;
        void Log(string message, MessageTypeEnum type);
    }
}
