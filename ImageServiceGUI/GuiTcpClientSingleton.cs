using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Enums;
using Infrastructure.Event;
using Infrastructure.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ImageServiceGUI
{
    internal class GuiTcpClientSingleton
    {
        private static GuiTcpClientSingleton _instance;
        private TcpClient _client;
        private IPEndPoint _ep;

        private GuiTcpClientSingleton()
        {
            Connected = false;
            ConnectToService();
        }

        public BinaryWriter Writer { get; set; }
        public BinaryReader Reader { get; set; }

        public static GuiTcpClientSingleton Instance => _instance ?? (_instance = new GuiTcpClientSingleton());
        public bool Connected { get; private set; }

        public event EventHandler<MessageRecievedEventArgs> LogMessageRecieved;
        public event EventHandler ConnectedToService;
        public event EventHandler<DirectoryHandlerClosedEventArgs> DirectoryHandlerRemoved;
        public event EventHandler<ConfigurationReceivedEventArgs> ConfigurationReceived;

        public void ConnectToService()
        {
            _ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            _client = new TcpClient();

            Task c = Task.Run(() =>
            {
                while (!Connected)
                {
                    Thread.Sleep(100);
                    try
                    {
                        _client.Connect(_ep);
                        Connected = true;
                    }
                    catch (SocketException) { }
                }

                ConnectedToService?.Invoke(this, null);
                Listen();
            });
        }

        private void Listen()
        {
            Debug.WriteLine(@"TcpClient Connected");
            NetworkStream stream = _client.GetStream();
            Reader = new BinaryReader(stream);
            Writer = new BinaryWriter(stream);

            Task.Run(() =>
            {
                Thread.Sleep(1000);
                try
                {
                    while (true)
                    {
                        string commandLine = Reader.ReadString();
                        Debug.WriteLine("Command Recieved: " + commandLine);
                        ParseMessage(commandLine);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.StackTrace);
                }
            });
        }

        /// <summary>
        ///     Parse the msg recieved from the server.
        /// </summary>
        /// <param name="msg">The message to parse.</param>
        public void ParseMessage(string msg)
        {
            string[] parameters = msg.Split('|');
            CommandEnum command = (CommandEnum) Enum.Parse(typeof(CommandEnum), parameters[0]);

            switch (command)
            {
                case CommandEnum.NewLogCommand:
                    Log(parameters[1], (EventLogEntryType) Enum.Parse(typeof(EventLogEntryType), parameters[2]));
                    break;
                case CommandEnum.CloseDirectoryHandlerCommand:
                    DirectoryHandlerClosedEventArgs
                        dhceArgs = new DirectoryHandlerClosedEventArgs(parameters[1], "hmm");

                    DirectoryHandlerRemoved?.Invoke(this, dhceArgs);
                    break;
                case CommandEnum.NewFileCommand:
                    break;
                case CommandEnum.ConfigCommand:
                    ConfigurationReceivedEventArgs creArgs = new ConfigurationReceivedEventArgs(parameters[1]);
                    ConfigurationReceived?.Invoke(this, creArgs);
                    break;
                case CommandEnum.LogHistoryCommand:
                    string jsonMsg = msg.Replace(CommandEnum.LogHistoryCommand + "|", "");
                    JObject logHistoryJson = JObject.Parse(jsonMsg);
                    List<Tuple<string, EventLogEntryType>> entries = logHistoryJson["LOGS"].ToObject<List<Tuple<string, EventLogEntryType>>>();
                    
                    foreach (Tuple<string, EventLogEntryType> logEntry in entries)
                    {
                        Log(logEntry.Item1, logEntry.Item2);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        /// <summary>
        ///     Sends the LogViewModel a log msg via event.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="logEntryType">Message type.</param>
        public void Log(string message, EventLogEntryType logEntryType)
        {
            MessageRecievedEventArgs messageRecievedEventArgs = new MessageRecievedEventArgs
            {
                Message = message,
                EventLogEntryType = logEntryType
            };

            LogMessageRecieved?.Invoke(this, messageRecievedEventArgs);
        }


        /// <summary>
        ///     Closes the TCP Client.
        /// </summary>
        public void Close()
        {
            _client.Close();
        }

        public List<Tuple<EventLogEntryType, string>> GetLogList()
        {
            // TODO Ask service for log entries.
            return new List<Tuple<EventLogEntryType, string>>();
        }
    }
}