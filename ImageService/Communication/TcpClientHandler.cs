using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using ImageService.Logger;
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


        public TcpClientHandler(TcpClient tcpClient, ILoggingService loggingService)
        {
            _tcpClient = tcpClient;
            _loggingService = loggingService;
            _stream = _tcpClient.GetStream();
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
                    }
                }
                catch (Exception e) { }
            }).Start();
        }
    }
}