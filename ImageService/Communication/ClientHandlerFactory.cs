using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ImageService.Controller;
using ImageService.Logger;

namespace ImageService.Communication
{
    class TcpClientHandlerFactory : IClientHandlerFactory
    {
        private readonly IImageController _imageController;

        public TcpClientHandlerFactory(IImageController imageController)
        {
            _imageController = imageController;
        }

        public ITcpClientHandler Create(TcpClient client, ILoggingService loggingService)
        {
            return new TcpClientHandler(client,loggingService,_imageController);
        }

    }

    
}
