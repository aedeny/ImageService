using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using ImageService.Controller;
using ImageService.Logger;
using Infrastructure.Enums;

namespace ImageService.Communication
{
    public class TcpClientHandler : ITcpClientHandler
    {
        private readonly IImageController _imageController;
        private readonly ILoggingService _loggingService;
        private readonly BinaryReader _reader;
        private readonly BinaryWriter _writer;


        /// <summary>
        ///     Initializes a new instance of the <see cref="TcpClientHandler" /> class.
        /// </summary>
        /// <param name="tcpClient">The TCP client.</param>
        /// <param name="loggingService">The logging service.</param>
        /// <param name="imageController">The image controller.</param>
        public TcpClientHandler(TcpClient tcpClient, ILoggingService loggingService, IImageController imageController)
        {
            _loggingService = loggingService;
            NetworkStream stream = tcpClient.GetStream();
            _imageController = imageController;
            _reader = new BinaryReader(stream);
            _writer = new BinaryWriter(stream);
        }

        /// <summary>
        ///     Occurs when GUI client closed.
        /// </summary>
        public event EventHandler GuiClientClosed;

        /// <summary>
        ///     Handles the client.
        /// </summary>
        public void HandleClient()
        {
            new Task(() =>
            {
                try
                {
                    while (true)
                    {
                        string commandLine = _reader.ReadString();
                        string[] parameters = commandLine.Split('|');
                        string retval = _imageController.ExecuteCommand(
                            (CommandEnum) Enum.Parse(typeof(CommandEnum), parameters[0]),
                            parameters.Skip(1).ToArray(), out EventLogEntryType _);

                        if (retval == null)
                        {
                            continue;
                        }

                        _writer.Write(parameters[0] + "|" + retval);
                        _writer.Flush();
                    }
                }
                catch (Exception)
                {
                    GuiClientClosed?.Invoke(this, null);
                    _loggingService.Log("Client Closed", EventLogEntryType.Error);
                }
            }).Start();
        }

        /// <summary>
        ///     Writes the specified string to the client.
        /// </summary>
        /// <param name="s">The string.</param>
        public void Write(string s)
        {
            _writer.Write(s);
            _writer.Flush();
        }
    }
}