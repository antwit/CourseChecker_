using CourseChecker.Course;
using HtmlAgilityPack;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CourseChecker.SiteReader {

    /// <summary>
    /// Sammelt alle URL's zu den Kursen auf der gegebenen Seite
    /// </summary>
    class GetCourseUrlsFromIntegrata {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private HtmlWeb webContent = new HtmlWeb();
        internal Queue<Uri> SetsOfUrls { get; set; }
        private List<Uri> listURI = new List<Uri>();
        private List<string> listIDs  = new List<string>();
        internal List<Kurse> GetCourses { get; set; }

        /// <summary>
        /// Kostruktor
        /// </summary>
        /// <param name="listUris">URL's zu den entsprechenden Webseiten</param>
        /// <param name="listIDs">Liste mit ID's werden benutzt um die Daten Selektieren zu können</param>
        public GetCourseUrlsFromIntegrata(List<Uri> listUris, List<String> listIDs) {
            SetsOfUrls = new Queue<Uri>();
            this.listIDs.AddRange(listIDs);
            this.listURI.AddRange(listUris);

            CollectUrls();
        }

        /// <summary>
        /// Aus der URL wird der Quelltext extrahiert
        /// </summary>
        private void CollectUrls() {
            Parallel.For(0,listURI.Count, i => {
                HtmlDocument htmlDoc = webContent.Load(this.listURI[i]);
                GetSingleUrls(htmlDoc, this.listIDs[i]);
            });
        }

        /// <summary>
        /// Extrahieren der Links zu den gegebenen Quelltext 
        /// </summary>
        /// <param name="htmlDoc">Quelltext</param>
        /// <param name="strID">Liste mit ID's werden benutzt um die Daten Selektieren zu können</param>
        private void GetSingleUrls(HtmlDocument htmlDoc, string strID) {
            HtmlNode nodeID = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='" + strID + "']");
            HtmlNodeCollection collNode = nodeID.SelectNodes(".//a[@class='column-group__sub-item-link']");
            foreach(HtmlNode singleNode in collNode) {
                Uri uri = new Uri(singleNode.Attributes["href"].Value);
                SetsOfUrls.Enqueue(uri);
            }
        }
    }
}
