using System;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ItchBundleDownloader
{
    public class ChromeInterface : BrowserInterface
    {
        public ChromeInterface()
        {
            bool checkExeFolder = false;
            
            try
            {
                driver = new ChromeDriver();
            }
            catch (WebDriverException e)
            {
                checkExeFolder = true;
            }

            if (checkExeFolder)
            {
                Console.WriteLine("Could not find Chrome driver in PATH. Checking exe folder.");
                
                string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string strWorkPath = Path.GetDirectoryName(strExeFilePath);

                string chromeDriverPath = Path.Combine(strWorkPath, "chromedriver.exe");

                if (File.Exists(chromeDriverPath) == false)
                {
                    throw new Exception("Could not find Chrome driver in exe folder.");
                }
                else
                {
                    driver = new ChromeDriver(chromeDriverPath);
                }
            }
        }
    }
}
