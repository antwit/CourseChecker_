using CourseChecker.Course;
using CourseChecker.SiteReader;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CourseChecker.CollectCourses
{
    class CollectCourseIntegrata
    {

        public List<Kurse> Kurse { get; set; }

        public CollectCourseIntegrata(String strSite)
        {
            Kurse = new List<Kurse>();
            ListSection(strSite);

        }

        private void ListSection(String strSite)
        {
            List<String> listCourseURL = GetCourseURL(strSite);
            foreach (String strURL in listCourseURL) {
                ListSubSection((new ReadSite(strURL)).GetSite());
            }
        }

        private void ListSubSection(String strSite)
        {
            int iStartPreis = 0;
            int iEndPreis = 0;
            int iStart = 0;
            int iEnd = 0;
            int iPreis = 0;
            String strKursNr = "";
            String strKursTitel = "";
            DateTime dateBeginn = DateTime.MinValue;
            DateTime dateEnde = DateTime.MinValue;
            String strOrt = "";
            Boolean boolGarantie = false;

            iStartPreis = strSite.IndexOf("class=\"side-tabs__info\"><span><strong>Preis", iStartPreis);
            iEndPreis = strSite.IndexOf("netto</strong>", iStartPreis);
            String strTmpPreis = strSite.Substring(iStartPreis, iEndPreis - iStartPreis);
            String patternPreis = "<strong>(.*?),";
            Match matchPreis = Regex.Match(strTmpPreis, patternPreis);
            while (matchPreis.Success) {
                iPreis = Int32.Parse(matchPreis.Groups[1].ToString().Replace(".", ""));
                matchPreis = matchPreis.NextMatch();
            }

            while (strSite.IndexOf("itemtype=\"http://schema.org/EducationEvent\">", iStart) > 0) {

                String patternKursNR = "IBM (.*?) -";
                String patterName = "content=\"(.*?)\"><meta itemprop=\"url\"";
                String patterStartDate = "content=\"(.*?)\"><meta itemprop=\"endDate\"";
                String patterEndDate = "content=\"(.*?)\"><div itemscope itemprop=\"location\"";
                String patterOrt = "content=\"(.*?)\"><div itemprop=\"address\"";
                String patterGarantieTermin = "Garantietermin";
                iStart = strSite.IndexOf("itemtype=\"http://schema.org/EducationEvent\">", iStart);
                iEnd = strSite.IndexOf("itemtype=\"http://schema.org/PostalAddress\">", iStart);
                String strTmp = strSite.Substring(iStart, iEnd - iStart);
                iStart = iEnd;

                foreach (Match match in Regex.Matches(strTmp, patterName)) {
                    Match matchNr = Regex.Match(match.Groups[1].ToString(), patternKursNR);
                    while (matchNr.Success) {
                        strKursNr = matchNr.Groups[1].ToString();
                        matchNr = matchNr.NextMatch();
                    }
                    if (match.Groups[1].ToString().StartsWith("IBM")) {
                        strKursTitel = match.Groups[1].ToString().Split(new String[] { "-" }, StringSplitOptions.None)[1].Replace("(✓ )", "").Trim();
                    } else {
                        strKursTitel = match.Groups[1].ToString().Replace("(✓ )", "").Trim();
                    }
                }

                foreach (Match match in Regex.Matches(strTmp, patterStartDate)) {
                    dateBeginn = DateTime.Parse(match.Groups[1].ToString());
                }

                foreach (Match match in Regex.Matches(strTmp, patterEndDate)) {
                    dateEnde = DateTime.Parse(match.Groups[1].ToString());
                }

                foreach (Match match in Regex.Matches(strTmp, patterOrt)) {
                    strOrt = match.Groups[1].ToString();
                }

                foreach(Match match in Regex.Matches(strTmp, patterGarantieTermin)) {
                    boolGarantie = match.Groups[1].Success;
                }
                
                Kurse.Add(new Kurse(strKursNr, strKursTitel, dateBeginn, dateEnde, strOrt, iPreis, boolGarantie));

                System.Threading.Thread.Sleep(150);
            }
        }

        private List<String> GetCourseURL(String strSite)
        {
            List<String> listURL = new List<string>();
            String patternURL = "href=\"(.*?)\"";
            String patternDB2_ZOS = "<li id=\"db2-zos\"";
            String patternDB2_LUW = "<li id=\"db2-luw\"";
            String patternEnd = "</ul></li>";
            int iStart = 0;
            int iEnd = 0;
            int iMax = strSite.Length;

            if (strSite.IndexOf(patternDB2_ZOS) > 0) {
                iStart = strSite.IndexOf(patternDB2_ZOS);
                iEnd = strSite.IndexOf(patternEnd, iStart);
                String strZOS = strSite.Substring(iStart, iEnd - iStart);

                foreach (Match matchURL in Regex.Matches(strZOS, patternURL)) {
                    //counter++;
                    //Console.WriteLine("{1,-3} | {0}", matchURL.Groups[1], counter);
                    listURL.Add(matchURL.Groups[1].ToString());
                }
            }

            if (strSite.IndexOf(patternDB2_LUW) > 0) {
                iStart = strSite.IndexOf(patternDB2_LUW);
                iEnd = strSite.IndexOf(patternEnd, iStart);
                String strLUW = strSite.Substring(iStart, iEnd - iStart);

                foreach (Match matchURL in Regex.Matches(strLUW, patternURL)) {
                    listURL.Add(matchURL.Groups[1].ToString());
                }
            }

            return listURL;
        }
    }
}
