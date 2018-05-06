using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ImageServiceGUI.ViewModels;
using Infrastructure.Enums;
using Infrastructure.Logging;

namespace ImageServiceGUI
{
    class OurTcpClient
    {
        public event EventHandler LogMsgRecieved;
        public event EventHandler DirHandlerRemoved;
        private IPEndPoint _ep;
        private TcpClient _client;
        private LogViewModel _logViewModel;

        private BinaryWriter _writer;
        private BinaryReader _reader;
        private NetworkStream _stream;
        public OurTcpClient(LogViewModel logViewModel)
        {
            _logViewModel = logViewModel;
        }

        // KAKA

        // TODO Put in Task?
        public void Start()
        {
            _ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            _client = new TcpClient();
            _client.Connect(_ep);

            Console.WriteLine(@"You are connected");
            using (_stream = _client.GetStream())
            using (_reader = new BinaryReader(_stream))
            using (_writer = new BinaryWriter(_stream))
            {
                string readCommand = _reader.ReadString();
                Console.WriteLine(@"Result = {0}", readCommand);
            }
        }

        /**
         * Parse the msg recieved from the server.
         */
        public void ParseMessage(string msg)
        {
            string[] parameters = msg.Split(';');
            CommandEnum command = (CommandEnum) int.Parse(parameters[0]);

            throw new NotImplementedException();
        }

        /**
         * Asks the server to send the config details which will later be recieved and sent to the SettingsVM by an event.
         */
        public void GetConfigDetails()
        {
            _writer.Write(CommandEnum.GetConfigCommand.ToString());
        }

        /**
         * Send the LogViewModel a log msg via event.
         */
        public void Log(string msg, MessageTypeEnum command)
        {
            MessageRecievedEventArgs messageRecievedEventArgs = new MessageRecievedEventArgs()
            {
                Message = msg,
                Status = command
            };
            LogMsgRecieved?.Invoke(this, messageRecievedEventArgs);
        }

        /**
         * Send a command to the TcpServer to remove the specified handler and wait for a confirmation.
         */
        public bool RemoveHandler(int handlerId)
        {
            throw new NotImplementedException();
        }


        public void Close()
        {
            _client.Close();
        }
    }
}