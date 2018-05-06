using System.Net.Sockets;

namespace ImageService.Communication
{
    public interface ITcpClientHandler
    {
        void HandleClient();
    }
}