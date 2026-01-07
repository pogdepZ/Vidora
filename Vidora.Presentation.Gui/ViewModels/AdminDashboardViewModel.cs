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
    private readonly GetDashboardStatsUseCase _getStatsUseCase;
    private readonly IPdfExportService _pdfExportService;
    private readonly IInfoBarService _infoBarService;

    [ObservableProperty] private AdminDashboardResult? _stats;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isExporting;

    public string CurrentDate => DateTime.Now.ToString("dd MMMM, yyyy");

    public AdminDashboardViewModel(
        GetDashboardStatsUseCase getStatsUseCase,
        IPdfExportService pdfExportService,
        IInfoBarService infoBarService)
    {
        _getStatsUseCase = getStatsUseCase;
        _pdfExportService = pdfExportService;
        _infoBarService = infoBarService;
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
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"[API Check] failed");
        }
        IsLoading = false;
    }

    public async Task OnNavigatedFromAsync()
    {

    }

    /// <summary>
    /// Command to export dashboard data to PDF.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExportToPdf))]
    private async Task ExportToPdfAsync()
    {
        if (Stats == null)
        {
            _infoBarService.ShowError("No data available to export.");
            return;
        }

        IsExporting = true;
        try
        {
            var success = await _pdfExportService.ExportDashboardToPdfAsync(Stats);
            if (success)
            {
                _infoBarService.ShowSuccess("PDF report exported successfully!");
            }
            else
            {
                // User cancelled or export failed silently
                System.Diagnostics.Debug.WriteLine("[ExportToPdf] Export cancelled or failed");
            }
        }
        catch (Exception ex)
        {
            _infoBarService.ShowError($"Error exporting PDF: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"[ExportToPdf] Error: {ex}");
        }
        finally
        {
            IsExporting = false;
        }
    }

    private bool CanExportToPdf() => Stats != null && !IsExporting && !IsLoading;
}
