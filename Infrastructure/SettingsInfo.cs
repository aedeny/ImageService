using Newtonsoft.Json.Linq;
using System.IO;

namespace Infrastructure
{
    public class SettingsInfo
    {
        public string OutputDirectory { get; set; }
        public string SourceName { get; set; }
        public string LogName { get; set; }
        public int ThumbnailSize { get; set; }

        public static string OutputDirectoryJsonName = "kaka";
        public static string SourceNameJsonName = "kaka2";
        public static string LogNameJsonName = "kaka3";
        public static string ThumbnailSizeJsonName = "kaka4";

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

            settingsInfo.OutputDirectory = (string) settingInfoJson[SettingsInfo.OutputDirectoryJsonName];
            settingsInfo.SourceName = (string) settingInfoJson[SettingsInfo.SourceNameJsonName];
            settingsInfo.LogName = (string) settingInfoJson[SettingsInfo.LogNameJsonName];
            settingsInfo.ThumbnailSize = (int) settingInfoJson[SettingsInfo.ThumbnailSizeJsonName];

            return settingsInfo;
        }
    }
}