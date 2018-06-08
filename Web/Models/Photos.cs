using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Communication;
using Infrastructure;
using Infrastructure.Event;

namespace Web.Models
{
    public class Photos
    {
        private readonly object _lock = new object();
        public string OutputDirectory;

        public Photos()
        {
            Active = false;
            Thumbnails = new ObservableCollection<PhotoInfo>();
            GuiTcpClientSingleton.Instance.Close();

            if (!Utils.IsServiceActive("ImageService"))
            {
                return;
            }

            Active = true;
            GuiTcpClientSingleton.Instance.ConfigurationReceived += OnConfigurationsReceived;

            lock (_lock)
            {
                Monitor.Wait(_lock, 5000);
            }

            if (!Directory.Exists(OutputDirectory))
            {
                return;
            }

            string pattern = OutputDirectory + "\\thumbnails\\.*.(jpg|jpeg|gif|png|bmp)";
            pattern = pattern.Replace("\\", "\\\\");
            Regex reg = new Regex(pattern, RegexOptions.IgnoreCase);

            List<string> temp = Directory.EnumerateFiles(OutputDirectory, "*.*", SearchOption.AllDirectories)
                .Where(s => reg.IsMatch(s)).ToList();

            foreach (string s in temp) Thumbnails.Add(new PhotoInfo(s));
        }

        public bool Active { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Thumbnails")]
        public ObservableCollection<PhotoInfo> Thumbnails { get; set; }

        private void OnConfigurationsReceived(object sender, ConfigurationReceivedEventArgs e)
        {
            SettingsInfo settingsInfo = SettingsInfo.FromJson(e.Args);
            OutputDirectory = settingsInfo.OutputDirectory;

            lock (_lock)
            {
                Monitor.Pulse(_lock);
            }
        }
    }
}