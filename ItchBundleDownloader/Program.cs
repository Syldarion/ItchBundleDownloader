using System;
using System.Collections.Generic;
using System.Globalization;

namespace ItchBundleDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Itch Bundle Claimer v0.2");
            Console.WriteLine("========================");
            Console.WriteLine("Choose your browser: 1. Chrome, 2. Firefox");

            string browserInputStr = Console.ReadLine();
            int browserInput;
            
            if (int.TryParse(browserInputStr, out browserInput) == false)
            {
                return;
            }

            switch (browserInput)
            {
                case 1:
                    BrowserInterface.SetBrowserType(BrowserInterface.Type.Chrome);
                    break;
                case 2:
                    BrowserInterface.SetBrowserType(BrowserInterface.Type.Firefox);
                    break;
                default:
                    return;
            }

            DisplayConfigMenu();
            Console.Clear();

            Console.WriteLine("Enter your Itch bundle link:");
            string bundleUrl = Console.ReadLine();

            Console.WriteLine("After the browser window opens, log in to Itch and return here to continue.");
            Console.WriteLine("Press enter to start...");
            Console.ReadLine();

            ItchBundleInterface itchBundleInterface;
            
            if (args.Length > 0 && args[0] == "dummy")
            {
                itchBundleInterface = new DummyBundleInterface(bundleUrl);
            }
            else
            {
                itchBundleInterface = new ItchBundleInterface(bundleUrl);
            }

            itchBundleInterface.Start(Config.Active.StartPage);

            Console.WriteLine("Finished claiming. Press enter to exit...");
            Console.ReadLine();
        }

        static void DisplayConfigMenu()
        {
            int currentInput = -1;

            while (currentInput != 0)
            {
                Console.Clear();
                Console.WriteLine("Current Config");
                Console.WriteLine("==============");

                Console.WriteLine($"1. Excluded Categories: {Config.Active.ExcludedCategoriesToString()}");
                Console.WriteLine($"2. Excluded Genres: {Config.Active.ExcludedGenresToString()}");
                Console.WriteLine($"3. Excluded Tags: {Config.Active.ExcludedTagsToString()}");
                Console.WriteLine($"4. Minimum Aggregate Rating: {Config.Active.MinimumRating}");
                Console.WriteLine($"5. Minimum Rating Count: {Config.Active.MinimumRatingCount}");
                Console.WriteLine($"6. Start Page: {Config.Active.StartPage}");

                Console.Write("Enter option to modify (or 0 to continue): ");
                string input = Console.ReadLine();
                int inputValue;

                if (int.TryParse(input, out inputValue) == false)
                {
                    continue;
                }

                currentInput = inputValue;

                switch (currentInput)
                {
                    case 1:
                        DisplayCategoryExclusionMenu();
                        break;
                    case 2:
                        DisplayGenreExclusionMenu();
                        break;
                    case 3:
                        DisplayTagExclusionMenu();
                        break;
                    case 4:
                        DisplayMinimumRatingMenu();
                        break;
                    case 5:
                        DisplayMinimumRatingCountMenu();
                        break;
                    case 6:
                        DisplayStartPageMenu();
                        break;
                }
            }
        }

        static void DisplayCategoryExclusionMenu()
        {
            int currentInput = -1;

            while (currentInput != 0)
            {
                Console.Clear();
                Console.WriteLine("Category Exclusions");
                Console.WriteLine("===================");

                List<KeyValuePair<string, bool>> exclusions = Config.Active.GetCategoryExclusions();

                for (int i = 0; i < exclusions.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {exclusions[i].Key} [{(exclusions[i].Value ? "" : "Not ")}Excluded]");
                }
                
                Console.Write("Enter category to toggle (or 0 to return): ");
                string input = Console.ReadLine();
                int inputValue;

                if (int.TryParse(input, out inputValue) == false)
                {
                    continue;
                }

                currentInput = inputValue;

                if (currentInput < 1 || currentInput >= exclusions.Count + 1)
                {
                    continue;
                }

                Config.Active.SetCategoryExclusion(exclusions[currentInput - 1].Key,
                                                   !exclusions[currentInput - 1].Value);
            }
        }

        static void DisplayGenreExclusionMenu()
        {
            int currentInput = -1;

            while (currentInput != 0)
            {
                Console.Clear();
                Console.WriteLine("Genre Exclusions");
                Console.WriteLine("================");

                List<KeyValuePair<string, bool>> exclusions = Config.Active.GetGenreExclusions();

                for (int i = 0; i < exclusions.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {exclusions[i].Key} [{(exclusions[i].Value ? "" : "Not ")}Excluded]");
                }
                
                Console.Write("Enter option to modify (or 0 to return): ");
                string input = Console.ReadLine();
                int inputValue;

                if (int.TryParse(input, out inputValue) == false)
                {
                    continue;
                }

                currentInput = inputValue;
                
                if (currentInput < 1 || currentInput >= exclusions.Count + 1)
                {
                    continue;
                }

                Config.Active.SetGenreExclusion(exclusions[currentInput - 1].Key,
                                                !exclusions[currentInput - 1].Value);
            }
        }

        static void DisplayTagExclusionMenu()
        {
            bool returnToConfig = false;

            while (returnToConfig == false)
            {
                Console.Clear();
                Console.WriteLine("Excluded Tags");
                Console.WriteLine("=============");

                List<string> exclusions = Config.Active.GetTagExclusions();

                for (int i = 0; i < exclusions.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {exclusions[i]}");
                }

                Console.WriteLine("\nOptions");
                Console.WriteLine("1. Add new excluded tag");
                Console.WriteLine("2. Remove excluded tag");
                Console.WriteLine("3. Return to config menu");

                Console.Write("Enter option: ");
                string input = Console.ReadLine();
                int inputValue;

                if (int.TryParse(input, out inputValue) == false)
                {
                    continue;
                }

                switch (inputValue)
                {
                    case 1:
                        Console.Write("Enter name of tag to add: ");
                        string newTag = Console.ReadLine();
                        Config.Active.AddTagExclusion(newTag?.Trim().ToLower());
                        break;
                    case 2:
                        Console.Write("Enter tag to remove: ");
                        string removeTag = Console.ReadLine();
                        Config.Active.RemoveTagExclusion(removeTag?.Trim().ToLower());
                        break;
                    case 3:
                        returnToConfig = true;
                        break;
                }
            }
        }

        static void DisplayMinimumRatingMenu()
        {
            Console.Write("Enter new minimum rating: ");
            string input = Console.ReadLine();
            float inputValue;
            if (float.TryParse(input, out inputValue))
            {
                Config.Active.MinimumRating = inputValue;
            }
        }

        static void DisplayMinimumRatingCountMenu()
        {
            Console.Write("Enter new minimum rating count: ");
            string input = Console.ReadLine();
            int inputValue;
            if (int.TryParse(input, out inputValue))
            {
                Config.Active.MinimumRatingCount = inputValue;
            }
        }

        static void DisplayStartPageMenu()
        {
            Console.Write("Enter new starting page: ");
            string input = Console.ReadLine();
            int inputValue;
            if (int.TryParse(input, out inputValue))
            {
                Config.Active.StartPage = inputValue;
            }
        }
    }
}
