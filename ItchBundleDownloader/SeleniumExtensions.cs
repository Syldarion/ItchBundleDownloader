using System;
using OpenQA.Selenium;

namespace ItchBundleDownloader
{
    public static class SeleniumExtensions
    {
        public static bool TryFindElement(this IWebElement element, By selector, out IWebElement result)
        {
            try
            {
                result = element.FindElement(selector);
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
