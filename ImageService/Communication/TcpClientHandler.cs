using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using ImageService.Controller;
using ImageService.Logger;
using Infrastructure.Enums;
using Infrastructure.Logging;

namespace ImageService.Communication
{
    public class TcpClientHandler : ITcpClientHandler
    {
        private readonly IImageController _imageController;
        private readonly ILoggingService _loggingService;
        private readonly BinaryReader _reader;
        private readonly BinaryWriter _writer;


        public TcpClientHandler(TcpClient tcpClient, ILoggingService loggingService, IImageController imageController)
        {
            _loggingService = loggingService;
            NetworkStream stream = tcpClient.GetStream();
            _imageController = imageController;
            _reader = new BinaryReader(stream);
            _writer = new BinaryWriter(stream);
        }

        public event EventHandler GuiClientClosed;

        public void HandleClient()
        {
            new Task(() =>
            {
                try
                {
                    while (true)
                    {
                        string commandLine = _reader.ReadString();
                        _loggingService.Log(@"Got command:" + commandLine, MessageTypeEnum.Info);
                        string[] parameters = commandLine.Split('|');
                        string retval = _imageController.ExecuteCommand(
                            (CommandEnum) Enum.Parse(typeof(CommandEnum), parameters[0]),
                            parameters.Skip(1).ToArray(), out MessageTypeEnum _);

                        // THIS SHOULD BE CHANGED
                        // ALL CLIENTS SHOULD BE NOTIFIED
                        if (retval == null) continue;

                        _writer.Write(parameters[0] + "|" + retval);
                        _writer.Flush();
                    }
                }
                catch (Exception e)
                {
                    GuiClientClosed?.Invoke(this,null);
                    _loggingService.Log("Client Closed", MessageTypeEnum.Failure);
                }
            }).Start();
        }

        public void Write(string s)
        {
            _writer.Write(s);
            _writer.Flush();
        }
    }
}