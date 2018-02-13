using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.PhantomJS;

namespace CourseChecker.SiteReader
{
    class ReadWithSeleniumTechDataMainSite
    {
        List<String> listUrl;
        public ReadWithSeleniumTechDataMainSite(String url)
        {
            listUrl = new List<String>();
            using (IWebDriver driver = new PhantomJSDriver()) {
                Console.Clear();
                driver.Url = url;
                System.Threading.Thread.Sleep(2000);
                AddUrl(driver);

                IWebElement getButtom = driver.FindElement(By.LinkText("»"));
                getButtom.Click();
                System.Threading.Thread.Sleep(5000);
                AddUrl(driver);
            }
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

        public List<String> ListUrl => listUrl;
    }
}
