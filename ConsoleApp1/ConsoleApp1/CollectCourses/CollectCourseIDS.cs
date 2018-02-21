using CourseChecker.Course;
using CourseChecker.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CourseChecker.CollectCourses {
    class CollectCourseIDS {
        private List<Kurse> kurseIDS;

        public CollectCourseIDS(String strSite, String strAnbieter) {
            List<String> listSection = SectionTableEntry(strSite);
            kurseIDS = new List<Kurse>();

            Program.iNumberOfCourses += listSection.Count;

            foreach(String strElement in listSection) {
                List<String> listTmp = SplitSection(strElement);
                kurseIDS.Add(new Kurse(listTmp.ElementAt(0), listTmp.ElementAt(1), DateTime.Parse(listTmp.ElementAt(2)),
                                DateTime.Parse(listTmp.ElementAt(3)), listTmp.ElementAt(4), Convert.ToInt32(listTmp.ElementAt(5)), strAnbieter));
                if(Program.boolIDS & Program.boolIntegrata & Program.boolTechData) {
                    Program.bw.ReportProgress((int)((double)Program.iCounter++ / (double)Program.iNumberOfCourses * 100));
                } else {
                    Program.bw.ReportProgress((int)((double)Program.iCounter++ / (double)Program.iThousend * 100));
                }
            }

        }

        internal List<Kurse> KurseIDS => kurseIDS;

        private List<String> SectionTableEntry(String strSite) {
            List<String> listSections = new List<String>();
            int iStart = 0;
            int iEnd = 0;
            int itmp = strSite.Length;

            while(strSite.IndexOf("<tr class=\"sectiontableentry\"><td headers=", iStart) != -1) {
                iStart = strSite.IndexOf("<tr class=\"sectiontableentry\"><td headers=", iStart);
                iEnd = iStart;
                iEnd = strSite.IndexOf("</tr>", iEnd);
                listSections.Add(strSite.Substring(iStart, iEnd - iStart));
                iStart = iEnd;
            }

            return listSections;
        }

        private List<String> SplitSection(String strElement) {
            List<String> listData = new List<String>();
            String[] strTmp = strElement.Split(new String[] { "</td>" }, StringSplitOptions.None);
            String pattern = "<.*?>|&nbsp;|,00 EUR| pro Platz";
            String replace = "";
            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);

            for(int i = 0; i < strTmp.Length; i++) {
                if(i == 2 || i == 3) {
                    listData.Add((rgx.Replace(strTmp[i], replace)).Replace('ä', 'a'));
                } else if(i == 1) {
                    listData.Add(rgx.Replace(strTmp[i], replace));
                } else if(i < 6) {
                    listData.Add(rgx.Replace(strTmp[i], replace));
                }
            }

            return listData;
        }
    }
}
