using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorAppCSharpLearn.Services
{
    public class Movie
    {
        public int Rank { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public string Budget { get; set; }
        public string GrossRevenue { get; set; }
        public string Director { get; set; }
        public List<string> TopStars { get; set; }
    }

    public class MovieService
    {
        public Task<List<Movie>> GetTopMoviesAsync()
        {
            // This is sample data - in a real application, this would come from an API
            var movies = new List<Movie>
            {
                new Movie { Rank = 1, Title = "Cosmic Odyssey", Year = 2025, Budget = "$250M", GrossRevenue = "$1.8B", Director = "Sofia Winters", TopStars = new List<string> { "James Chen", "Mira Patel", "Robert Wilson" } },
                new Movie { Rank = 2, Title = "Digital Horizon", Year = 2025, Budget = "$180M", GrossRevenue = "$1.5B", Director = "Marcus Johnson", TopStars = new List<string> { "Emma Rodriguez", "David Kim", "Sophia Lee" } },
                new Movie { Rank = 3, Title = "The Last Echo", Year = 2025, Budget = "$210M", GrossRevenue = "$1.3B", Director = "Elena Petrov", TopStars = new List<string> { "Michael Barnes", "Aisha Johnson", "Thomas Wong" } },
                new Movie { Rank = 4, Title = "Quantum Dreams", Year = 2025, Budget = "$195M", GrossRevenue = "$1.2B", Director = "Jonathan Chen", TopStars = new List<string> { "Olivia Martinez", "William Park", "Zoe Wilson" } },
                new Movie { Rank = 5, Title = "Eternal Skies", Year = 2025, Budget = "$220M", GrossRevenue = "$1.1B", Director = "Amara Williams", TopStars = new List<string> { "Daniel Lee", "Isabella Nguyen", "Nathan Johnson" } },
                new Movie { Rank = 6, Title = "Memory Fragments", Year = 2024, Budget = "$160M", GrossRevenue = "$980M", Director = "Carlos Rodriguez", TopStars = new List<string> { "Hannah Smith", "Lucas Chen", "Grace Thompson" } },
                new Movie { Rank = 7, Title = "Solar Eclipse", Year = 2025, Budget = "$185M", GrossRevenue = "$950M", Director = "Nadia Patel", TopStars = new List<string> { "Ryan Wilson", "Emily Tanaka", "Jason Moore" } },
                new Movie { Rank = 8, Title = "The Silent Pulse", Year = 2024, Budget = "$140M", GrossRevenue = "$925M", Director = "Derek Johnson", TopStars = new List<string> { "Michelle Park", "Brandon Lee", "Sarah Chen" } },
                new Movie { Rank = 9, Title = "Beyond the Void", Year = 2025, Budget = "$230M", GrossRevenue = "$900M", Director = "Leila Hassan", TopStars = new List<string> { "Kevin Williams", "Julia Rodriguez", "Tyler Zhang" } },
                new Movie { Rank = 10, Title = "Neon Ghosts", Year = 2024, Budget = "$175M", GrossRevenue = "$875M", Director = "Victor Chen", TopStars = new List<string> { "Sophia Miller", "Alex Johnson", "Olivia Park" } }
            };
            
            return Task.FromResult(movies);
        }
    }
}
