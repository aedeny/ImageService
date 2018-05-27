using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Infrastructure.Logging
{
    public class LogHistory
    {
        // TODO Use this class or remove it.
        public static string LogEntriesJsonName = "LogEntries";

        public LogHistory()
        {
            LogEntries = new List<string>();
        }

        public List<string> LogEntries { get; set; }

        public string ToJson()
        {
            JObject logHistoryJson = new JObject
            {
                [LogEntriesJsonName] = JArray.FromObject(LogEntries)
            };

            return logHistoryJson.ToString();
        }

        public static LogHistory FromJson(string logHistoryAsJson)
        {
            LogHistory logHistory = new LogHistory();

            JObject logHistoryJson = JObject.Parse(logHistoryAsJson);
            logHistory.LogEntries = logHistoryJson[LogEntriesJsonName].ToObject<List<string>>();

            return logHistory;
        }
    }
}