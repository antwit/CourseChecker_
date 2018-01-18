using System;
using System.IO;
using System.Net;

namespace CourseChecker.SiteReader
{
    class ReadSite
    {
        String strInhalt;

        public ReadSite(string url)
        {
            HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream());
            this.strInhalt = sr.ReadToEnd();
            sr.Close();
            response.Close();
        }

        public String GetSite()
        {
            return this.strInhalt;
        }
    }
}
