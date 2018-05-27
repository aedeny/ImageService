using System.Net.Sockets;
using ImageService.Logger;

namespace ImageService.Communication
{
    public interface IClientHandlerFactory
    {
        /// <summary>
        ///     Creates the specified client.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="loggingService">The logging service.</param>
        /// <returns></returns>
        ITcpClientHandler Create(TcpClient client, ILoggingService loggingService);
    }
}