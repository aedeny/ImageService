using Newtonsoft.Json.Linq;


namespace ImageServiceGUI
{
    internal class SettingsInfo
    {
        public string OutputDirectory { get; set; }
        public string SourceName { get; set; }
        public string LogName { get; set; }
        public int ThumbnailSize { get; set; }
        public string HandledDir { get; set; }

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

        public static SettingsInfo FromJson(string settingsInfoAsJson)
        {
            SettingsInfo settingsInfo = new SettingsInfo();

            JObject settingInfoJson = JObject.Parse(settingsInfoAsJson);
            settingsInfo.OutputDirectory = (string) settingInfoJson[OutputDirectoryJsonName];
            settingsInfo.SourceName = (string) settingInfoJson[SourceNameJsonName];
            settingsInfo.LogName = (string) settingInfoJson[LogNameJsonName];
            settingsInfo.ThumbnailSize = (int) settingInfoJson[ThumbnailSizeJsonName];
            settingsInfo.HandledDir = (string)settingInfoJson[SettingsInfo.HandledDirJsonName];

            return settingsInfo;
        }
    }
}