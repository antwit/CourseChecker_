﻿using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using CourseChecker.Course;
using System.Text.RegularExpressions;
using System.Linq;

namespace CourseChecker.SiteReader
{
    class ReadWithSeleniumTechDataSite
    {
        private List<Kurse> listKurse;

        public ReadWithSeleniumTechDataSite(List<String> ListUrl, List<String> listExclude)
        {
            String pattern = ".*?(\\d+\\.?\\d+)";
            Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
            listKurse = new List<Kurse>();
            List<String>[] arrLocDate = null;
            int iPrice = 0;
            String[] kursNr_Title = null;
            
            using (IWebDriver driver = new ChromeDriver()) {
                Console.Clear();
                foreach (String url in ListUrl) {
                    driver.Url = url;
                    IWebElement keywords = driver.FindElement(By.Name("keywords"));
                    IList<IWebElement> location = driver.FindElements(By.ClassName("location"));
                    arrLocDate = new List<String>[location.Count];
                    for (int i = 1; i <= location.Count; i++) {
                        Boolean boolGarantie = false;
                        String strSearchForDate = "";
                        arrLocDate[i - 1] = new List<String>();
                        IList<IWebElement> list = driver.FindElements(By.XPath("/html/body/div[4]/div/div/div/div[3]/div[2]/div/div/div[2]/ul/li[" + i + "]"));

                        IList<IWebElement> garantie = list.ElementAt(0).FindElements(By.ClassName("date"));
                        foreach (IWebElement gar in garantie)
                        {
                            try
                            {
                                IWebElement garTMP = gar.FindElement(By.ClassName("sprite-promo-icons-guaranteed-course"));
                                if (garTMP.GetAttribute("title").Equals("Guaranteed course")) { 
                                    boolGarantie = true;
                                    strSearchForDate = gar.Text;
                                }
                            }
                            catch (NoSuchElementException)
                            {

                            }
                        }

                        String[] listSplit = list.ElementAt(0).Text.Split('\n');
                        for (int j = 0; j < listSplit.Length; j++) {
                            if (j == 0) {
                                arrLocDate[i - 1].Add(listSplit.GetValue(j).ToString().Trim());
                            } else if (j % 3 == 2) {
                                arrLocDate[i - 1].Add(listSplit.GetValue(j).ToString().Trim());
                                if (boolGarantie && listSplit.GetValue(j).ToString().Trim().Equals(strSearchForDate))
                                    arrLocDate[i - 1].Add("true");
                                else
                                    arrLocDate[i - 1].Add("false");
                            } else if (j % 3 == 0) {
                                arrLocDate[i - 1].Add(listSplit.GetValue(j).ToString().Replace(".", "").Trim());
                            }
                        }
                    }

                    kursNr_Title = keywords.GetAttribute("content").Split('-');
                    if (kursNr_Title.Length > 2) {
                        for (int i = 2; i < kursNr_Title.Length; i++)
                            kursNr_Title[1] = kursNr_Title[1].Trim() + " - " + kursNr_Title[i].Trim();
                    }

                    try {
                        if (listExclude.Contains(kursNr_Title[0].Trim()))
                            throw new NoSuchElementException();                        

                        for (int i = 0; i < arrLocDate.Length; i++) {
                            for (int j = 1; j < arrLocDate[i].Count; j+=3) {
                                String[] splitDate = arrLocDate[i].ElementAt(j).Split('-');
                                DateTime dateBegin = DateTime.Parse(splitDate[0].Trim());
                                DateTime dateEnd = DateTime.Parse(splitDate[1].Trim());
                                Match m = r.Match(arrLocDate[i].ElementAt(j + 2));
                                Boolean isGuaranteed = arrLocDate[i].ElementAt(j + 1).Equals("true");
                                if (m.Success) {
                                    if (m.Groups[1].ToString().Contains('.')) {
                                        iPrice = Int32.Parse(m.Groups[1].ToString().Replace(".", ""));
                                    } else {
                                        iPrice = Int32.Parse(m.Groups[1].ToString());
                                    }
                                }
                                listKurse.Add(new Kurse(kursNr_Title[0].Trim(), kursNr_Title[1].Trim(), dateBegin, dateEnd, arrLocDate[i].ElementAt(0).ToString(), iPrice, isGuaranteed));
                            }
                        }
                    } catch (NoSuchElementException) {
                        Console.Out.WriteLine("Keine Termine für: {0}  {1}", kursNr_Title[0], kursNr_Title[1]);
                    }
                }
            }
        }

        public List<Kurse> ListKurse => listKurse;

    }
}