using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.PhantomJS;
using System.Threading.Tasks;

namespace CourseChecker.SiteReader
{
    class ReadWithSeleniumTechDataMainSite
    {
        List<String> listUrl;
        public List<String> ListUrl => listUrl;

        public ReadWithSeleniumTechDataMainSite(Uri url, List<String> listManuelCheck, Uri uriSearch)
        {
            listUrl = new List<String>();
            using (IWebDriver driver = new PhantomJSDriver()) {
                driver.Url = url.AbsoluteUri;
                System.Threading.Thread.Sleep(3000);
                AddUrl(driver);

                IWebElement getButtom = driver.FindElement(By.LinkText("»"));
                getButtom.Click();
                System.Threading.Thread.Sleep(3000);
                AddUrl(driver);
                driver.Quit();
            }

            Parallel.ForEach(listManuelCheck, list => {
                using (IWebDriver driver = new PhantomJSDriver())
                {
                    driver.Url = uriSearch.AbsoluteUri + list;
                    System.Threading.Thread.Sleep(3000);
                    AddUrl(driver);

                    driver.Quit();
                }
            });
        }

        private void AddUrl(IWebDriver driver)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until<IWebElement>(d => d.FindElement(By.CssSelector(".btn-sm")));
            IList<IWebElement> test = driver.FindElements(By.CssSelector(".btn-sm"));
            foreach (IWebElement b in test) {
                this.listUrl.Add(b.GetAttribute("href"));
            }
        }
    }
}
