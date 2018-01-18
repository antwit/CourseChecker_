using CourseChecker.Course;
using CourseChecker.SiteReader;
using System;
using System.Collections.Generic;

namespace CourseChecker.CollectCourses
{
    class CollectCourseTechData
    {
        public List<Kurse> Kurse { get; set; }

        public CollectCourseTechData(List<String> strSite, List<String> listExclude)
        {
            Kurse = new List<Kurse>();
            ReadWithSeleniumTechDataSite getCours = new ReadWithSeleniumTechDataSite(strSite, listExclude);
            Kurse.AddRange(getCours.ListKurse);

        }
    }
}
