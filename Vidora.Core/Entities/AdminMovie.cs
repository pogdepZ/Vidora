using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vidora.Core.Entities
{
    public class AdminMovie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
        public List<string> Genres { get; set; } = new();

        public string GenresText => string.Join(", ", Genres);

        public string DirectorName { get; set; } = "N/A";
    }


}
