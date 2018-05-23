using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Infrastructure
{
    public class SettingsInfo
    {
        public static string OutputDirectoryJsonName = "OutputDirectory";
        public static string SourceNameJsonName = "SourceName";
        public static string LogNameJsonName = "LogName";
        public static string ThumbnailSizeJsonName = "ThumbnailSize";
        public static string HandledDirJsonName = "HandledDirectories";

        public SettingsInfo()
        {
            HandledDirectories = new List<string>();
        }

        public string OutputDirectory { get; set; }
        public string SourceName { get; set; }
        public string LogName { get; set; }
        public int ThumbnailSize { get; set; }
        public List<string> HandledDirectories { get; set; }

        public string ToJson()
        {
            JObject settingInfoJson = new JObject
            {
                [OutputDirectoryJsonName] = OutputDirectory,
                [SourceNameJsonName] = SourceName,
                [LogNameJsonName] = LogName,
                [ThumbnailSizeJsonName] = ThumbnailSize,
                [HandledDirJsonName] = JArray.FromObject(HandledDirectories)
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
            settingsInfo.HandledDirectories = settingInfoJson[HandledDirJsonName].ToObject<List<string>>();

            return settingsInfo;
        }
    }
}