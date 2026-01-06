using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vidora.Infrastructure.Api.Dtos.Requests
{
    public class CreateMovieRequest
    {
        public string Title { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
        public string DirectorName { get; set; } = string.Empty;

        // Chứa tên các thể loại/diễn viên mới hoặc ID của các mục đã có
        public List<string> Genres { get; set; } = new();
        public List<string> Actors { get; set; } = new();
    }
}
