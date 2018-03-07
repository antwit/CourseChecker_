using CourseChecker.Course;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using static CourseChecker.WPF.CounterForProgressbar;
using CourseChecker.Events;

namespace CourseChecker.SiteReader {

    class GetCoursesFromIDS {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        internal List<Kurse> KurseIDS { get; set; }
        private event EventHandler<CounterEventArgs> Counter;

        public GetCoursesFromIDS(Uri url, String strAnbieter) {
            String strSite = GetSite(url);
            List<String> listSection = SectionTableEntry(strSite);
            KurseIDS = new List<Kurse>();
            this.Counter += SiteCounter;

            NumberOfCourses += listSection.Count;

            foreach(String strElement in listSection) {
                List<String> listTmp = SplitSection(strElement);
                KurseIDS.Add(new Kurse(listTmp.ElementAt(0), listTmp.ElementAt(1), DateTime.Parse(listTmp.ElementAt(2)),
                                DateTime.Parse(listTmp.ElementAt(3)), listTmp.ElementAt(4), Convert.ToInt32(listTmp.ElementAt(5)), strAnbieter, url));
                Counter(this, new CounterEventArgs());
            }
        }

        public String GetSite(Uri url) {
            String strInhalt;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream());
            strInhalt = sr.ReadToEnd();
            sr.Close();
            response.Close();

            return strInhalt;
        }

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
