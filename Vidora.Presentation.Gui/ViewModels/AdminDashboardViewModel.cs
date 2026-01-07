using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vidora.Core.Contracts.Commands;
using Vidora.Core.Contracts.Results;
using Vidora.Core.Contracts.Services;
using Vidora.Core.UseCases;
using Vidora.Presentation.Gui.Contracts.Services;
using Vidora.Presentation.Gui.Contracts.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Vidora.Presentation.Gui.ViewModels;

public partial class AdminDashboardViewModel : ObservableRecipient, INavigationAware
{

    public async Task OnNavigatedFromAsync()
    {

    }

    private readonly GetDashboardStatsUseCase _getStatsUseCase;

    [ObservableProperty] private AdminDashboardResult? _stats;
    [ObservableProperty] private bool _isLoading;
    public string CurrentDate => DateTime.Now.ToString("dd MMMM, yyyy");
    public AdminDashboardViewModel(GetDashboardStatsUseCase getStatsUseCase)
    {
        _getStatsUseCase = getStatsUseCase;
    }

    public async Task OnNavigatedToAsync(object parameter)
    {
        IsLoading = true;
        var result = await _getStatsUseCase.ExecuteAsync();
        if (result.IsSuccess)
        {
            Stats = result.Value;
            var jsonDebug = System.Text.Json.JsonSerializer.Serialize(Stats, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            System.Diagnostics.Debug.WriteLine("===== API DATA START =====");
            System.Diagnostics.Debug.WriteLine(jsonDebug);
            System.Diagnostics.Debug.WriteLine("===== API DATA END =====");
            //if (Stats.RevenueData == null || Stats.RevenueData.Count == 0 || Stats.RevenueData.Sum() == 0)
            //{
            //    // Tạo mảng giả lập 30 ngày nhấp nhô
            //    var random = new Random();
            //    var fakeRevenue = new List<double>();
            //    double lastValue = 500000; // Giá trị khởi điểm (500k VND)

            //    for (int i = 0; i < 30; i++)
            //    {
            //        // Tạo biến động từ -20% đến +30% so với ngày trước đó
            //        double change = lastValue * (random.NextDouble() * 0.5 - 0.2);
            //        lastValue = Math.Max(100000, lastValue + change); // Đảm bảo không dưới 100k
            //        fakeRevenue.Add(lastValue);
            //    }

            //    // Gán lại dữ liệu giả lập vào Stats (Dùng record 'with' expression)
            //    Stats = Stats with { RevenueData = fakeRevenue };
            //}
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"[API Check] failed");
        }
            IsLoading = false;
    }
}
