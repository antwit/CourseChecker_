using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CourseChecker.Course;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace CourseChecker.SiteReader
{
    class ReadWithSeleniumIntegrata
    {
        private List<Kurse> listKurse;

        internal List<Kurse> GetListKurse() { return listKurse; }

        internal ReadWithSeleniumIntegrata(Queue<Uri> queue)
        {
            listKurse = new List<Kurse>();
            GetKurse(queue);
        }

        private void GetKurse(Queue<Uri> queue)
        {
            using(IWebDriver driver = new ChromeDriver()) {
                Console.Clear();
                foreach(Uri url in queue) {
                    driver.Url = url.AbsoluteUri;
                    GetData(driver);
                }
            }
        }

        private void GetData(IWebDriver driver)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            String strTitle = "";
            String strNumber = "";
            List<String[]> listStrArrPlaceDate = new List<string[]>();
            String patternKursNummer = "IBM\\s+([\\d\\w]+)\\s+-";
            int iPrice = 0;

            try {
                IWebElement price = wait.Until<IWebElement>(d => d.FindElement(By.ClassName("side-tabs__info")));
                iPrice = GetPrice(price.Text);
                IList<IWebElement> placeDate = driver.FindElements(By.ClassName("city-item"));
                foreach(IWebElement ele in placeDate) {
                    String[] strArrgetDatePlace = new String[4];
                    IList<IWebElement> singleDate = ele.FindElements(By.ClassName("row"));
                    foreach(IWebElement eleDate in singleDate) {
                        //Kursbezeichnung
                        strTitle = eleDate.FindElement(By.XPath("meta[1]")).GetAttribute("content");
                        Match match = Regex.Match(strTitle, patternKursNummer);
                        if (match.Success) {
                            strNumber = match.Groups[1].Value;
                            strTitle = strTitle.Split('-')[1].Trim();
                        } else {
                            strTitle = strTitle.Trim();
                        }
                        //startDate
                        strArrgetDatePlace[1] = eleDate.FindElement(By.XPath("meta[4]")).GetAttribute("content");
                        //endDate
                        strArrgetDatePlace[2] = eleDate.FindElement(By.XPath("meta[5]")).GetAttribute("content");
                        //Place
                        strArrgetDatePlace[0] = eleDate.FindElement(By.XPath("div[1]/meta")).GetAttribute("content");
                        //Garantietermin
                        try {
                            eleDate.FindElement(By.XPath("div[4]/span/small/a"));
                            strArrgetDatePlace[3] = "true";
                        } catch (NoSuchElementException) {
                            strArrgetDatePlace[3] = "false";
                        }
                        listStrArrPlaceDate.Add(strArrgetDatePlace);
                    }
                }

            } catch (NoSuchElementException) {
                Console.Out.WriteLine("Keine Termine für: {0}  {1}", strNumber, strTitle);
            }

            if (listStrArrPlaceDate.Count > 0) {
                foreach (string[] strArr in listStrArrPlaceDate) {
                    DateTime startDate = new DateTime();
                    DateTime.TryParse(strArr[1], out startDate);
                    DateTime endDate = new DateTime();
                    DateTime.TryParse(strArr[2], out endDate);
                    Boolean boolgarantie = new Boolean();
                    Boolean.TryParse(strArr[3], out boolgarantie);
                    listKurse.Add(new Kurse(strNumber, strTitle, startDate, endDate, strArr[0], iPrice, boolgarantie));
                }
            }
        }

        private String GetNumber(String number)
        {
            String pattern = "([\\w\\d]+)$";
            return Regex.Match(number, pattern).Value;
        }

        private int GetPrice(String strPrice)
        {
            String pattern = "(\\d[\\d\\.]+)";
            return Convert.ToInt32(Regex.Match(strPrice, pattern).Value.Replace(".", ""));
        }

    }
}
