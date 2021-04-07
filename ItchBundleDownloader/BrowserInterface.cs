using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ItchBundleDownloader
{
    public class BrowserInterface
    {
        protected IWebDriver driver;

        public void Navigate(string url)
        {
            driver.Url = url;
        }

        public void Close()
        {
            driver.Quit();
        }
        
        public IWebElement FindElementByClass(string className)
        {
            return driver.FindElement(By.ClassName(className));
        }

        public IWebElement FindElementById(string id)
        {
            return driver.FindElement(By.Id(id));
        }

        public List<IWebElement> FindElementsByClass(string className)
        {
            return driver.FindElements(By.ClassName(className)).ToList();
        }

        public List<IWebElement> FindElementsById(string id)
        {
            return driver.FindElements(By.Id(id)).ToList();
        }

        public IWebElement WaitForElementByClass(string className)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            return wait.Until(x => x.FindElement(By.ClassName(className)));
        }
    }
}
