using System;
using System.ComponentModel;
using CourseChecker.Events;

namespace CourseChecker.WPF {

    class CounterForProgressbar {

        private readonly static int iEstimatedValue = 308;
        private static bool _BoolIDS = false;
        private static bool _BoolIntegrata = false;
        private static bool _BboolTechData = false;
        private static int _NumberOfCourses;
        private static int _Counter;

        internal static BackgroundWorker bw = new BackgroundWorker();

        internal static bool BoolIDS {
            private get { return _BoolIDS; }
            set { _BoolIDS = value; }
        }

        internal static bool BoolIntegrata {
            private get { return _BoolIntegrata; }
            set { _BoolIntegrata = value; }
        }

        internal static bool BoolTechData {
            private get { return _BboolTechData; }
            set { _BboolTechData = value; }
        }

        internal static int NumberOfCourses {
            get { return _NumberOfCourses; }
            set { _NumberOfCourses = value; }
        }

        internal static int Counter {
            get { return _Counter; }
            set { _Counter = value; }
        }

        internal static void SiteCounter(Object sender, CounterEventArgs e) {
            if (BoolIDS & BoolIntegrata & BoolTechData) {
                bw.ReportProgress((int)((double)Counter++ / (double)NumberOfCourses * 100));
            } else {
                bw.ReportProgress((int)((double)Counter++ / (double)iEstimatedValue * 100));
            }
        }
    }
}

