using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Newtonsoft.Json.Linq;

namespace Infrastructure
{
    public class LogHistory
    {
        public static string LogEntriesJsonName = "LogEntries";
        public List<string> LogEntries { get; set; }

        public LogHistory()
        {
            LogEntries = new List<string>();
        }

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