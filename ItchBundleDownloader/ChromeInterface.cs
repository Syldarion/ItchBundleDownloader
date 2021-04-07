using OpenQA.Selenium.Chrome;

namespace ItchBundleDownloader
{
    public class ChromeInterface : BrowserInterface
    {
        public ChromeInterface() : base(new ChromeDriver())
        {
            
        }
    }
}
