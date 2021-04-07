using System;
using OpenQA.Selenium;

namespace ItchBundleDownloader
{
    public static class SeleniumExtensions
    {
        public static bool TryFindElementByClass(this IWebElement element, string className, out IWebElement result)
        {
            try
            {
                result = element.FindElement(By.ClassName(className));
                return true;
            }
            catch (NoSuchElementException e)
            {
                result = null;
                return false;
            }
        }
    }
}
