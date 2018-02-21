﻿using CourseChecker.Course;
using CourseChecker.SiteReader;
using System;
using System.Collections.Generic;

namespace CourseChecker.CollectCourses {
    class CollectCourseTechData {
        public List<Kurse> Kurse { get; set; }

        public CollectCourseTechData(List<String> strSite, List<String> listExclude) {
            Kurse = new List<Kurse>();
            GetCoursesFromTechData getCourses = new GetCoursesFromTechData(strSite, listExclude);
            Kurse.AddRange(getCourses.ListKurse);
        }
    }
}
