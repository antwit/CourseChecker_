using System;
using System.Collections.Generic;
using CourseChecker.WPF;
using System.Windows;
using System.ComponentModel;
using System.Threading;

namespace CourseChecker
{

    class Program
    {
        private readonly static List<String> listManualCheckForTechData = new List<string> {
                "TD-TS6S",
                "KM423G"
        };

        internal readonly static List<String> listExcludeForTechData = new List<string> {
                "OE98G",
                "OL4AG",
                "F258G",
                "F211G",
                "AS24G",
                "AS27G"
        };

        internal readonly static int iThousend = 308;
        internal static bool boolIDS = false;
        internal static bool boolIntegrata = false;
        internal static bool boolTechData = false;
        internal static int iNumberOfCourses;
        internal static int iCounter;
        internal static BackgroundWorker bw = new BackgroundWorker();

        internal static List<string> ListManualCheckForTechData => listManualCheckForTechData;

        [STAThread]
        static void Main(string[] args)
        {
            Application app = new Application();
            app.Run(new MainWindow());
        }
    }
}

