using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace ItchBundleDownloader
{
    public class GamePageInterface
    {
        private string gameUrl;
        private BrowserInterface browserInterface;
        
        public GamePageInterface(string gameUrl, BrowserInterface browserInterface)
        {
            this.gameUrl = gameUrl;
            this.browserInterface = browserInterface;
        }

        /// <summary>
        /// Retrieve the information from the game page info widget.
        /// </summary>
        /// <param name="info">Outbound GamePageInfo to store the info in.</param>
        /// <returns>True if the information was retrieved, otherwise False.</returns>
        public bool GetPageInfo(out GamePageInfo info)
        {
            browserInterface.Navigate(gameUrl);
            IWebElement infoElement = browserInterface.WaitForElement(By.ClassName("game_info_panel_widget"));

            info = new GamePageInfo()
            {
                aggregateRating = 0.0f,
                category = string.Empty,
                genres = new string[] { },
                ratingCount = 0,
                tags = new string[] { }
            };
            
            if (infoElement == null)
            {
                return false;
            }
            
            List<IWebElement> infoRows = infoElement.FindElements(By.TagName("tr")).ToList();
            
            foreach (IWebElement infoRow in infoRows)
            {
                List<IWebElement> rowCells = infoRow.FindElements(By.TagName("td")).ToList();
                IWebElement rowNameCell = rowCells[0];
                string rowNameText = rowNameCell.GetAttribute("textContent");

                switch (rowNameText)
                {
                    case "Rating":
                        ParseRatingCell(rowCells[1], ref info);
                        break;
                    case "Category":
                        ParseCategoryCell(rowCells[1], ref info);
                        break;
                    case "Genre":
                        ParseGenreCell(rowCells[1], ref info);
                        break;
                    case "Tags":
                        ParseTagsCell(rowCells[1], ref info);
                        break;
                }
            }

            //It seems on itch that only the Games category does
            //not display a category on the page.
            if (string.IsNullOrEmpty(info.category))
            {
                info.category = "Games";
            }

            return true;
        }

        private void ParseRatingCell(IWebElement ratingCell, ref GamePageInfo info)
        {
            IWebElement aggregateElement = ratingCell.FindElement(By.ClassName("aggregate_rating"));
            IWebElement countElement = ratingCell.FindElement(By.ClassName("rating_count"));

            string aggregateValue = aggregateElement.GetAttribute("title");
            string countValue = countElement.GetAttribute("content");

            if (float.TryParse(aggregateValue, out info.aggregateRating) == false)
            {
                info.aggregateRating = 0.0f;
            }

            if (int.TryParse(countValue, out info.ratingCount) == false)
            {
                info.ratingCount = 0;
            }
        }

        private void ParseCategoryCell(IWebElement categoryCell, ref GamePageInfo info)
        {
            string categoryValue = categoryCell.GetAttribute("textContent");
            info.category = categoryValue;
        }

        private void ParseGenreCell(IWebElement genreCell, ref GamePageInfo info)
        {
            string genreValue = genreCell.GetAttribute("textContent");
            string[] genres = genreValue.Split(',', StringSplitOptions.RemoveEmptyEntries);
            info.genres = genres.Select(x => x.Trim()).ToArray();
        }

        private void ParseTagsCell(IWebElement tagsCell, ref GamePageInfo info)
        {
            string tagsValue = tagsCell.GetAttribute("textContent");
            string[] tags = tagsValue.Split(',', StringSplitOptions.RemoveEmptyEntries);
            info.tags = tags.Select(x => x.Trim()).ToArray();
        }
    }
}
