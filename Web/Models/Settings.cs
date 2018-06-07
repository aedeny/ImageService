using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Communication;
using Infrastructure;
using Infrastructure.Event;

namespace Web.Models
{
    public class Settings
    {
        private bool _gotSettings;
        private string _logName;
        private string _outputDirectory;
        private string _sourceName;

        public Settings()
        {
            _gotSettings = false;
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

            // TODO Can we do better? Probably. Do we want to do better? No. Will we do better? Maybe.
            Task.Run(() =>
            {
                for (int i = 0; i < 20; i++)
                    if (!_gotSettings)
                    {
                        Thread.Sleep(250);
                    }
                    else
                    {
                        break;
                    }
            }).Wait();
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

            _gotSettings = true;
        }
    }
}