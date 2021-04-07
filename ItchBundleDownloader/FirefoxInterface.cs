using System;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace ItchBundleDownloader
{
    public class FirefoxInterface : BrowserInterface
    {
        public FirefoxInterface()
        {
            bool checkExeFolder = false;
            
            try
            {
                driver = new FirefoxDriver();
            }
            catch (WebDriverException e)
            {
                checkExeFolder = true;
            }

            if (checkExeFolder)
            {
                Console.WriteLine("Could not find Firefox driver in PATH. Checking exe folder.");
                
                string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string strWorkPath = Path.GetDirectoryName(strExeFilePath);

                string firefoxDriverPath = Path.Combine(strWorkPath, "geckodriver.exe");

                if (File.Exists(firefoxDriverPath) == false)
                {
                    throw new Exception("Could not find Firefox driver in exe folder.");
                }
                else
                {
                    driver = new FirefoxDriver(firefoxDriverPath);
                }
            }
        }
    }
}
