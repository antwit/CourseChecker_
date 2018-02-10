using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace CourseChecker.SiteReader
{

    class ReadWithSeliumIntegrataMainSite
    {
        private Uri url;
        private String strID;
        private Queue<Uri> setsOfUrls;

        internal ReadWithSeliumIntegrataMainSite(Uri url, String strID)
        {
            this.url = url;
            this.strID = strID;
            this.setsOfUrls = new Queue<Uri>();

            CollectUrls();
        }

        private void CollectUrls()
        {
            using (IWebDriver driver = new ChromeDriver()) {
                Console.Clear();
                driver.Url = this.url.AbsoluteUri;
                GetSinglesURLs(driver);

            }
        }

        private void GetSinglesURLs(IWebDriver driver)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until<IWebElement>(d => d.FindElement(By.ClassName("column-group__sub-item-title")));
            IWebElement iWebElementDB2_LUW = driver.FindElement(By.Id(strID));
            IList<IWebElement> iListDB2_LUW = iWebElementDB2_LUW.FindElements(By.ClassName("column-group__sub-item-link"));
            foreach(IWebElement tmp in iListDB2_LUW) {
                Uri tmpo = new Uri(tmp.GetAttribute("href"));
                this.setsOfUrls.Enqueue(tmpo);
            }
        }

        internal Queue<Uri> SetsOfUrls => setsOfUrls;
    }
}
