using System;
using System.Collections.Generic;
using CourseChecker.WPF;
using System.Windows;
using System.ComponentModel;
using System.Threading;

namespace CourseChecker {

    
    class Program {

        /// <summary>
        /// Auf TechData gibt es wiederum Kurse die separat geprüft werden müssen
        /// </summary>
        private readonly static List<String> listManualCheckForTechData = new List<string> {
                "TD-TS6S",
                "KM423G"
        };

        /// <summary>
        /// Hier werden alle Kurse die man aus TechData nicht benötigt aufgelistet
        /// </summary>
        internal readonly static List<String> listExcludeForTechData = new List<string> {
                "OE98G",
                "OL4AG",
                "F258G",
                "F211G",
                "AS24G",
                "AS27G"
        };
        
        internal static List<string> ListManualCheckForTechData => listManualCheckForTechData;

        /// <summary>
        /// Start des UI
        /// </summary>
        /// <param name="args">Es werden keine Parameter benötigt</param>
        [STAThread]
        static void Main(string[] args) {
            Application app = new Application();
            app.Run(new MainWindow());
        }
    }
}

