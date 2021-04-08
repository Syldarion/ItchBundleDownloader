using System.Collections.Generic;
using System.Linq;

namespace ItchBundleDownloader
{
    public class Config
    {
        private static class StaticLoader
        {
            public static readonly Config Instance = Config.DefaultConfig();
        }

        public static Config Active => StaticLoader.Instance;

        public float MinimumRating { get; set; }

        public int MinimumRatingCount { get; set; }

        private Dictionary<string, bool> genreExclusions;
        private Dictionary<string, bool> categoryExclusions;
        private List<string> tagExclusions;

        private static Config DefaultConfig()
        {
            Config defaultConfig = new Config();

            defaultConfig.SetupDefaultCategoryExclusions();
            defaultConfig.SetupDefaultGenreExclusions();
            defaultConfig.tagExclusions = new List<string>();
            defaultConfig.MinimumRating = 0.0f;
            defaultConfig.MinimumRatingCount = 0;

            return defaultConfig;
        }

        private void SetupDefaultGenreExclusions()
        {
            genreExclusions = new Dictionary<string, bool>()
            {
                {"Action", false},
                {"Adventure", false},
                {"Card Game", false},
                {"Educational", false},
                {"Fighting", false},
                {"Interactive Fiction", false},
                {"Platformer", false},
                {"Puzzle", false},
                {"Racing", false},
                {"Rhythm", false},
                {"Role Playing", false},
                {"Shooter", false},
                {"Simulation", false},
                {"Sports", false},
                {"Strategy", false},
                {"Survival", false},
                {"Visual Novel", false},
                {"Other", false}
            };
        }

        private void SetupDefaultCategoryExclusions()
        {
            categoryExclusions = new Dictionary<string, bool>()
            {
                {"Games", false},
                {"Assets", false},
                {"Soundtrack", false},
                {"Physical game", false},
                {"Tool", false},
                {"Comic", false},
                {"Book", false},
                {"Other", false}
            };
        }

        //Are these three even necessary? I just don't want the collections messed with.
        
        public List<KeyValuePair<string, bool>> GetCategoryExclusions()
        {
            return categoryExclusions.ToList();
        }

        public List<KeyValuePair<string, bool>> GetGenreExclusions()
        {
            return genreExclusions.ToList();
        }

        public List<string> GetTagExclusions()
        {
            return tagExclusions;
        }

        public void SetCategoryExclusion(string category, bool excluded)
        {
            if (categoryExclusions.ContainsKey(category))
            {
                categoryExclusions[category] = excluded;
            }
        }

        public void SetGenreExclusion(string genre, bool excluded)
        {
            if (genreExclusions.ContainsKey(genre))
            {
                genreExclusions[genre] = excluded;
            }
        }

        public void AddTagExclusion(string tag)
        {
            if (tagExclusions.Contains(tag.ToLower()) == false)
            {
                tagExclusions.Add(tag.ToLower());
            }
        }

        public void RemoveTagExclusion(string tag)
        {
            if (tagExclusions.Contains(tag.ToLower()))
            {
                tagExclusions.Remove(tag.ToLower());
            }
        }

        public bool IsCategoryExcluded(string category)
        {
            if (categoryExclusions.ContainsKey(category) == false)
            {
                return false;
            }

            return categoryExclusions[category];
        }

        public bool IsGenreExcluded(string genre)
        {
            if (genreExclusions.ContainsKey(genre) == false)
            {
                return false;
            }

            return genreExclusions[genre];
        }
        
        public bool IsTagExcluded(string tag)
        {
            return tagExclusions.Contains(tag.ToLower());
        }

        public string ExcludedCategoriesToString()
        {
            List<string> excludedCategories = new List<string>();

            foreach (string category in categoryExclusions.Keys)
            {
                if (categoryExclusions[category])
                {
                    excludedCategories.Add(category);
                }
            }

            if (excludedCategories.Count == 0)
            {
                return "None";
            }

            return string.Join(',', excludedCategories);
        }

        public string ExcludedGenresToString()
        {
            List<string> excludedGenres = new List<string>();

            foreach (string genre in genreExclusions.Keys)
            {
                if (genreExclusions[genre])
                {
                    excludedGenres.Add(genre);
                }
            }

            if (excludedGenres.Count == 0)
            {
                return "None";
            }

            return string.Join(',', excludedGenres);
        }

        public string ExcludedTagsToString()
        {
            if (tagExclusions.Count == 0)
            {
                return "None";
            }

            return string.Join(',', tagExclusions);
        }
    }
}
