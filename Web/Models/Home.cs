using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Communication;
using Infrastructure;
using Infrastructure.Event;
using Newtonsoft.Json;

namespace Web.Models
{
    public class Home
    {
        private readonly object _lock = new object();
        public bool Active;
        public int NumberOfPhotos;
        public string OutputDirectory;

        public Home()
        {
            Active = false;
            GuiTcpClientSingleton.Instance.Close();
            StudentsInfoRoot = LoadStudentsInfoFromFile(@"C:\Users\edeny\Documents\ex01\details.txt");
            StudentsInfoRoot?.StudentsInfo.Sort((x, y) => string.CompareOrdinal(x.FirstName, y.FirstName));

            Active = Utils.IsServiceActive("ImageService");
            if (Active)
            {
                GuiTcpClientSingleton.Instance.ConfigurationReceived += OnConfigurationsReceived;

                lock (_lock)
                {
                    Monitor.Wait(_lock, 5000);
                }

                NumberOfPhotos = GetNumberOfPhotos(OutputDirectory);
            }
            else
            {
                NumberOfPhotos = -1;
            }
        }

        [DataType(DataType.Text)]
        [Display(Name = "Students")]
        public StudentsInfoRootObject StudentsInfoRoot { get; set; }

        private void OnConfigurationsReceived(object sender, ConfigurationReceivedEventArgs e)
        {
            SettingsInfo settingsInfo = SettingsInfo.FromJson(e.Args);
            OutputDirectory = settingsInfo.OutputDirectory;

            lock (_lock)
            {
                Monitor.Pulse(_lock);
            }
        }

        private static int GetNumberOfPhotos(string path)
        {
            if (!Directory.Exists(path))
            {
                return 0;
            }

            // Matches all files with these extensions which are not in thumbnails directory.
            string pattern = "^" + path + "\\(?!thumbnails).*.(jpg|jpeg|gif|png|bmp)";
            pattern = pattern.Replace("\\", "\\\\");
            Regex reg = new Regex(pattern, RegexOptions.IgnoreCase);
            return Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                .Where(s => reg.IsMatch(s)).ToList().Count;
        }

        public StudentsInfoRootObject LoadStudentsInfoFromFile(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<StudentsInfoRootObject>(json);
            }
        }

        public class StudentsInfoRootObject
        {
            public List<StudentInfo> StudentsInfo { get; set; }
        }
    }
}