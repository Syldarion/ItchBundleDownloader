using OpenQA.Selenium.Firefox;

namespace ItchBundleDownloader
{
    public class FirefoxInterface : BrowserInterface
    {
        public FirefoxInterface() : base(new FirefoxDriver())
        {
            
        }
    }
}
