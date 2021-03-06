﻿using CourseChecker.Course;
using HtmlAgilityPack;
using NLog;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static CourseChecker.WPF.CounterForProgressbar;
using CourseChecker.Events;

namespace CourseChecker.SiteReader {

    /// <summary>
    /// Extrahiert die Kurse aus der gebenen URL's
    /// </summary>
    class GetCoursesFromIntegrata {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private HtmlWeb webContent = new HtmlWeb();
        private List<Kurse> lstKurse = new List<Kurse>();
        private Uri link;
        internal List<Kurse> GetListKurse { get; set; }
        private event EventHandler<CounterEventArgs> Counter;

        /// <summary>
        /// Kostruktor, jede Seite wir Parallel aufgerufen
        /// </summary>
        /// <param name="listSites">Liste mit URL's zu den einzelnen Kursen</param>
        internal GetCoursesFromIntegrata(Queue<Uri> listSites) {
            GetListKurse = new List<Kurse>();
            this.Counter += SiteCounter;

            Parallel.ForEach(listSites, url => {
                HtmlDocument htmlDoc = webContent.Load(url);
                this.link = url;
                GetData(htmlDoc);
                Counter(this, new CounterEventArgs());
            });
        }

        /// <summary>
        /// Aus dem Quelltext werden die nötigen Daten für die Kurse extrahiert
        /// </summary>
        /// <param name="htmlDoc">Quelltext</param>
        private void GetData(HtmlDocument htmlDoc) {
            String strTitle = "";
            String strNumber = "";
            String patternKursNummer = "IBM\\s+([\\d\\w]+)\\s+-";
            List<String[]> listStrArrPlaceDate = new List<string[]>();
            int iPrice = 0;

            try {
                String strPrice = htmlDoc.DocumentNode
                .SelectSingleNode("//*[@class='side-tabs__info']")
                .InnerText;
                iPrice = Convert.ToInt32(Regex.Match(strPrice, "(\\d[\\d\\.]+)").Value.Replace(".", ""));
                // get all locations
                HtmlNodeCollection placeDate = htmlDoc.DocumentNode.SelectNodes("//*[@class='city-item']");
                if (placeDate == null) {
                    strTitle = Regex.Match(htmlDoc.DocumentNode.SelectSingleNode("//title").InnerText, ".*?:(.*)?- Training, Seminar").Groups[1].Value;
                    throw new Exception();
                }

                foreach(HtmlNode node in placeDate) {
                    String[] strArrGetDatePlace = new String[4];
                    // Select all appointments for specific location
                    HtmlNodeCollection singleDate = node.SelectNodes("*/div/div[@class='row']");
                    foreach(HtmlNode eleDate in singleDate) {
                        // Course identifier
                        strTitle = eleDate.SelectSingleNode("meta[1]").GetAttributeValue("Content", "");
                        Match match = Regex.Match(strTitle, patternKursNummer);
                        if(match.Success) {
                            strNumber = match.Groups[1].Value;
                            strTitle = strTitle.Split('-')[1].Trim();
                        } else {
                            strTitle = strTitle.Trim();
                        }
                        // Start date
                        strArrGetDatePlace[1] = eleDate
                            .SelectSingleNode("meta[4]")
                            .GetAttributeValue("Content", "");
                        // End date
                        strArrGetDatePlace[2] = eleDate
                            .SelectSingleNode("meta[5]")
                            .GetAttributeValue("Content", "");
                        // Place
                        strArrGetDatePlace[0] = eleDate
                            .SelectSingleNode("div[1]/meta")
                            .GetAttributeValue("Content", "");
                        // Guarantee
                        strArrGetDatePlace[3] = eleDate.SelectSingleNode("div[4]/span/small/a") == null ? "false" : "true";

                        listStrArrPlaceDate.Add(strArrGetDatePlace);
                    }
                }
            } catch(Exception) {
                // Course not found
                logger.Info("[Integrata] Keine Termine gefunden für: {0}", strTitle);
            }

            // speichert die Kurse ab
            if(listStrArrPlaceDate.Count > 0) {
                foreach(string[] strArr in listStrArrPlaceDate) {
                    DateTime startDate = new DateTime();
                    DateTime.TryParse(strArr[1], out startDate);
                    DateTime endDate = new DateTime();
                    DateTime.TryParse(strArr[2], out endDate);
                    Boolean boolgarantie = new Boolean();
                    Boolean.TryParse(strArr[3], out boolgarantie);
                    GetListKurse.Add(new Kurse(strNumber, strTitle, startDate, endDate, strArr[0], iPrice, boolgarantie, "Integrata", this.link));
                }
            }
        }
    }
}
