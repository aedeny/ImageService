using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
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

        /// <summary>
        /// Parse the msg recieved from the server.
        /// </summary>
        /// <param name="msg">The message to parse.</param>
        public void ParseMessage(string msg)
        {
            string[] parameters = msg.Split(';');
            CommandEnum command = (CommandEnum) int.Parse(parameters[0]);
            if (command == CommandEnum.LogCommand)
            {
                Log(parameters[1], (MessageTypeEnum)int.Parse(parameters[2]));
            }
            else if (command == CommandEnum.CloseCommand)
            {
                
            }

            throw new NotImplementedException();
        }

        /**
         * Asks the server to send the config details which will later be recieved and sent to the SettingsVM by an event.
         */
        public void GetConfigDetails()
        {
            _writer.Write(CommandEnum.GetConfigCommand.ToString());
        }

        /// <summary>
        /// Sends the LogViewModel a log msg via event.
        /// </summary>
        /// <param name="msg">The message to log.</param>
        /// <param name="messageType">Message type.</param>
        public void Log(string msg, MessageTypeEnum messageType)
        {
            MessageRecievedEventArgs messageRecievedEventArgs = new MessageRecievedEventArgs()
            {
                Message = msg,
                Status = messageType
            };
            LogMsgRecieved?.Invoke(this, messageRecievedEventArgs);
        }

        /// <summary>
        /// Sends a messageType to the TCP Server to remove the specified handler and wait for a confirmation.
        /// </summary>
        /// <param name="handlerId"></param>
        /// <returns></returns>
        public bool RemoveHandler(int handlerId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Closes the TCP Client.
        /// </summary>
        public void Close()
        {
            _client.Close();
        }
    }
}