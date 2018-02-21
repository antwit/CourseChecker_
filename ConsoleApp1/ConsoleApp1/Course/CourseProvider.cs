using System;
using System.Collections.Generic;
using CourseChecker.SiteReader;
using NLog;

namespace CourseChecker.Course {
    
    abstract class CourseProvider {
        public List<Kurse> GetCourse { get; set; }
        
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

    class Integrata : CourseProvider {
        private static Logger logger = LogManager.GetCurrentClassLogger();

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
            Program.iNumberOfCourses += queueUrls.Count;
            Program.boolIntegrata = true;
            logger.Info("Links zu den entsprechenden Kursen aus Integrata extrahiert!");

            GetCourse.AddRange((new GetCoursesFromIntegrata(queueUrls)).GetListKurse);
            logger.Info("Es wurden {0} Kurse aus Integrata extrahiert.", GetCourse.Count);
        }

        private void AddRange(Queue<Uri> queue, Queue<Uri> input) {
            foreach (Uri temp in input) {
                queue.Enqueue(temp);
            }
        }

    }

    class Techdata : CourseProvider {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public Techdata(List<String> listExclude) {
            Uri uriSearch = new Uri("https://academy.techdata.com/de/search/index/#?country=DE&selectedVendor=&searchTerm=");
            Uri uriSearchDb2 = new Uri("https://academy.techdata.com/de/search/index/#?country=DE&selectedVendor=5&searchTerm=db2&modality=classroom");
            GetCourse = new List<Kurse>();

            ReadWithSeleniumTechDataMainSite collectUrl = new ReadWithSeleniumTechDataMainSite(uriSearchDb2, Program.ListManualCheckForTechData, uriSearch);
            Program.iNumberOfCourses += collectUrl.ListUrl.Count;
            Program.boolTechData = true;
            logger.Info("Links zu den entsprechenden Kursen aus TechData extrahiert!");

            GetCoursesFromTechData collectCourseTechData = new GetCoursesFromTechData(collectUrl.ListUrl, listExclude);
            GetCourse = collectCourseTechData.ListKurse;
            logger.Info("Es wurden {0} Kurse aus TechData extrahiert!", GetCourse.Count);
        }
    }

    class IDS : CourseProvider {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public List<Kurse> GetCourseIntegrata { get; set; }
        public List<Kurse> GetCourseTechData { get; set; }

        public IDS() {
            GetCourse = new List<Kurse>();
            GetCourseIntegrata = new List<Kurse>();

            GetCourseTechData = new List<Kurse>();
            String strIntegrata = "Integrata";
            String strTechData = "TechData";
            String strNAN = "NAN";

            GetCoursesFromIDS readSiteIDS_2 = new GetCoursesFromIDS(new Uri("http://www.ids-system.de/leistung/schulungen/tutor/2"), strIntegrata);
            GetCoursesFromIDS readSiteIDS_3 = new GetCoursesFromIDS(new Uri("http://www.ids-system.de/leistung/schulungen/tutor/3"), strTechData);
            GetCoursesFromIDS readSiteIDS = new GetCoursesFromIDS(new Uri("http://www.ids-system.de/component/seminarman/2-100-durchfuehrungsgarantie?Itemid=585"), strNAN);
            
            for (int i = 0; i < readSiteIDS.KurseIDS.Count; i++) {
                Vergleich(i, readSiteIDS, readSiteIDS_2);
                Vergleich(i, readSiteIDS, readSiteIDS_3);
            }

            GetCourseIntegrata.AddRange(readSiteIDS_2.KurseIDS);
            GetCourseTechData.AddRange(readSiteIDS_3.KurseIDS);
            GetCourse.AddRange(readSiteIDS_2.KurseIDS);
            GetCourse.AddRange(readSiteIDS_3.KurseIDS);

            Program.boolIDS = true;
            logger.Info("Es wurden {0} Kurse aus IDS extrahiert!", GetCourse.Count);
        }

        private void Vergleich(int i, GetCoursesFromIDS collectIDS, GetCoursesFromIDS collectIDS_2) {
            for (int j = 0; j < collectIDS_2.KurseIDS.Count; j++) {
                if (collectIDS_2.KurseIDS[j].Contains(collectIDS.KurseIDS[i])) {
                    collectIDS_2.KurseIDS[j].BoolGarantieTermin = true;
                }
            }
        }
    }
}
