using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace CourseChecker.Logging {
    internal delegate void PropertyChanged(object sender, PropertyChangedEventArgs e);

    public static class Logger{
        public class LogInfo : INotifyPropertyChanged {
            public String Time { get; set; }
            public String LogLvl { get; set; }
            public String Reason { get; set; }

            public LogInfo(String time, String logLvl, String reason) {
                this.Time = time;
                this.LogLvl = logLvl;
                this.Reason = reason;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("LogInfo"));
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        internal static ICollection<LogInfo> GetLogging { get; private set; } = new ObservableCollection<LogInfo>();

        public static void LoggerNLog(String strLevel, String strMessage, String strTime) {
            Application.Current.Dispatcher.Invoke(delegate {
                GetLogging.Add(new LogInfo(strTime, strLevel, strMessage));
            });
        }
    }
}
