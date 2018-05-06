using System.Net.Sockets;

namespace Communication
{
    public interface ITcpClientHandler
    {
        void HandleClient(TcpClient client);
    }
}