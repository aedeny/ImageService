using System.Net.Sockets;
using ImageService.Controller;
using ImageService.Logger;

namespace ImageService.Communication
{
    internal class TcpClientHandlerFactory : IClientHandlerFactory
    {
        private readonly IImageController _imageController;

        public TcpClientHandlerFactory(IImageController imageController)
        {
            _imageController = imageController;
        }

        public ITcpClientHandler Create(TcpClient client, ILoggingService loggingService)
        {
            return new TcpClientHandler(client, loggingService, _imageController);
        }
    }
}