using System;

namespace ImageService.Communication
{
    public interface ITcpClientHandler
    {
        event EventHandler GuiClientClosed;
        void HandleClient();
        void Write(string s);
    }
}