﻿using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.PhantomJS;
using System.Threading.Tasks;
using NLog;

namespace CourseChecker.SiteReader {

    class ReadWithSeleniumTechDataMainSite {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public List<Uri> ListUrl;

        public ReadWithSeleniumTechDataMainSite(Uri url, List<String> listManuelCheck, Uri uriSearch) {
            ListUrl = new List<Uri>();
            using(IWebDriver driver = new PhantomJSDriver()) {
                driver.Url = url.AbsoluteUri;
                AddUrl(driver);

                IWebElement getButtom = driver.FindElement(By.LinkText("»"));
                getButtom.Click();
                AddUrl(driver);
                driver.Quit();
            }

            Parallel.ForEach(listManuelCheck, list => {
                using(IWebDriver driver = new PhantomJSDriver()) {
                    driver.Url = uriSearch.AbsoluteUri + list;
                    AddUrl(driver);
                    driver.Quit();
                }
            });
        }

        private void AddUrl(IWebDriver driver) {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector(".btn-sm")));
            IList<IWebElement> test = driver.FindElements(By.CssSelector(".btn-sm"));
            foreach(IWebElement b in test) {
                this.ListUrl.Add(new Uri(b.GetAttribute("href")));
            }
        }
    }
}
