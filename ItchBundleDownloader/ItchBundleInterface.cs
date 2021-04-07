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
        private BrowserInterface browserRef;

        private int currentPage;

        private List<IWebElement> currentPageGameRowElements;
        private int gameRowElementsCount;

        private string[] excludedGenres;
        private string[] excludedTags;

        public ItchBundleInterface(string rootUrl, BrowserInterface browserRef)
        {
            bundleRootUrl = rootUrl;
            this.browserRef = browserRef;
        }

        #region public

        public void SetupExclusions(string[] excludedGenres, string[] excludedTags)
        {
            this.excludedGenres = excludedGenres.Select(x => x.ToLower().Trim()).ToArray();
            this.excludedTags = excludedTags.Select(x => x.ToLower().Trim()).ToArray();
        }
        
        public void GoToPage(int page)
        {
            string pageUrl = $"{bundleRootUrl}?page={page}";
            browserRef.Navigate(pageUrl);
            browserRef.WaitForElementByClass("game_list");
            currentPage = page;
            currentPageGameRowElements = GetPageGameElements();
            gameRowElementsCount = currentPageGameRowElements.Count;
        }

        public bool GoToNextPage()
        {
            IWebElement pager = browserRef.FindElementByClass("pager");
            IWebElement next_page_button;
            
            if (pager.TryFindElementByClass("next_page", out next_page_button))
            {
                //this is silly to do rather than click the button
                //but it works, and this whole thing is a mess anyways
                GoToPage(currentPage + 1);
                return true;
            }

            return false;
        }
        
        public void ClaimCurrentPage()
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
                else 
                {
                    ClaimGame(currentPageGameRowElements[i]);
                }
            }
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

        private bool IsGameExcluded(IWebElement gameRowElement)
        {
            string gameUrl = GetGameUrl(gameRowElement);
            browserRef.Navigate(gameUrl);
            IWebElement infoPanelElement = browserRef.WaitForElementByClass("game_info_panel_widget");

            List<IWebElement> infoRows = infoPanelElement.FindElements(By.TagName("tr")).ToList();
            IWebElement genreCell = null;
            IWebElement tagsCell = null;

            foreach (IWebElement infoRow in infoRows)
            {
                List<IWebElement> rowCells = infoRow.FindElements(By.TagName("td")).ToList();
                IWebElement rowNameCell = rowCells[0];
                string rowNameText = rowNameCell.GetAttribute("textContent");
                if (rowNameText == "Genre")
                {
                    genreCell = rowCells[1];
                }
                else if (rowNameText == "Tags")
                {
                    tagsCell = rowCells[1];
                }
            }

            string genreText = string.Empty;
            string tagsText = string.Empty;

            if (genreCell != null)
            {
                genreText = genreCell.GetAttribute("textContent").ToLower();
            }

            if (tagsCell != null)
            {
                tagsText = tagsCell.GetAttribute("textContent").ToLower();
            }

            bool isExcluded = false;

            string[] genresSplit = genreText.Split(',');

            foreach (string genre in genresSplit)
            {
                if (excludedGenres.Contains(genre.Trim()))
                {
                    isExcluded = true;
                }
            }
            
            string[] tagsSplit = tagsText.Split(',');

            foreach (string tag in tagsSplit)
            {
                if (excludedTags.Contains(tag.Trim()))
                {
                    isExcluded = true;
                }
            }
            
            GoToPage(currentPage);

            return isExcluded;
        }

        private void WaitForClaimPage()
        {
            browserRef.WaitForElementByClass("game_download_page");
        }

        private List<IWebElement> GetPageGameElements()
        {
            IWebElement mainElement = browserRef.FindElementByClass("main");
            IWebElement innerColumnElement = mainElement.FindElement(By.ClassName("inner_column"));
            IWebElement gameListElement = innerColumnElement.FindElement(By.ClassName("game_list"));

            return gameListElement?.FindElements(By.ClassName("game_row")).ToList();
        }

        private bool IsGameClaimed(IWebElement gameRowElement)
        {
            IWebElement buttonRowElement = gameRowElement.FindElement(By.ClassName("button_row"));

            return buttonRowElement.TryFindElementByClass("game_download_btn", out IWebElement downloadButtonElement);
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
