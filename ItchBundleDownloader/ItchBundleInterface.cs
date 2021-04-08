using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ItchBundleDownloader
{
    public class ItchBundleInterface
    {
        private string bundleRootUrl;
        private BrowserInterface browserInterface;

        private int currentPage;

        private List<IWebElement> currentPageGameRowElements;
        private int gameRowElementsCount;

        public ItchBundleInterface(string rootUrl)
        {
            bundleRootUrl = rootUrl;
            browserInterface = BrowserInterface.Build();
        }

        #region public

        public void Start()
        {
            Console.WriteLine("Navigating to Itch...");
            browserInterface.Navigate("https://itch.io");
            Console.WriteLine("Log in and press enter to begin claiming.");
            Console.ReadLine();

            GoToPage(1);
            ClaimCurrentPage();

            while (GoToNextPage())
            {
                ClaimCurrentPage();
            }

            browserInterface.Close();
        }

        #endregion

        #region protected
        
        protected virtual void ClaimGame(IWebElement gameRowElement)
        {
            Console.WriteLine($"Claiming {GetGameNameFromRow(gameRowElement)}");
            
            //clicking the button will navigate to a new page
            ClickClaimButton(gameRowElement);
            WaitForClaimPage();
            //return to the current page
            GoToPage(currentPage);
        }
        
        protected string GetGameNameFromRow(IWebElement gameRowElement)
        {
            IWebElement titleElement = gameRowElement.FindElement(By.ClassName("game_title"));
            return titleElement.Text;
        }
        
        #endregion

        #region private

        private void GoToPage(int page)
        {
            string pageUrl = $"{bundleRootUrl}?page={page}";
            browserInterface.Navigate(pageUrl);
            browserInterface.WaitForElement(By.ClassName("game_list"));
            currentPage = page;
            currentPageGameRowElements = GetPageGameElements();
            gameRowElementsCount = currentPageGameRowElements.Count;
        }

        private bool GoToNextPage()
        {
            IWebElement pager = browserInterface.FindElementByClass("pager");

            if (pager.TryFindElement(By.ClassName("next_page"), out IWebElement _))
            {
                //this is silly to do rather than click the button
                //but it works, and this whole thing is a mess anyways
                GoToPage(currentPage + 1);
                return true;
            }

            return false;
        }
        
        private void ClaimCurrentPage()
        {
            for (int i = 0; i < gameRowElementsCount; i++)
            {
                if (IsGameClaimed(currentPageGameRowElements[i]))
                {
                    Console.WriteLine($"Already claimed {GetGameNameFromRow(currentPageGameRowElements[i])}");
                }
                else if (IsGameExcluded(currentPageGameRowElements[i]))
                {
                    Console.WriteLine($"Excluding {GetGameNameFromRow(currentPageGameRowElements[i])}");
                }
                else if(HasClaimButton(currentPageGameRowElements[i]))
                {
                    ClaimGame(currentPageGameRowElements[i]);
                }
            }
        }
        
        private bool HasClaimButton(IWebElement gameRowElement)
        {
            IWebElement buttonRowElement = gameRowElement.FindElement(By.ClassName("button_row"));
            IWebElement downloadButtonElement;
            return buttonRowElement.TryFindElement(By.Name("action"), out downloadButtonElement);
        }
        
        private bool IsGameExcluded(IWebElement gameRowElement)
        {
            string gameUrl = GetGameUrl(gameRowElement);

            GamePageInterface gamePageInterface = new GamePageInterface(gameUrl, browserInterface);
            GamePageInfo gameInfo;
            bool retrievalSuccess = gamePageInterface.GetPageInfo(out gameInfo);

            GoToPage(currentPage);

            if (retrievalSuccess == false)
            {
                return true;
            }

            if (gameInfo.aggregateRating < Config.Active.MinimumRating)
            {
                return true;
            }

            if (gameInfo.ratingCount < Config.Active.MinimumRatingCount)
            {
                return true;
            }
            
            if (Config.Active.IsCategoryExcluded(gameInfo.category))
            {
                return true;
            }
            
            foreach (string genre in gameInfo.genres)
            {
                if (Config.Active.IsGenreExcluded(genre))
                {
                    return true;
                }
            }

            foreach (string tag in gameInfo.tags)
            {
                if (Config.Active.IsTagExcluded(tag))
                {
                    return true;
                }
            }

            return false;
        }

        private void WaitForClaimPage()
        {
            browserInterface.WaitForElement(By.ClassName("game_download_page"));
        }

        private List<IWebElement> GetPageGameElements()
        {
            IWebElement mainElement = browserInterface.FindElementByClass("main");
            IWebElement innerColumnElement = mainElement.FindElement(By.ClassName("inner_column"));
            IWebElement gameListElement = innerColumnElement.FindElement(By.ClassName("game_list"));

            return gameListElement?.FindElements(By.ClassName("game_row")).ToList();
        }

        private bool IsGameClaimed(IWebElement gameRowElement)
        {
            IWebElement buttonRowElement = gameRowElement.FindElement(By.ClassName("button_row"));
            return buttonRowElement.TryFindElement(By.ClassName("game_download_btn"), out IWebElement _);
        }

        private void ClickClaimButton(IWebElement gameRowElement)
        {
            IWebElement buttonRowElement = gameRowElement.FindElement(By.ClassName("button_row"));
            IWebElement downloadButtonElement = buttonRowElement.FindElement(By.Name("action"));
            //this should be the form submit action button
            downloadButtonElement.Click();
        }

        private string GetGameUrl(IWebElement gameRowElement)
        {
            IWebElement titleElement = gameRowElement.FindElement(By.ClassName("game_title"));
            IWebElement linkElement = titleElement.FindElement(By.TagName("a"));

            return linkElement?.GetProperty("href");
        }
        
        #endregion
    }
}
