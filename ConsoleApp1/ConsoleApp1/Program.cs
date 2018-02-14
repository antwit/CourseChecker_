using System;
using System.Collections.Generic;
using CourseChecker.WPF;
using System.Windows;

namespace CourseChecker
{

    class Program
    {
        internal readonly static List<String> listManualCheck = new List<string> {
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

        [STAThread]
        static void Main(string[] args)
        {
            Application app = new Application();
            app.Run(new MainWindow());
        }
    }
}

