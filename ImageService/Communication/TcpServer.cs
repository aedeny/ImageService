using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ImageService.Logger;
using Infrastructure.Logging;

namespace ImageService.Communication
{
    public class TcpServer : ITcpServer
    {
        private readonly IClientHandlerFactory _clientHandlerFactory;
        private readonly ILoggingService _loggingService;
        private readonly int _port;
        private TcpListener _listener;

        public TcpServer(int port, ILoggingService loggingService, IClientHandlerFactory clientHandlerFactory)
        {
            _clientHandlerFactory = clientHandlerFactory;
            _port = port;
            _loggingService = loggingService;
            Start();
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
                    try
                    {
                        TcpClient client = _listener.AcceptTcpClient();
                        _loggingService.Log("Got new connection", MessageTypeEnum.Info);
                        ITcpClientHandler ch = _clientHandlerFactory.Create(client, _loggingService);

                        ch.HandleClient();
                    }
                    catch (SocketException)
                    {
                        break;
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