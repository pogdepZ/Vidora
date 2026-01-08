using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vidora.Core.Entities;

namespace Vidora.Core.Contracts.Results
{
    // Dữ liệu trong record phải nằm trong ngoặc đơn (Primary Constructor) 
    // Hoặc nằm trong ngoặc nhọn { } nếu là thuộc tính bổ sung.
    public record AdminDashboardResult
    {
        // Cung cấp constructor trống
        public AdminDashboardResult() { }

        public int TotalUsers { get; set; }
        public int TotalTodayNewUsers { get; set; }
        public IReadOnlyList<Vidora.Core.Entities.User> NewUsers { get; set; } = new List<Vidora.Core.Entities.User>();
        public int TodayViews { get; set; }
        public int TotalMovies { get; set; }
        public IReadOnlyList<Vidora.Core.Entities.Movie> MostWatchedMovies { get; set; } = new List<Vidora.Core.Entities.Movie>();
        public IReadOnlyList<Vidora.Core.Entities.Movie> HighestRatedMovies { get; set; } = new List<Vidora.Core.Entities.Movie>();
        public List<double> RevenueData { get; set; } = new();

        public string MonthlyRevenue => (RevenueData?.Sum() ?? 0).ToString("N0") + " VND";
    }
}
