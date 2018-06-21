using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading.Tasks;
using ImageService.Commands;
using ImageService.Communication;
using ImageService.Controller;
using ImageService.Logger;
using ImageService.Model;
using ImageService.Server;
using Infrastructure;
using Infrastructure.Enums;
using Infrastructure.Event;
using Infrastructure.Logging;
using Newtonsoft.Json.Linq;

namespace ImageService
{
    public enum ServiceState
    {
        ServiceStopped = 0x00000001,
        ServiceStartPending = 0x00000002,
        ServiceStopPending = 0x00000003,
        ServiceRunning = 0x00000004,
        ServiceContinuePending = 0x00000005,
        ServicePausePending = 0x00000006,
        ServicePaused = 0x00000007
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public int dwServiceType;
        public ServiceState dwCurrentState;
        public int dwControlsAccepted;
        public int dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint;
    }

    public partial class ImageService : ServiceBase
    {
        private readonly string _logName = ConfigurationManager.AppSettings["LogName"];

        // Gets info from App.config
        private readonly string _sourceName = ConfigurationManager.AppSettings["SourceName"];

        //private string kaka = ConfigurationManager.
        private IImageController _controller;
        private ImageServer _imageServer;
        private ILoggingService _loggingService;
        private IImageServiceModel _model;
        private SettingsInfo _settingsInfo;
        private TcpServer _tcpServer;

        public ImageService()
        {
            InitializeComponent();
            eventLog = new EventLog();
            if (!EventLog.SourceExists(_sourceName))
            {
                EventLog.CreateEventSource(_sourceName, _logName);
            }

            eventLog.Source = _sourceName;
            eventLog.Log = _logName;
            eventLog.EnableRaisingEvents = true;
        }

        protected override void OnStart(string[] args)
        {
            #region Other

            eventLog.WriteEntry("In OnStart", EventLogEntryType.Information);

            // Updates the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus
            {
                dwCurrentState = ServiceState.ServiceStartPending,
                dwWaitHint = 100000
            };

            SetServiceStatus(ServiceHandle, ref serviceStatus);

            // Updates the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.ServiceRunning;
            SetServiceStatus(ServiceHandle, ref serviceStatus);
            _loggingService = new LoggingService();
            _loggingService.MessageRecieved += OnMessegeRecieved;

            #endregion

            _controller = new ImageController();
            _imageServer = new ImageServer(_controller, _loggingService);
            _imageServer.DirectoryHandlerClosed += OnDirectoryHandlerClosed;

            InitializeSettingsInfo();

            _model = new ImageServiceModel(_settingsInfo.OutputDirectory, _settingsInfo.ThumbnailSize);

            // Creates TCP server
            _tcpServer = new TcpServer(8000, _loggingService, new TcpClientHandlerFactory(_controller));
            _tcpServer.NewClientConnected += OnNewClientConnected;
            eventLog.EntryWritten += _tcpServer.OnLogEntryWritten;

            _controller.AddCommand(CommandEnum.NewFileCommand, new NewFileCommand(_model));
            _controller.AddCommand(CommandEnum.CloseDirectoryHandlerCommand,
                new CloseDirectoryHandlerCommand(_imageServer));
        }

        /// <summary>
        ///     Called when a directory handler is closed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DirectoryHandlerClosedEventArgs" /> instance containing the event data.</param>
        private void OnDirectoryHandlerClosed(object sender, DirectoryHandlerClosedEventArgs e)
        {
            eventLog.WriteEntry("In OnDirectoryHandlerClosed", EventLogEntryType.Information);

            // 'e' being 'null' means to remove all handlers.
            if (e == null)
            {
                _settingsInfo.HandledDirectories.Clear();
            }
            else
            {
                _settingsInfo.HandledDirectories.Remove(e.DirectoryPath);
                _tcpServer.RemoveDirHandlerFromAllGuis(e.DirectoryPath);
            }
        }

        protected override void OnStop()
        {
            eventLog.WriteEntry("In OnStop", EventLogEntryType.Information);
            _imageServer.Close();
        }

        /// <summary>
        ///     Called when a messege is recieved.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="MessageRecievedEventArgs" /> instance containing the event data.</param>
        private void OnMessegeRecieved(object sender, MessageRecievedEventArgs args)
        {
            eventLog.WriteEntry(args.Message, args.EventLogEntryType);
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);


        /// <summary>
        ///     Initializes the settings information.
        /// </summary>
        private void InitializeSettingsInfo()
        {
            _settingsInfo = new SettingsInfo
            {
                OutputDirectory = ConfigurationManager.AppSettings["OutputDir"],
                SourceName = ConfigurationManager.AppSettings["SourceName"],
                LogName = ConfigurationManager.AppSettings["LogName"],
                ThumbnailSize = !int.TryParse(ConfigurationManager.AppSettings["ThumbnailSize"], out int thumbnailSize)
                    ? 100
                    : thumbnailSize
            };

            string handledDirInfo = ConfigurationManager.AppSettings["HandledDirectories"];
            string[] handeledDirectories = handledDirInfo.Split(';');

            foreach (string handeledDir in handeledDirectories)
            {
                _settingsInfo.HandledDirectories.Add(handeledDir);
                _imageServer.CreateHandler(handeledDir);
            }
        }


        /// <summary>
        ///     Called when a new client is connected Sends settings information and last log entries.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="NewClientConnectedEventArgs" /> instance containing the event data.</param>
        public void OnNewClientConnected(object sender, NewClientConnectedEventArgs args)
        {
            string settings = _settingsInfo.ToJson();
            args.ClientHandler.Write(CommandEnum.ConfigCommand + "|" + settings);

            Task.Run(() =>
            {
                List<Tuple<string, EventLogEntryType>> entries = new List<Tuple<string, EventLogEntryType>>();

                foreach (EventLogEntry logEntry in eventLog.Entries)
                {
                    entries.Add(new Tuple<string, EventLogEntryType>(logEntry.Message, logEntry.EntryType));
                }

                JObject logHistoryJson = new JObject
                {
                    ["LOGS"] = JArray.FromObject(entries)
                };

                args.ClientHandler.Write(CommandEnum.LogHistoryCommand + "|" + logHistoryJson);
            });
        }
    }
}