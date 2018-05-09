using Newtonsoft.Json.Linq;


namespace Infrastructure
{
    public class SettingsInfo
    {
        public string OutputDirectory { get; set; }
        public string SourceName { get; set; }
        public string LogName { get; set; }
        public int ThumbnailSize { get; set; }

        public static string OutputDirectoryJsonName;
        public static string SourceNameJsonName;
        public static string LogNameJsonName;
        public static string ThumbnailSizeJsonName;


        public string ToJson()
        {
            JObject settingInfoJson = new JObject
            {
                [OutputDirectoryJsonName] = OutputDirectory,
                [SourceNameJsonName] = SourceName,
                [LogNameJsonName] = LogName,
                [ThumbnailSizeJsonName] = ThumbnailSize
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

            return settingsInfo;
        }
    }
}