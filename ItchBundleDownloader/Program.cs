using System;
using System.Collections.Generic;
using System.Globalization;

namespace ItchBundleDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Itch Bundle Claimer v0.1");
            Console.WriteLine("========================");
            Console.WriteLine("Choose your browser: 1. Chrome, 2. Firefox");

            string browserInputStr = Console.ReadLine();
            int browserInput;
            
            if (int.TryParse(browserInputStr, out browserInput) == false)
            {
                return;
            }

            BrowserInterface browserInterface;

            try
            {
                switch (browserInput)
                {
                    case 1:
                        browserInterface = new ChromeInterface();
                        break;
                    case 2:
                        browserInterface = new FirefoxInterface();
                        break;
                    default:
                        return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to build browser interface: {e.Message}");
                Console.WriteLine("Press enter to exit...");
                Console.ReadLine();
                return;
            }

            browserInterface.Navigate("https://itch.io/");
            Console.WriteLine("Log in to Itch and hit enter to continue...");
            Console.ReadLine();
            Console.WriteLine("Enter your Itch bundle link:");

            string bundleUrl = Console.ReadLine();
            browserInterface.Navigate(bundleUrl);

            string[] genres = new[]
            {
                "Action",
                "Adventure",
                "Card Game",
                "Educational",
                "Fighting",
                "Interactive Fiction",
                "Platformer",
                "Puzzle",
                "Racing",
                "Rhythm",
                "Role Playing",
                "Shooter",
                "Simulation",
                "Sports",
                "Strategy",
                "Survival",
                "Visual Novel",
                "Other"
            };

            string genreInput = string.Empty;

            string genreSelectionString = "Itch has the following genres: ";
            for (int i = 0; i < genres.Length; i++)
            {
                genreSelectionString += $"{i + 1}. {genres[i]}, ";
            }

            Console.WriteLine(genreSelectionString);
            Console.WriteLine("Enter genre numbers to exclude in a comma-delimited list (e.g. 1,3,7):");

            string toExclude = Console.ReadLine();
            string[] toExcludeSplit = toExclude.Split(',', StringSplitOptions.RemoveEmptyEntries);

            List<string> excludedGenres = new List<string>();

            foreach (string s in toExcludeSplit)
            {
                if (int.TryParse(s, out int result))
                {
                    if (result > 0 && result <= genres.Length)
                    {
                        excludedGenres.Add(genres[result - 1]);
                    }
                }
            }

            Console.WriteLine($"Excluded genres: {string.Join(',', excludedGenres)}");

            Console.WriteLine("Enter excluded tags in a comma-delimited list (e.g. 2D,Horror,Adventure):");
            Console.WriteLine("If you need to know Itch's tags, go to https://itch.io/tags");

            toExclude = Console.ReadLine();
            toExcludeSplit = toExclude.Split(',', StringSplitOptions.RemoveEmptyEntries);

            Console.WriteLine($"Excluded tags: {string.Join(',', toExcludeSplit)}");

            Console.WriteLine("Setup complete. Press enter to start claiming...");
            Console.ReadLine();
            
            ItchBundleInterface itchBundleInterface = new DummyBundleInterface(bundleUrl, browserInterface);
            itchBundleInterface.SetupExclusions(excludedGenres.ToArray(), toExcludeSplit);
            
            itchBundleInterface.GoToPage(1);
            itchBundleInterface.ClaimCurrentPage();

            while (itchBundleInterface.GoToNextPage())
            {
                itchBundleInterface.ClaimCurrentPage();
            }

            Console.WriteLine("Finished claiming. Press enter to exit...");
            Console.ReadLine();

            browserInterface.Close();
        }
    }
}
