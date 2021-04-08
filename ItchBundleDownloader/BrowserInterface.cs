using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ItchBundleDownloader
{
    public class BrowserInterface
    {
        public enum Type
        {
            Chrome,
            Firefox
        }
        
        private static Type activeBrowserType;
        
        protected IWebDriver driver;

        public static void SetBrowserType(Type type)
        {
            activeBrowserType = type;
        }

        public static BrowserInterface Build()
        {
            switch (activeBrowserType)
            {
                case Type.Chrome:
                    return new ChromeInterface();
                case Type.Firefox:
                    return new FirefoxInterface();
                default:
                    throw new Exception($"Invalid activeBrowserType set: {activeBrowserType}");
            }
        }
        
        public bool Navigate(string url)
        {
            try
            {
                driver.Url = url;
                return true;
            }
            catch (WebDriverException e)
            {
                Console.WriteLine($"Page load error: {e.Message}");
                return false;
            }
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

        public IWebElement WaitForElement(By selector)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            try
            {
                IWebElement foundElement = wait.Until(x => x.FindElement(selector));
                return foundElement;
            }
            catch (WebDriverTimeoutException e)
            {
                return null;
            }
        }
    }
}
