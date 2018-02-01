using System;
using System.Collections.Generic;
using CourseChecker.Course;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace CourseChecker.SiteReader
{
    class ReadWithSeleniumIntegrata
    {
        List<Kurse> listKurse;

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

                }
            }
        }

    }
}
