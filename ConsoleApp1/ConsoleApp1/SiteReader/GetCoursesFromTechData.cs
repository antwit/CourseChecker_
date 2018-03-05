using CourseChecker.Course;
using HtmlAgilityPack;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static CourseChecker.WPF.CounterForProgressbar;
using CourseChecker.Events;

namespace CourseChecker.SiteReader {

    class GetCoursesFromTechData {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private HtmlWeb webContent = new HtmlWeb();
        private Uri link;
        internal List<Kurse> ListKurse { get; set; }
        private event CounterEventHandler Counter;

        public GetCoursesFromTechData(List<Uri> listUrl, List<String> listExcluded) {
            ListKurse = new List<Kurse>();
            webContent.AutoDetectEncoding = false;
            webContent.OverrideEncoding = Encoding.UTF8;
            this.Counter += SiteCounter;

            Parallel.ForEach(listUrl, url => {
                HtmlDocument htmlDoc = webContent.Load(url);
                this.link = url;
                GetCourses(htmlDoc, listExcluded);
                Counter(this, new CounterEventArgs());
            });
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
                String strTitleAndNr = htmlDoc.DocumentNode.SelectSingleNode("//*[@name='keywords']").Attributes["content"].DeEntitizeValue;
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

                if (listExcluded.Contains(kursNr_Title[0])) {
                    logger.Info("[TechData] Kurs '" + kursNr_Title[0] + "' wurde aussortiert, da es sich in der Excludeliste befand.");
                    throw new Exception();
                }
                //find all locations
                HtmlNodeCollection collNodeLocation = htmlDoc.DocumentNode.SelectNodes("//*[@class='location']");
                if (collNodeLocation.Count < 1) {
                    logger.Info("[TechData] Kein Termine gefunden für: " + kursNr_Title[0]);
                    throw new Exception();
                }

                arrLocDate = new List<string>[collNodeLocation.Count];
                for (int i = 0; i < collNodeLocation.Count; i++) {
                    String strLocation = System.Net.WebUtility.HtmlDecode(collNodeLocation[i].InnerText);
                    arrLocDate[i] = new List<string>() { strLocation };
                    //get all appointments
                    HtmlNodeCollection collNodeDates = collNodeLocation[i].SelectNodes("following-sibling::*");

                    foreach (HtmlNode nodeEle in collNodeDates) {
                        //get start and end date
                        String nodeDateElement = nodeEle.SelectSingleNode("*[@class='date']").InnerText.Trim();
                        Match matchDate = Regex.Match(nodeDateElement, patternDate);
                        String startDate = matchDate.Groups[1].Value;
                        String endDate = matchDate.Groups[2].Value;
                        arrLocDate[i].Add(startDate);
                        arrLocDate[i].Add(endDate);

                        //get price
                        String nodePoceElement = nodeEle.SelectSingleNode("*[@class='price']").InnerText.Trim();
                        Match matchPrice = Regex.Match(nodePoceElement, patternPrice);
                        strPrice = matchPrice.Groups[1].Value.Replace(".", "");
                        arrLocDate[i].Add(strPrice);

                        //is guarantee appointment
                        String strGuarantee = nodeEle.SelectSingleNode("*/*[@class='sprite-promo-icons-guaranteed-course']") == null ? "false" : "true";
                        arrLocDate[i].Add(strGuarantee);
                    }
                }

                for (int i = 0; i < arrLocDate.Length; i++) {
                    for (int j = 1; j < arrLocDate[i].Count; j += 4) {
                        DateTime dateBegin = DateTime.Parse(arrLocDate[i].ElementAt(j));
                        DateTime dateEnd = DateTime.Parse(arrLocDate[i].ElementAt(j + 1));
                        int iPrice = Int32.Parse(arrLocDate[i].ElementAt(j+2));
                        Boolean boolGuar = Boolean.Parse(arrLocDate[i].ElementAt(j + 3));

                        ListKurse.Add(new Kurse(kursNr_Title[0], kursNr_Title[1], dateBegin, dateEnd, arrLocDate[i].ElementAt(0), iPrice, boolGuar, "TechData", this.link));
                    }
                }
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }
    }
}
