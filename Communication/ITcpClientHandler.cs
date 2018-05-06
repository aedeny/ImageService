using System.Net.Sockets;

namespace Communication
{
    internal interface ITcpClientHandler
    {
        void HandleClient(TcpClient client);
    }
}