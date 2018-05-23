using Newtonsoft.Json.Linq;

namespace ImageServiceGUI
{
    internal class LogHistoryInfo
    {
        public static string OutputDirectoryJsonName = "OutputDirectory";
        public static string SourceNameJsonName = "SourceName";
        public static string LogNameJsonName = "LogName";
        public static string ThumbnailSizeJsonName = "ThumbnailSize";
        public static string HandledDirJsonName = "HandledDirectories";
        public string OutputDirectory { get; set; }
        public string SourceName { get; set; }
        public string LogName { get; set; }
        public int ThumbnailSize { get; set; }
        public string HandledDir { get; set; }

        //public string ToJson()
        //{
        //    JObject settingInfoJson = new JObject
        //    {
        //        [OutputDirectoryJsonName] = OutputDirectory,
        //        [SourceNameJsonName] = SourceName,
        //        [LogNameJsonName] = LogName,
        //        [ThumbnailSizeJsonName] = ThumbnailSize,
        //        [HandledDirJsonName] = HandledDirectories
        //    };
        //    return settingInfoJson.ToString();
        //}

        public static LogHistoryInfo FromJson(string logHistoryInfoAsJson)
        {
            LogHistoryInfo logHistoryInfo = new LogHistoryInfo();

            JObject logHistoryInfoJson = JObject.Parse(logHistoryInfoAsJson);
            logHistoryInfo.OutputDirectory = (string) logHistoryInfoJson[OutputDirectoryJsonName];
            logHistoryInfo.SourceName = (string) logHistoryInfoJson[SourceNameJsonName];
            logHistoryInfo.LogName = (string) logHistoryInfoJson[LogNameJsonName];
            logHistoryInfo.ThumbnailSize = (int) logHistoryInfoJson[ThumbnailSizeJsonName];
            logHistoryInfo.HandledDir = (string) logHistoryInfoJson[HandledDirJsonName];

            return logHistoryInfo;
        }
    }
}