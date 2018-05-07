using System.Net.Sockets;
using ImageService.Logger;

namespace ImageService.Communication
{
    public interface IClientHandlerFactory
    {
        ITcpClientHandler Create(TcpClient client, ILoggingService loggingService);
    }
}