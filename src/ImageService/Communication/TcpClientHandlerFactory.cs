using System.Net.Sockets;
using ImageService.Controller;
using ImageService.Logger;

namespace ImageService.Communication
{
    /// <summary>
    ///     TcpClientHandlerFactory.
    /// </summary>
    internal class TcpClientHandlerFactory : IClientHandlerFactory
    {
        private readonly IImageController _imageController;

        public TcpClientHandlerFactory(IImageController imageController)
        {
            _imageController = imageController;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Creates the specified TCP client.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="loggingService">The logging service.</param>
        /// <returns></returns>
        public ITcpClientHandler Create(TcpClient client, ILoggingService loggingService)
        {
            return new TcpClientHandler(client, loggingService, _imageController);
        }
    }
}