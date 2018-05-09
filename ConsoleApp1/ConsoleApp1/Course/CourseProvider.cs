using System;
using System.Collections.Generic;
using CourseChecker.SiteReader;
using NLog;
using static CourseChecker.WPF.CounterForProgressbar;

namespace CourseChecker.Course {
    
    /// <summary>
    /// Liefert RemoveMatches und eine Liste wo alle Kurse gespeichert werden
    /// </summary>
    abstract class CourseProvider {
        public List<Kurse> GetCourse { get; set; }
        
        /// <summary>
        /// Bei den übergebenen Listen werden die gleichen Kurse entfernt
        /// </summary>
        /// <param name="course">erster Kurs</param>
        /// <param name="idsCourse">zweiter Kurs</param>
        internal static void RemoveMatches(List<Kurse> course, List<Kurse> idsCourse) {
            List<Kurse> deleteCourse = new List<Kurse>();
            List<Kurse> deleteIDSCourse = new List<Kurse>();

            foreach (Kurse kurs in course) {
                foreach (Kurse kursIDS in idsCourse) {
                    if (kurs.Contains(kursIDS)) {
                        deleteCourse.Add(kurs);
                        deleteIDSCourse.Add(kursIDS);
                    } else if (kurs.ContainsForIDS(kursIDS)) {
                        deleteIDSCourse.Add(kursIDS);
                    }
                }
            }

            foreach (Kurse m in deleteCourse) {
                course.Remove(m);
            }

            foreach (Kurse m in deleteIDSCourse) {
                idsCourse.Remove(m);
            }
        }
    }

    /// <summary>
    /// Hier werden die meisten Informationen zu Intgrata gespeichert
    /// </summary>
    class Integrata : CourseProvider {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Konstruktor, startet den Auftrag úm die Daten zu sammeln
        /// </summary>
        public Integrata() {
            GetCourse = new List<Kurse>();
            Queue<Uri> queueUrls = new Queue<Uri>();
            List<String> listIDs = new List<string>() {
                "db2-luw",
                "db2-z-os",
                "db2-z-os"
            };

            List<Uri> listURIs = new List<Uri>() {
                new Uri("https://www.integrata.de/seminarangebot/ibm-operations/"),
                new Uri("https://www.integrata.de/seminarangebot/ibm-operations/"),
                new Uri("https://www.integrata.de/seminarangebot/ibm-development/#db2-zos")
            };

            GetCourseUrlsFromIntegrata urlsFromIntegrata = new GetCourseUrlsFromIntegrata(listURIs, listIDs);

            AddRange(queueUrls, urlsFromIntegrata.SetsOfUrls);
            NumberOfCourses += queueUrls.Count;
            BoolIntegrata = true;
            logger.Info("[Integrata] Links zu den entsprechenden Kursen aus Integrata extrahiert!");

            GetCourse.AddRange((new GetCoursesFromIntegrata(queueUrls)).GetListKurse);
            logger.Info("[Integrata] Es wurden {0} Kurse aus Integrata extrahiert.", GetCourse.Count);
        }

        /// <summary>
        /// "Erweitert" die Klasse Queue um AddRange, sodass zwei Queue's zusammengefügt werden können  
        /// </summary>
        /// <param name="queue">Die zuerweiternde Queue</param>
        /// <param name="input">Queue als Quelle</param>
        private void AddRange(Queue<Uri> queue, Queue<Uri> input) {
            foreach (Uri temp in input) {
                queue.Enqueue(temp);
            }
        }

    }

    /// <summary>
    /// Hier werden die meisten Informationen zu TechData gespeichert
    /// </summary>
    class Techdata : CourseProvider {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Konstruktor, sammelt die Kurse
        /// </summary>
        /// <param name="listExclude">Eine Liste die Kurse beinhalten die man ausschließen will</param>
        public Techdata(List<String> listExclude) {
            Uri uriSearch = new Uri("https://academy.techdata.com/de/search/index/#?country=DE&selectedVendor=&searchTerm=");
            Uri uriSearchDb2 = new Uri("https://academy.techdata.com/de/search/index/#?country=DE&selectedVendor=5&searchTerm=db2&modality=classroom");
            GetCourse = new List<Kurse>();

            ReadWithSeleniumTechDataMainSite collectUrl = new ReadWithSeleniumTechDataMainSite(uriSearchDb2, Program.ListManualCheckForTechData, uriSearch);
            NumberOfCourses += collectUrl.ListUrl.Count;
            BoolTechData = true;
            logger.Info("[TechData] Links zu den entsprechenden Kursen aus TechData extrahiert!");

            GetCoursesFromTechData collectCourseTechData = new GetCoursesFromTechData(collectUrl.ListUrl, listExclude);
            GetCourse = collectCourseTechData.ListKurse;
            logger.Info("[TechData] Es wurden {0} Kurse aus TechData extrahiert!", GetCourse.Count);
        }
    }

    /// <summary>
    /// Hier werden die meisten Informationen zu IDS gespeichert
    /// </summary>
    class IDS : CourseProvider {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public List<Kurse> GetCourseIntegrata { get; set; }
        public List<Kurse> GetCourseTechData { get; set; }

        /// <summary>
        /// Konstruktor, Kurse werden gesammelt und in zwei Kurse, in TechData und Integrata, gespeichert
        /// </summary>
        public IDS() {
            GetCourse = new List<Kurse>();
            GetCourseIntegrata = new List<Kurse>();

            GetCourseTechData = new List<Kurse>();
            const String strIntegrata = "Integrata";
            const String strTechData = "TechData";
            const String strNAN = "NAN";

            // Addresse wurde geändert, da Tool nun obsolet
            GetCoursesFromIDS readSiteIDS_Integrata = new GetCoursesFromIDS(new Uri("http://www.google.de/"), strIntegrata);
            GetCoursesFromIDS readSiteIDS_TechData = new GetCoursesFromIDS(new Uri("http://www.google.de"), strTechData);
            GetCoursesFromIDS isGuaranteeDate = new GetCoursesFromIDS(new Uri("http://www.google.de"), strNAN);
            
            for (int i = 0; i < isGuaranteeDate.KurseIDS.Count; i++) {
                Vergleich(i, isGuaranteeDate, readSiteIDS_Integrata);
                Vergleich(i, isGuaranteeDate, readSiteIDS_TechData);
            }

            GetCourseIntegrata.AddRange(readSiteIDS_Integrata.KurseIDS);
            GetCourseTechData.AddRange(readSiteIDS_TechData.KurseIDS);
            GetCourse.AddRange(readSiteIDS_Integrata.KurseIDS);
            GetCourse.AddRange(readSiteIDS_TechData.KurseIDS);

            BoolIDS = true;
            logger.Info("[IDS] Es wurden {0} Kurse aus IDS extrahiert!", GetCourse.Count);
        }

        /// <summary>
        /// Bei den Kursen wird überprüft, ob es ein Garantietermin ist wenn ja wird das Flag gesetzt
        /// </summary>
        /// <param name="i">Zähler</param>
        /// <param name="collectIDS">Die zu prüfende Kurse</param>
        /// <param name="collectIDS_2">Kurse die einen Garantietermin haben</param>
        private void Vergleich(int i, GetCoursesFromIDS collectIDS, GetCoursesFromIDS collectIDS_2) {
            for (int j = 0; j < collectIDS_2.KurseIDS.Count; j++) {
                if (collectIDS_2.KurseIDS[j].Contains(collectIDS.KurseIDS[i])) {
                    collectIDS_2.KurseIDS[j].BoolGarantieTermin = true;
                }
            }
        }
    }
}
