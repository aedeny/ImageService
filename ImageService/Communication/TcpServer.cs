using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ImageService.Logger;
using Infrastructure.Enums;
using Infrastructure.Logging;

namespace ImageService.Communication
{
    public class TcpServer : ITcpServer
    {
        private readonly IClientHandlerFactory _clientHandlerFactory;
        private readonly List<ITcpClientHandler> _clientHandlersList;
        private readonly ILoggingService _loggingService;
        private readonly int _port;
        private TcpListener _listener;

        public TcpServer(int port, ILoggingService loggingService, IClientHandlerFactory clientHandlerFactory)
        {
            _clientHandlerFactory = clientHandlerFactory;
            _port = port;
            _loggingService = loggingService;
            _clientHandlersList = new List<ITcpClientHandler>();
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
                        // Listens for new clients.
                        TcpClient client = _listener.AcceptTcpClient();

                        _loggingService.Log("Got new connection", MessageTypeEnum.Info);
                        OnConnected(client.GetStream());

                        ITcpClientHandler
                            ch = _clientHandlerFactory.Create(client, _loggingService);

                        _clientHandlersList.Add(ch);
                        ch.GuiClientClosed += (sender, args) => _clientHandlersList.Remove((ITcpClientHandler) sender);
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

        public void RemoveDirHandlerFromAllGuis(string directoryPath)
        {
            foreach (ITcpClientHandler ch in _clientHandlersList)
                ch.Write(CommandEnum.CloseDirectoryHandlerCommand + "|" + directoryPath);
        }

        //KFIR
        public event EventHandler<ConnectedEventArgs> Connected;


        public void OnConnected(NetworkStream stream)
        {
            ConnectedEventArgs args = new ConnectedEventArgs
            {
                Stream = stream
            };

            Connected?.Invoke(this, args);
        }
    }
}