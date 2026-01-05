using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vidora.Core.Entities
{
    public class Movie
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string? PosterUrl { get; set; }
        public string? BannerUrl { get; set; }
        public string? VideoUrl { get; set; }
        public string? TrailerUrl { get; set; }

        public int ReleaseYear { get; set; }
        public double Rating { get; set; }
        public List<string> Genres { get; set; } = new();
        public int DurationMinutes { get; set; }
    }

}
