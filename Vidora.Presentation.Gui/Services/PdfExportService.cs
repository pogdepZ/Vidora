using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Vidora.Core.Contracts.Results;
using Vidora.Presentation.Gui.Contracts.Services;
using Vidora.Presentation.Gui.Documents;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Vidora.Presentation.Gui.Services;

/// <summary>
/// Service for exporting data to PDF files using QuestPDF.
/// </summary>
public class PdfExportService : IPdfExportService
{
    static PdfExportService()
    {
        // Set QuestPDF license type - Community license for open source/personal projects
        QuestPDF.Settings.License = LicenseType.Community;
    }

    /// <inheritdoc />
    public async Task<bool> ExportDashboardToPdfAsync(AdminDashboardResult data)
    {
        if (data == null)
        {
            return false;
        }

        try
        {
            // Create FileSavePicker
            var savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = $"Vidora_Dashboard_Report_{DateTime.Now:yyyyMMdd_HHmmss}"
            };
            savePicker.FileTypeChoices.Add("PDF Document", new[] { ".pdf" });

            // Get window handle for WinUI 3
            var window = App.MainWindow;
            if (window == null)
            {
                return false;
            }

            var hwnd = WindowNative.GetWindowHandle(window);
            InitializeWithWindow.Initialize(savePicker, hwnd);

            // Show picker
            var file = await savePicker.PickSaveFileAsync();
            if (file == null)
            {
                // User cancelled
                return false;
            }

            // Generate PDF document
            var document = new DashboardPdfDocument(data);

            // Generate PDF bytes
            var pdfBytes = document.GeneratePdf();

            // Write to file using Windows Storage API
            await FileIO.WriteBytesAsync(file, pdfBytes);

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[PdfExportService] Error exporting PDF: {ex.Message}");
            return false;
        }
    }
}
