using CourseChecker.Course;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseChecker.SiteReader {
    class GetCourseUrlsFromIntegrata {
        private HtmlWeb webContent = new HtmlWeb();
        internal Queue<Uri> SetsOfUrls { get; set; }
        private List<Uri> listURI = new List<Uri>();
        private List<string> listIDs  = new List<string>();
        internal List<Kurse> GetCourses { get; set; }

        public GetCourseUrlsFromIntegrata(List<Uri> listUris, List<String> listIDs) {
            SetsOfUrls = new Queue<Uri>();
            this.listIDs.AddRange(listIDs);
            this.listURI.AddRange(listUris);

            CollectUrls();
        }

        private void CollectUrls() {
            Parallel.For(0,listURI.Count, i => {
                HtmlDocument htmlDoc = webContent.Load(this.listURI[i]);
                GetSingleUrls(htmlDoc, this.listIDs[i]);
            });
        }

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
