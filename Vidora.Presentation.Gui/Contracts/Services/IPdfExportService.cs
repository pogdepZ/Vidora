using System.Threading.Tasks;
using Vidora.Core.Contracts.Results;

namespace Vidora.Presentation.Gui.Contracts.Services;

/// <summary>
/// Service for exporting data to PDF files.
/// </summary>
public interface IPdfExportService
{
    /// <summary>
    /// Exports the admin dashboard data to a PDF file.
    /// Shows a FileSavePicker dialog to let the user choose the save location.
    /// </summary>
    /// <param name="data">The dashboard data to export.</param>
    /// <returns>True if export was successful, false if cancelled or failed.</returns>
    Task<bool> ExportDashboardToPdfAsync(AdminDashboardResult data);
}
