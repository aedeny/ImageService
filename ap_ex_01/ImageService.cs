using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using ImageService.Server;
using ImageService.Controller;
using ImageService.Model;
using ImageService.Logger;
using ImageService.Logger.Model;
using System.Configuration;

namespace ImageService
{

    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
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
    };

    public partial class ImageService : ServiceBase
    {
        private int eventId = 1;
        private ImageServer mImageServer;
        private IImageServiceModel mModel;
        private IImageController mController;
        private ILoggingService mLoggingService;

        // Gets info from App.config
        string sourceName = ConfigurationManager.AppSettings["SourceName"];
        string logName = ConfigurationManager.AppSettings["LogName"];

        public ImageService(string[] args)
        {
            InitializeComponent();
            eventLog = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists(sourceName))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    sourceName, logName);
            }
            eventLog.Source = sourceName;
            eventLog.Log = logName;
        }

        protected override void OnStart(string[] args)
        {
            eventLog.WriteEntry("In OnStart");

            // Sets up a timer to trigger every minute.  
            System.Timers.Timer timer = new System.Timers.Timer
            {
                // 60 seconds
                Interval = 60000
            };
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();

            // Updates the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus
            {
                dwCurrentState = ServiceState.SERVICE_START_PENDING,
                dwWaitHint = 100000
            };
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // Updates the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            mLoggingService = new LoggingService();
            mLoggingService.MsgRecievedEvent += OnMsgEvent;

            // Gets info from App.config
            string outputDir = ConfigurationManager.AppSettings["OutputDir"];
            string handledDir = ConfigurationManager.AppSettings["HandledDir"];
            if (!int.TryParse(ConfigurationManager.AppSettings["ThumbnailSize"], out int thumbnailSize))
            {
                // Sets default thumbnail size
                thumbnailSize = 100;
            }

            mModel = new ImageServiceModel(outputDir, thumbnailSize, mLoggingService);
            mController = new ImageController(mModel);
            mImageServer = new ImageServer(mController, mLoggingService);
            mImageServer.CreateHandler(handledDir);
        }

        protected override void OnStop()
        {
            eventLog.WriteEntry("In OnStop");
            mImageServer.Close();
        }

        private void OnMsgEvent(object sender, MessageRecievedEventArgs args)
        {
            eventLog.WriteEntry(args.Message);
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            eventLog.WriteEntry("Monitoring the System", EventLogEntryType.Information, eventId++);
        }
    }
}
