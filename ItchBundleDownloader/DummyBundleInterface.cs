using System;
using OpenQA.Selenium;

namespace ItchBundleDownloader
{
    public class DummyBundleInterface : ItchBundleInterface
    {
        public DummyBundleInterface(string rootUrl, BrowserInterface browserRef) 
            : base(rootUrl, browserRef)
        {
            
        }

        protected override void ClaimGame(IWebElement gameRowElement)
        {
            Console.WriteLine($"Claiming {GetGameNameFromRow(gameRowElement)}");
        }
    }
}
