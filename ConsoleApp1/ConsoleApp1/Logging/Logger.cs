using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseChecker.Logging {

    public static class Logger {
        public class LogInfo {
            public String Time { get; set; }
            public String LogLvl { get; set; }
            public String Reason { get; set; }

            public LogInfo(String time, String logLvl, String reason) {
                this.Time = time;
                this.LogLvl = logLvl;
                this.Reason = reason;
            }
        }

        internal static List<LogInfo> GetLogging { get; set; } = new List<LogInfo>();

        public static void LoggerNLog(String strLevel, String strMessage, String strTime) {
            GetLogging.Add(new LogInfo(strTime, strLevel, strMessage));
        }
    }
}
