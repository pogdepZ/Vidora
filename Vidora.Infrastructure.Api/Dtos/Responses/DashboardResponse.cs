using System.Collections.Generic;
using Vidora.Infrastructure.Api.Dtos.Responses.Datas;

namespace Vidora.Infrastructure.Api.Dtos.Responses;

public record DashboardResponse
{
    public int TotalUsers { get; set; }

    public int TotalTodayNewUsers { get; set; }

    public List<UserData> NewUsers { get; set; }

    public int TodayViews { get; set; }

    public int TotalMovies { get; set; }

    public List<MovieData> MostWatchedMovies { get; set; }

    public List<MovieData> HighestRatedMovies { get; set; }

    public RevenueChartDto RevenueByDayinCurrentMonth { get; set; }
}

public class RevenueChartDto
{
    public List<string> Labels { get; set; }
    public List<double> Data { get; set; }
}
