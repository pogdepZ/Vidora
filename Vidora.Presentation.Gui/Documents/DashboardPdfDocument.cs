using System;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Vidora.Core.Contracts.Results;

namespace Vidora.Presentation.Gui.Documents;

/// <summary>
/// QuestPDF document for Admin Dashboard Report.
/// </summary>
public class DashboardPdfDocument : IDocument
{
    private readonly AdminDashboardResult _data;
    private readonly DateTime _exportTime;

    // Vidora brand colors
    private static readonly string VidoraRed = "#D81F26";
    private static readonly string DarkBackground = "#121212";
    private static readonly string LightGray = "#A1A1A1";

    public DashboardPdfDocument(AdminDashboardResult data)
    {
        _data = data ?? throw new ArgumentNullException(nameof(data));
        _exportTime = DateTime.Now;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public DocumentSettings GetSettings() => DocumentSettings.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(40);
            page.DefaultTextStyle(x => x.FontSize(11).FontColor(Colors.Black));

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().Element(ComposeFooter);
        });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Column(column =>
        {
            // Title
            column.Item().Background(VidoraRed).Padding(15).Row(row =>
            {
                row.RelativeItem().Text("VIDORA – ADMIN DASHBOARD REPORT")
                    .FontSize(20)
                    .Bold()
                    .FontColor(Colors.White);
            });

            // Export time
            column.Item().PaddingTop(10).Text(text =>
            {
                text.Span("Report Generated: ").SemiBold();
                text.Span(_exportTime.ToString("MM/dd/yyyy HH:mm:ss"));
            });

            column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.PaddingVertical(20).Column(column =>
        {
            // 1. Summary Statistics Section
            column.Item().Element(ComposeSummaryStats);

            column.Item().PaddingVertical(15).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

            // 2. Top Movies Table
            column.Item().Element(ComposeTopMoviesTable);

            column.Item().PaddingVertical(15).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

            // 3. Recent Signups Table
            column.Item().Element(ComposeRecentSignupsTable);
        });
    }

    private void ComposeSummaryStats(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Text("OVERVIEW STATISTICS").FontSize(14).Bold().FontColor(VidoraRed);
            column.Item().PaddingTop(10);

            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(8)
                        .Text("Total Users").Bold().AlignCenter();
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(8)
                        .Text("Total Movies").Bold().AlignCenter();
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(8)
                        .Text("Today's Views").Bold().AlignCenter();
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(8)
                        .Text("Monthly Revenue").Bold().AlignCenter();
                });

                // Data row
                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10)
                    .Text(_data.TotalUsers.ToString("N0")).FontSize(16).Bold().AlignCenter();
                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10)
                    .Text(_data.TotalMovies.ToString("N0")).FontSize(16).Bold().AlignCenter();
                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10)
                    .Text(_data.TodayViews.ToString("N0")).FontSize(16).Bold().AlignCenter();
                table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10)
                    .Text(_data.MonthlyRevenue).FontSize(16).Bold().AlignCenter();
            });
        });
    }

    private void ComposeTopMoviesTable(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Text("TOP MOST WATCHED MOVIES").FontSize(14).Bold().FontColor(VidoraRed);
            column.Item().PaddingTop(10);

            if (_data.MostWatchedMovies == null || !_data.MostWatchedMovies.Any())
            {
                column.Item().Text("No data available").Italic().FontColor(LightGray);
                return;
            }

            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(40);  // #
                    columns.RelativeColumn(3);   // Title
                    columns.RelativeColumn(2);   // Genre
                    columns.ConstantColumn(60);  // Year
                    columns.ConstantColumn(60);  // Rating
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Background(VidoraRed).Padding(8)
                        .Text("#").Bold().FontColor(Colors.White).AlignCenter();
                    header.Cell().Background(VidoraRed).Padding(8)
                        .Text("Title").Bold().FontColor(Colors.White);
                    header.Cell().Background(VidoraRed).Padding(8)
                        .Text("Genre").Bold().FontColor(Colors.White);
                    header.Cell().Background(VidoraRed).Padding(8)
                        .Text("Year").Bold().FontColor(Colors.White).AlignCenter();
                    header.Cell().Background(VidoraRed).Padding(8)
                        .Text("Rating").Bold().FontColor(Colors.White).AlignCenter();
                });

                // Data rows
                var index = 1;
                foreach (var movie in _data.MostWatchedMovies.Take(10))
                {
                    var bgColor = index % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;

                    table.Cell().Background(bgColor).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(6)
                        .Text(index.ToString()).AlignCenter();
                    table.Cell().Background(bgColor).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(6)
                        .Text(movie.Title);
                    table.Cell().Background(bgColor).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(6)
                        .Text(string.Join(", ", movie.Genres ?? new System.Collections.Generic.List<string>()));
                    table.Cell().Background(bgColor).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(6)
                        .Text(movie.ReleaseYear.ToString()).AlignCenter();
                    table.Cell().Background(bgColor).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(6)
                        .Text($"{movie.Rating:F1}").AlignCenter();

                    index++;
                }
            });
        });
    }

    private void ComposeRecentSignupsTable(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Text("RECENT USER SIGNUPS").FontSize(14).Bold().FontColor(VidoraRed);
            column.Item().PaddingTop(5).Text($"New signups today: {_data.TotalTodayNewUsers}")
                .FontSize(10).FontColor(LightGray);
            column.Item().PaddingTop(10);

            if (_data.NewUsers == null || !_data.NewUsers.Any())
            {
                column.Item().Text("No data available").Italic().FontColor(LightGray);
                return;
            }

            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(40);  // #
                    columns.RelativeColumn(2);   // Full Name
                    columns.RelativeColumn(3);   // Email
                    columns.RelativeColumn(2);   // Created Date
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Background(VidoraRed).Padding(8)
                        .Text("#").Bold().FontColor(Colors.White).AlignCenter();
                    header.Cell().Background(VidoraRed).Padding(8)
                        .Text("Full Name").Bold().FontColor(Colors.White);
                    header.Cell().Background(VidoraRed).Padding(8)
                        .Text("Email").Bold().FontColor(Colors.White);
                    header.Cell().Background(VidoraRed).Padding(8)
                        .Text("Signup Date").Bold().FontColor(Colors.White).AlignCenter();
                });

                // Data rows
                var index = 1;
                foreach (var user in _data.NewUsers.Take(10))
                {
                    var bgColor = index % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;

                    table.Cell().Background(bgColor).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(6)
                        .Text(index.ToString()).AlignCenter();
                    table.Cell().Background(bgColor).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(6)
                        .Text(user.FullName);
                    table.Cell().Background(bgColor).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(6)
                        .Text(user.Email.ToString());
                    table.Cell().Background(bgColor).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(6)
                        .Text(user.CreatedAt?.ToString("MM/dd/yyyy") ?? "N/A").AlignCenter();

                    index++;
                }
            });
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
            column.Item().PaddingTop(10).Row(row =>
            {
                row.RelativeItem().Text(text =>
                {
                    text.Span("© Vidora Admin Dashboard - ");
                    text.Span(_exportTime.Year.ToString());
                });

                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.Span("Page ");
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
            });
        });
    }
}
