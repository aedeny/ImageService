﻿using System;
using System.IO;
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

        //KFIR
        public event EventHandler<ConnectedEventArgs> Connected;

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
                {
                    try
                    {
                        TcpClient client = _listener.AcceptTcpClient(); // Listen for new clietns

                        _loggingService.Log("Got new connection", MessageTypeEnum.Info); // Log the new connection
                        OnConnected(client.GetStream()); // Invoke the event OnClientConnected

                        ITcpClientHandler ch = _clientHandlerFactory.Create(client, _loggingService); // Create client handler
                        ch.HandleClient(); // Start handle client
                    }
                    catch (SocketException)
                    {
                        break;
                    }
                } // End of while

                _loggingService.Log("Server stopped", MessageTypeEnum.Info);
            });
            task.Start();
        }

        public void OnConnected(NetworkStream stream)
        {
            ConnectedEventArgs args = new ConnectedEventArgs
            {
                Stream = stream
            };
            
            Connected?.Invoke(this, args);
        }

        public void Stop()
        {
            _listener.Stop();
        }
    }
}