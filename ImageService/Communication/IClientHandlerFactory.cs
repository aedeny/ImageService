using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ImageService.Logger;

namespace ImageService.Communication
{
    public interface IClientHandlerFactory
    {
        ITcpClientHandler Create(TcpClient client, ILoggingService loggingService);
    }
}
