using System;
using ImageService.Logger.Model;

namespace ImageService.Logger
{
    public interface ILoggingService
    {
        event EventHandler<MessageRecievedEventArgs> MsgRecievedEvent;
        void Log(string message, MessageTypeEnum type);
    }
}
