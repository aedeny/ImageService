using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using ImageService.Controller;
using ImageService.Logger;
using Infrastructure.Enums;
using Infrastructure.Event;
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
                        string retval = _imageController.ExecuteCommand((CommandEnum)int.Parse(parameters[0]),
                            parameters.Skip(1).ToArray(), out MessageTypeEnum _);
                        if (retval != null) _writer.Write(parameters[0] + ";" + retval);
                    }
                }
                catch (Exception e)
                {
                    _loggingService.Log(e.StackTrace, MessageTypeEnum.Failure);
                }
            }).Start();
        }
    }
}