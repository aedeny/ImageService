using Newtonsoft.Json.Linq;

namespace Infrastructure
{
    public class SettingsInfo
    {
        public SettingsInfo()
        {
            HandledDir = new JArray();
        }

        public string OutputDirectory { get; set; }
        public string SourceName { get; set; }
        public string LogName { get; set; }
        public int ThumbnailSize { get; set; }
        public JArray HandledDir { get; set; }

        public static string OutputDirectoryJsonName = "OutputDirectory";
        public static string SourceNameJsonName = "SourceName";
        public static string LogNameJsonName = "LogName";
        public static string ThumbnailSizeJsonName = "ThumbnailSize";
        public static string HandledDirJsonName = "HandledDir";

        public string ToJson()
        {
            JObject settingInfoJson = new JObject
            {
                [OutputDirectoryJsonName] = OutputDirectory,
                [SourceNameJsonName] = SourceName,
                [LogNameJsonName] = LogName,
                [ThumbnailSizeJsonName] = ThumbnailSize,
                [HandledDirJsonName] = HandledDir
            };

            return settingInfoJson.ToString();
        }
    }
}