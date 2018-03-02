using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

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

        internal static ICollection<LogInfo> GetLogging { get; private set; } = new ObservableCollection<LogInfo>();

        public static void LoggerNLog(String strLevel, String strMessage, String strTime) {
            Application.Current.Dispatcher.Invoke(delegate {
                GetLogging.Add(new LogInfo(strTime, strLevel, strMessage));
            });
        }
    }
}
