using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ImageService.Logger;
using Infrastructure.Logging;

namespace ImageService.Communication
{
    public class TcpServer : ITcpServer
    {
        private readonly int _port;
        private TcpListener _listener;
        private ICollection<ITcpClient> _clients;
        private readonly ITcpClientHandler _ch;
        private readonly ILoggingService _loggingService;

        public TcpServer(int port, ITcpClientHandler ch, ILoggingService loggingService)
        {
            _port = port;
            _ch = ch;
            _clients = new List<ITcpClient>();
            _loggingService = loggingService;
        }


        public void Start()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _port);
            _listener = new TcpListener(ep);

            _listener.Start();
            _loggingService.Log("Waiting for connections...", MessageTypeEnum.Info);

            Task task = new Task(() =>
            {
                while (true)
                {
                    try
                    {
                        TcpClient client = _listener.AcceptTcpClient();
                        _loggingService.Log("Got new connection", MessageTypeEnum.Info);
                        _ch.HandleClient(client);
                    }
                    catch (SocketException)
                    {
                        break;
                    }
                }

                _loggingService.Log("Server stopped", MessageTypeEnum.Info);
            });
            task.Start();
        }

        public void Stop()
        {
            _listener.Stop();
        }
    }
}