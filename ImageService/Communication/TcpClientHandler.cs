using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using ImageService.Controller;
using ImageService.Logger;
using Infrastructure.Event;
using Infrastructure.Enums;
using Infrastructure.Logging;

namespace ImageService.Communication
{
    public class TcpClientHandler : ITcpClientHandler
    {
        private readonly ILoggingService _loggingService;
        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _stream;
        private readonly BinaryReader _reader;
        private readonly BinaryWriter _writer;
        private readonly IImageController _imageController;

        public TcpClientHandler(TcpClient tcpClient, ILoggingService loggingService, IImageController imageController)
        {
            _tcpClient = tcpClient;
            _loggingService = loggingService;
            _stream = _tcpClient.GetStream();
            _imageController = imageController;
            _reader = new BinaryReader(_stream);
            _writer = new BinaryWriter(_stream);
        }

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
                        string[] parameters = commandLine.Split(';');
                        string retval =_imageController.ExecuteCommand((CommandEnum) int.Parse(parameters[0]),
                            parameters.Skip(1).ToArray(), out MessageTypeEnum messageType);
                        _writer.Write(parameters[0] + ";" + retval);
                    }
                }
                catch (Exception e) { }
            }).Start();
        }

        public void OnDirectoryHandlerClosed(object sender, DirectoryHandlerClosedEventArgs args)
        {

        }
    }
}