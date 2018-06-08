using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading;
using Communication;
using Infrastructure;
using Infrastructure.Event;

namespace Web.Models
{
    public class Settings
    {
        private readonly object _lock = new object();
        private string _logName;
        private string _outputDirectory;
        private string _sourceName;

        public Settings()
        {
            if (GuiTcpClientSingleton.Instance.Connected)
            {
                GuiTcpClientSingleton.Instance.Close();
            }

            ThumbnailSize = -1;
            if (!Utils.IsServiceActive("ImageService"))
            {
                return;
            }

            GuiTcpClientSingleton.Instance.ConfigurationReceived += OnSettingsInfoReceived;

            lock (_lock)
            {
                Monitor.Wait(_lock, 5000);
            }
        }

        [DataType(DataType.Text)]
        [Display(Name = "Directory Handlers")]
        public ObservableCollection<string> DirectoryHandlers { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Log Name")]
        public string LogName
        {
            get => _logName ?? "N / A";
            set => _logName = value;
        }

        [DataType(DataType.Text)]
        [Display(Name = "Output Directory")]
        public string OutputDirectory
        {
            get => _outputDirectory ?? "N / A";
            set => _outputDirectory = value;
        }

        [DataType(DataType.Text)]
        [Display(Name = "Source Name")]
        public string SourceName
        {
            get => _sourceName ?? "N / A";
            set => _sourceName = value;
        }

        [DataType(DataType.Text)]
        [Display(Name = "Thumbnail Size")]
        public int ThumbnailSize { get; set; }

        private void OnSettingsInfoReceived(object sender, ConfigurationReceivedEventArgs e)
        {
            Debug.WriteLine("OnSettingsInfoReceived");
            SettingsInfo settingsInfo = SettingsInfo.FromJson(e.Args);
            LogName = settingsInfo.LogName;
            SourceName = settingsInfo.SourceName;
            OutputDirectory = settingsInfo.OutputDirectory;
            ThumbnailSize = settingsInfo.ThumbnailSize;

            DirectoryHandlers = new ObservableCollection<string>();
            List<string> handlers = settingsInfo.HandledDirectories;

            foreach (string handler in handlers) DirectoryHandlers.Add(handler);

            lock (_lock)
            {
                Monitor.Pulse(_lock);
            }
        }
    }
}