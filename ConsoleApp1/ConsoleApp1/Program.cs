using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace CourseChecker
{

    class Program
    {
        static void Main(string[] args)
        {
            IDS ids = new IDS();
            List<Kurse> idsAll = ids.GetCourse;
            List<Kurse> idsIntegrata = ids.GetCourseIntegrata;
            List<Kurse> idsTechData = ids.GetCourseTechData;
            List<Kurse> integrata = (new Integrata()).GetCourse;
            //Techdata test = new Techdata();

            List<Kurse> loeschenIntegrata = new List<Kurse>();
            List<Kurse> loeschenIDSIntegrata = new List<Kurse>();

            foreach (Kurse kurs in integrata) {
                foreach (Kurse kursIDS in idsIntegrata) {
                    if (kurs.Contains(kursIDS)) {
                        loeschenIntegrata.Add(kurs);
                        loeschenIDSIntegrata.Add(kursIDS);
                    }
                }
            }

            foreach (Kurse m in loeschenIntegrata) {
                integrata.Remove(m);
            }

            foreach (Kurse m in loeschenIDSIntegrata) {
                idsIntegrata.Remove(m);
            }

            if (integrata.Count > 0) {
                int i = 1;
                Console.WriteLine("Folgende \"Integrata\" Kurse sind nicht in \"ids\" enthalten: \n");
                foreach (Kurse kurs in integrata) {
                    Console.WriteLine("{6,-3} |{0,-7} | {1,-82} | {2,-20} | {3,-20} | {4,-10} | {5}", kurs.StrKursNr, kurs.StrKursTitel, kurs.DateBeginn, kurs.DateEnde, kurs.StrOrt, kurs.IPreis, i);
                    i++;
                }
            } else {
                Console.WriteLine("Folgende \"Integrata\" Kurse sind nicht in \"ids\" enthalten: \n Keine gefunden!\n");
            }

            if (idsIntegrata.Count > 0) {
                int i = 1;
                Console.WriteLine("Folgende \"ids\" Kurse sind nicht in \"Integrata\" enthalten: \n");
                foreach (Kurse kurs in idsIntegrata) {
                    Console.WriteLine("{6,-3} |{0,-7} | {1,-82} | {2,-20} | {3,-20} | {4,-10} | {5}", kurs.StrKursNr, kurs.StrKursTitel, kurs.DateBeginn, kurs.DateEnde, kurs.StrOrt, kurs.IPreis, i);
                    i++;
                }
            } else {
                Console.WriteLine("Folgende \"ids\" Kurse sind nicht in \"Integrata\" enthalten: \n Keine gefunden!\n");
            }

            Console.ReadKey();
        }
    }

    abstract class KursAnbieter
    {
        public List<Kurse> GetCourse { get; set; }
    }

    class Integrata:KursAnbieter
    {
        public Integrata()
        {
            GetCourse = new List<Kurse>();

            ReadSite readSiteOperation = new ReadSite("https://www.integrata.de/seminarangebot/ibm-operations/");
            CollectCourseIntegrata collectIntegrataOperation = new CollectCourseIntegrata(readSiteOperation.GetSite());
            ReadSite readSiteDevelopment = new ReadSite("https://www.integrata.de/seminarangebot/ibm-development/");
            CollectCourseIntegrata collectIntegrataDevelopment = new CollectCourseIntegrata(readSiteDevelopment.GetSite());

            GetCourse.AddRange(collectIntegrataDevelopment.Kurse);
            GetCourse.AddRange(collectIntegrataOperation.Kurse);
        }
    }

    class Techdata:KursAnbieter
    {
        public Techdata()
        {
            GetCourse = new List<Kurse>();
            ReadSite readSite = new ReadSite("https://academy.techdata.com/de/search/index/#?country=DE&selectedVendor=5&searchTerm=db2&modality=classroom");
        }
    }

    class IDS:KursAnbieter
    {
        public List<Kurse> GetCourseIntegrata { get; set; }
        public List<Kurse> GetCourseTechData { get; set; }

        public IDS()
        {
            GetCourse = new List<Kurse>();
            GetCourseIntegrata = new List<Kurse>();
            GetCourseTechData = new List<Kurse>();

            ReadSite readSiteIDS_2 = new ReadSite("http://www.ids-system.de/schulungen/tutor/2");
            CollectCourseIDS collectIDS_2 = new CollectCourseIDS(readSiteIDS_2.GetSite());
            ReadSite readSiteIDS_3 = new ReadSite("http://www.ids-system.de/schulungen/tutor/3");
            CollectCourseIDS collectIDS_3 = new CollectCourseIDS(readSiteIDS_3.GetSite());
            ReadSite readSiteIDS = new ReadSite("http://www.ids-system.de/component/seminarman/2-100-durchfuehrungsgarantie?Itemid=585");
            CollectCourseIDS collectIDS = new CollectCourseIDS(readSiteIDS.GetSite());

            for (int i = 0; i < collectIDS.KurseIDS.Count; i++) {
                Vergleich(i, collectIDS, collectIDS_2);
                Vergleich(i, collectIDS, collectIDS_3);
            }

            GetCourseIntegrata.AddRange(collectIDS_2.KurseIDS);
            GetCourseTechData.AddRange(collectIDS_3.KurseIDS);
            GetCourse.AddRange(collectIDS_2.KurseIDS);
            GetCourse.AddRange(collectIDS_3.KurseIDS);
        }

        private void Vergleich(int i, CollectCourseIDS collectIDS, CollectCourseIDS collectIDS_2)
        {
            for (int j = 0; j < collectIDS_2.KurseIDS.Count; j++) {
                if (collectIDS_2.KurseIDS[j].Contains(collectIDS.KurseIDS[i])) {
                    collectIDS_2.KurseIDS[j].BoolGarantieTermin = true;
                }
            }
        }
    }

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

    class CollectCourseTechData
    {
        public List<Kurse> Kurse { get; set; }

        public CollectCourseTechData(String strSite)
        {
            Kurse = new List<Kurse>();
            List<String> listSection = ListSection(strSite);

        }

        private List<String> ListSection(String strSite)
        {
            List<String> listSections = new List<String>();

            return listSections;
        }
    }

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
                        strKursTitel = match.Groups[1].ToString().Split(new String[] { "-" }, StringSplitOptions.None)[1].Trim();
                    } else {
                        strKursTitel = match.Groups[1].ToString();
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

                Kurse.Add(new Kurse(strKursNr, strKursTitel, dateBeginn, dateEnde, strOrt, iPreis));

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

    class CollectCourseIDS
    {
        private List<Kurse> kurseIDS;

        public CollectCourseIDS(String strSite)
        {
            List<String> listSection = SectionTableEntry(strSite);
            kurseIDS = new List<Kurse>();

            foreach (String strElement in listSection) {
                List<String> listTmp = SplitSection(strElement);
                kurseIDS.Add(new Kurse(listTmp.ElementAt(0), listTmp.ElementAt(1), DateTime.Parse(listTmp.ElementAt(2)),
                                DateTime.Parse(listTmp.ElementAt(3)), listTmp.ElementAt(4), Convert.ToInt32(listTmp.ElementAt(5))));
            }

        }

        internal List<Kurse> KurseIDS => kurseIDS;

        private List<String> SectionTableEntry(String strSite)
        {
            List<String> listSections = new List<String>();
            int iStart = 0;
            int iEnd = 0;
            int itmp = strSite.Length;

            while (strSite.IndexOf("<tr class=\"sectiontableentry\"><td headers=", iStart) != -1) {
                iStart = strSite.IndexOf("<tr class=\"sectiontableentry\"><td headers=", iStart);
                iEnd = iStart;
                iEnd = strSite.IndexOf("</tr>", iEnd);
                listSections.Add(strSite.Substring(iStart, iEnd - iStart));
                iStart = iEnd;
            }

            return listSections;
        }

        private List<String> SplitSection(String strElement)
        {
            List<String> listData = new List<String>();
            String[] strTmp = strElement.Split(new String[] { "</td>" }, StringSplitOptions.None);
            String pattern = "<.*?>|&nbsp;|,00 EUR| pro Platz";
            String replace = "";
            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);

            for (int i = 0; i < strTmp.Length; i++) {
                if (i == 2 || i == 3) {
                    listData.Add((rgx.Replace(strTmp[i], replace)).Replace('ä', 'a'));
                } else if (i == 1) {
                    listData.Add(rgx.Replace(strTmp[i], replace));
                } else if (i < 6) {
                    listData.Add(rgx.Replace(strTmp[i], replace));
                }
            }

            return listData;
        }
    }

    class Kurse
    {
        private String strKursNr;
        private String strKursTitel;
        private DateTime dateBeginn;
        private DateTime dateEnde;
        private String strOrt;
        private int iPreis;
        private Boolean boolGarantieTermin;

        public Kurse(String strKursNr, String strKursTitel, DateTime dateBeginn, DateTime dateEnde, String strOrt, int iPreis)
        {
            this.strKursNr = strKursNr;
            this.strKursTitel = strKursTitel;
            this.dateBeginn = dateBeginn;
            this.dateEnde = dateEnde;
            this.strOrt = strOrt;
            this.iPreis = iPreis;
            this.boolGarantieTermin = false;
        }

        public String StrOrt => strOrt;
        public int IPreis => iPreis;
        public Boolean BoolGarantieTermin { get => boolGarantieTermin; set => boolGarantieTermin = value; }
        public DateTime DateEnde => dateEnde;
        public DateTime DateBeginn => dateBeginn;
        public String StrKursTitel => strKursTitel;
        public String StrKursNr => strKursNr;

        public Boolean Contains(Kurse kursCheck)
        {
            if (this.strKursNr.ToLowerInvariant().Equals(kursCheck.StrKursNr.ToLowerInvariant()) & this.dateBeginn.Equals(kursCheck.DateBeginn) & this.dateEnde.Equals(kursCheck.DateEnde)
                & this.strOrt.ToLowerInvariant().Equals(kursCheck.StrOrt.ToLowerInvariant()) & this.iPreis.Equals(kursCheck.IPreis)
                & this.strKursTitel.ToLowerInvariant().Replace("–", "-").Replace("/", "").Replace("ibm ", "").Equals(kursCheck.StrKursTitel.ToLowerInvariant().Replace("–", "-").Replace("/", "").Replace("ibm ", ""))) { return true; }
            return false;
        }

    }
}

