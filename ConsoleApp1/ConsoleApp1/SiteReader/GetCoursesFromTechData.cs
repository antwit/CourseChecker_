using CourseChecker.Course;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CourseChecker.SiteReader {
    class GetCoursesFromTechData {
        private HtmlWeb webContent = new HtmlWeb();
        internal List<Kurse> ListKurse { get; set; }

        public GetCoursesFromTechData(List<String> listUrl, List<String> listExcluded) {
            ListKurse = new List<Kurse>();

            //Parallel.ForEach(listUrl, url => {
            foreach (String url in listUrl) {
                HtmlDocument htmlDoc = webContent.Load(url);
                GetCourses(htmlDoc, listExcluded);

                if (Program.boolIDS & Program.boolIntegrata & Program.boolTechData) {
                    Program.bw.ReportProgress((int)((double)Program.iCounter++ / (double)Program.iNumberOfCourses * 100));
                } else {
                    Program.bw.ReportProgress((int)((double)Program.iCounter++ / (double)Program.iThousend * 100));
                }
            }
            //});
        }

        private void GetCourses(HtmlDocument htmlDoc, List<string> listExcluded) {
            try {
                List<String>[] arrLocDate;
                String[] kursNr_Title;
                String pattern = ".*?(\\d+\\.?\\d+)";
                Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
                String patternName = "([\\w-\\d]+)\\s-\\s(.*)";
                String patternDate = "(\\d{1,2}\\.\\d{1,2}\\.\\d{2,4})\\s*-\\s*(\\d{1,2}\\.\\d{1,2}\\.\\d{2,4})";
                String patternPrice = "([\\d\\.]+),00";
                String strPrice = "";

                //get title and course number
                String strTitleAndNr = htmlDoc.DocumentNode.SelectSingleNode("//*[@name='keywords']").Attributes["content"].Value;
                kursNr_Title = new string[2] { "", "" };
                Match match = Regex.Match(strTitleAndNr, patternName);
                if (match.Groups.Count == 3) {
                    kursNr_Title[0] = match.Groups[1].Value.Trim();
                    kursNr_Title[1] = match.Groups[2].Value.Trim();
                } else if (match.Groups.Count < 3) {
                    kursNr_Title[1] = match.Groups[0].Value.Trim();
                } else {
                    throw new Exception("Title wurde nicht gefunden!");
                }

                if (listExcluded.Contains(kursNr_Title[0]))
                    throw new Exception("Kurs " + kursNr_Title[0] + " wurde aussortiert, da es sich in der Excludeliste befand!");
                //find all locations
                HtmlNodeCollection collNodeLocation = htmlDoc.DocumentNode.SelectNodes("//*[@class='location']");
                if (collNodeLocation.Count < 1)
                    throw new Exception("Kurs " + kursNr_Title[0] + " wurde aussortiert, da es keine Termine gibt!");

                arrLocDate = new List<string>[collNodeLocation.Count];
                for (int i = 1; i <= collNodeLocation.Count; i++) {
                    String strLocation = collNodeLocation[i].InnerText;
                    arrLocDate[i - 1] = new List<string>() { strLocation };
                    //get all appointments
                    HtmlNodeCollection collNodeDates = collNodeLocation[i].SelectNodes("following-sibling::*");

                    foreach (HtmlNode nodeEle in collNodeDates) {
                        //get start and end date
                        String nodeDateElement = nodeEle.SelectSingleNode("*[@class='date']").InnerText.Trim();
                        Match matchDate = Regex.Match(nodeDateElement, patternDate);
                        String startDate = matchDate.Groups[1].Value;
                        String endDate = matchDate.Groups[2].Value;
                        arrLocDate[i - 1].Add(startDate);
                        arrLocDate[i - 1].Add(endDate);

                        //get price
                        String nodePoceElement = nodeEle.SelectSingleNode("*[@class='price']").InnerText.Trim();
                        Match matchPrice = Regex.Match(nodePoceElement, patternPrice);
                        strPrice = matchPrice.Groups[1].Value.Replace(".", "");

                        //is guarantee appointment
                        String strGuarantee = nodeEle.SelectSingleNode("*/*[@class='sprite-promo-icons-guaranteed-course']") == null ? "false" : "true";
                        arrLocDate[i - 1].Add(strGuarantee);
                    }
                }

                for (int i = 0; i < arrLocDate.Length; i++) {
                    for (int j = 1; j < arrLocDate[i].Count; j += 3) {
                        DateTime dateBegin = DateTime.Parse(arrLocDate[i].ElementAt(j));
                        DateTime dateEnd = DateTime.Parse(arrLocDate[i].ElementAt(j + 1));
                        int iPrice = Int32.Parse(strPrice);
                        Boolean boolGuar = Boolean.Parse(arrLocDate[i].ElementAt(j + 2));

                        ListKurse.Add(new Kurse(kursNr_Title[0], kursNr_Title[1], dateBegin, dateEnd, arrLocDate[i].ElementAt(0), iPrice, boolGuar, "TechData"));
                    }
                }
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }
    }
}
