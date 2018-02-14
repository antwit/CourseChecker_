﻿using System;
using System.Collections.Generic;
using CourseChecker.CollectCourses;
using CourseChecker.SiteReader;

namespace CourseChecker.Course
{
    abstract class CourseProvider
    {
        public List<Kurse> GetCourse { get; set; }

        internal static void RemoveMatches(List<Kurse> course, List<Kurse> idsCourse)
        {
            List<Kurse> deleteCourse = new List<Kurse>();
            List<Kurse> deleteIDSCourse = new List<Kurse>();

            foreach (Kurse kurs in course)
            {
                foreach (Kurse kursIDS in idsCourse)
                {
                    if (kurs.Contains(kursIDS))
                    {
                        deleteCourse.Add(kurs);
                        deleteIDSCourse.Add(kursIDS);
                    }
                    else if (kurs.ContainsForIDS(kursIDS))
                    {
                        deleteIDSCourse.Add(kursIDS);
                    }
                }
            }

            foreach (Kurse m in deleteCourse)
            {
                course.Remove(m);
            }

            foreach (Kurse m in deleteIDSCourse)
            {
                idsCourse.Remove(m);
            }
        }
    }

    class Integrata:CourseProvider
    {
        public Integrata()
        {
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


            ReadWithSeliumIntegrataMainSite urlsPartOne = new ReadWithSeliumIntegrataMainSite(listURIs, listIDs);

            AddRange(queueUrls, urlsPartOne.SetsOfUrls);

            ReadWithSeleniumIntegrata getKurse = new ReadWithSeleniumIntegrata(queueUrls);
            GetCourse.AddRange(getKurse.GetListKurse());
        }

        private void AddRange(Queue<Uri> queue, Queue<Uri> input)
        {
            foreach (Uri temp in input) {
                queue.Enqueue(temp);
            }
        }

    }

    class Techdata:CourseProvider
    {
        public Techdata(List<String> listExclude)
        {
            Uri uriSearch = new Uri("https://academy.techdata.com/de/search/index/#?country=DE&selectedVendor=&searchTerm=");
            Uri uriSearchDb2 = new Uri("https://academy.techdata.com/de/search/index/#?country=DE&selectedVendor=5&searchTerm=db2&modality=classroom"); 
            GetCourse = new List<Kurse>();
            ReadWithSeleniumTechDataMainSite collectUrl = new ReadWithSeleniumTechDataMainSite(uriSearchDb2, Program.listManualCheck, uriSearch);
            CollectCourseTechData collectCourseTechData = new CollectCourseTechData(collectUrl.ListUrl, listExclude);
            GetCourse = collectCourseTechData.Kurse;
        }
    }

    class IDS:CourseProvider
    {
        public List<Kurse> GetCourseIntegrata { get; set; }
        public List<Kurse> GetCourseTechData { get; set; }

        public IDS()
        {
            GetCourse = new List<Kurse>();
            GetCourseIntegrata = new List<Kurse>();
            GetCourseTechData = new List<Kurse>();

            ReadSite readSiteIDS_2 = new ReadSite("http://www.ids-system.de/leistung/schulungen/tutor/2");
            CollectCourseIDS collectIDS_2 = new CollectCourseIDS(readSiteIDS_2.GetSite());
            ReadSite readSiteIDS_3 = new ReadSite("http://www.ids-system.de/leistung/schulungen/tutor/3");
            CollectCourseIDS collectIDS_3 = new CollectCourseIDS(readSiteIDS_3.GetSite());
            ReadSite readSiteIDS = new ReadSite("http://www.ids-system.de/component/seminarman/2-100-durchfuehrungsgarantie?Itemid=585");
            CollectCourseIDS collectIDS = new CollectCourseIDS(readSiteIDS.GetSite());

            for (int i = 0; i < collectIDS.KurseIDS.Count; i++) {
                Vergleich(i, collectIDS, collectIDS_2);
                Vergleich(i, collectIDS, collectIDS_3);
            }

            GetCourseIntegrata.AddRange(collectIDS_2.KurseIDS);
            GetCourseTechData.AddRange(collectIDS_3.KurseIDS);
            GetCourse.AddRange(collectIDS_2.KurseIDS);
            GetCourse.AddRange(collectIDS_3.KurseIDS);
        }

        private void Vergleich(int i, CollectCourseIDS collectIDS, CollectCourseIDS collectIDS_2)
        {
            for (int j = 0; j < collectIDS_2.KurseIDS.Count; j++) {
                if (collectIDS_2.KurseIDS[j].Contains(collectIDS.KurseIDS[i])) {
                    collectIDS_2.KurseIDS[j].BoolGarantieTermin = true;
                }
            }
        }
    }
}
