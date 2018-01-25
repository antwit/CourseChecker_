using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace CourseChecker.SiteReader
{
    class ReadSiteWithSeleniumIntegrata
    {
        private Uri url;
        private Queue<Uri> setsOfUrls;

        internal ReadSiteWithSeleniumIntegrata(Uri url)
        {
            this.url = url;
        }

        internal void CollectUrls()
        {
            using (IWebDriver driver = new ChromeDriver())
            {

            }
        } 
    }
}
