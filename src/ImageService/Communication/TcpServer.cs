using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ImageService.Logger;
using Infrastructure.Enums;

namespace ImageService.Communication
{
    public class TcpServer : ITcpServer
    {
        private readonly IClientHandlerFactory _clientHandlerFactory;
        private readonly List<ITcpClientHandler> _clientHandlersList;
        private readonly ILoggingService _loggingService;
        private readonly int _port;
        private TcpListener _listener;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TcpServer" /> class.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="loggingService">The logging service.</param>
        /// <param name="clientHandlerFactory">The client handler factory.</param>
        public TcpServer(int port, ILoggingService loggingService, IClientHandlerFactory clientHandlerFactory)
        {
            _clientHandlerFactory = clientHandlerFactory;
            _port = port;
            _loggingService = loggingService;
            _clientHandlersList = new List<ITcpClientHandler>();
            Start();
        }

        /// <summary>
        ///     Starts this instance.
        /// </summary>
        public void Start()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _port);
            _listener = new TcpListener(ep);

            _listener.Start();
            _loggingService.Log("Waiting for connections...", EventLogEntryType.Information);

            Task.Run(() =>
            {
                while (true)
                    try
                    {
                        // Listens for new clients.
                        TcpClient client = _listener.AcceptTcpClient();

                        _loggingService.Log("Got new connection", EventLogEntryType.Information);
                        ITcpClientHandler
                            ch = _clientHandlerFactory.Create(client, _loggingService);

                        NewClientConnected?.Invoke(this, new NewClientConnectedEventArgs {ClientHandler = ch});
                        _clientHandlersList.Add(ch);
                        ch.GuiClientClosed += (sender, args) => _clientHandlersList.Remove((ITcpClientHandler) sender);
                        ch.HandleClient();
                    }
                    catch (SocketException)
                    {
                        break;
                    }

                _loggingService.Log("Server stopped", EventLogEntryType.Information);
            });
        }

        /// <summary>
        ///     Stops this instance.
        /// </summary>
        public void Stop()
        {
            _listener.Stop();
        }

        public event EventHandler<NewClientConnectedEventArgs> NewClientConnected;

        /// <summary>
        ///     Removes the dir handler from all guis.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        public void RemoveDirHandlerFromAllGuis(string directoryPath)
        {
            foreach (ITcpClientHandler ch in _clientHandlersList)
                ch.Write(CommandEnum.CloseDirectoryHandlerCommand + "|" + directoryPath);
        }

        /// <summary>
        ///     Called when a log entry is written.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EntryWrittenEventArgs" /> instance containing the event data.</param>
        public void OnLogEntryWritten(object sender, EntryWrittenEventArgs e)
        {
            foreach (ITcpClientHandler ch in _clientHandlersList)
                ch.Write(CommandEnum.NewLogCommand + "|" + e.Entry.Message + "|" + e.Entry.EntryType);
        }
    }
}