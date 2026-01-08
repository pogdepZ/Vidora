using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Vidora.Core.Entities;
using Vidora.Presentation.Gui.ViewModels;
using Windows.Storage.Pickers;

namespace Vidora.Presentation.Gui.Views;

public sealed partial class ManageMoviesPage : Page
{
    public ManageMoviesViewModel ViewModel { get; } = App.GetService<ManageMoviesViewModel>();
    public ManageMoviesPage()
    {
        InitializeComponent();
    }

    private async void OnImportExcelButtonClick(object sender, RoutedEventArgs e)
    {
        try
        {
            var picker = new FileOpenPicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeFilter.Add(".xlsx");
            picker.FileTypeFilter.Add(".xls");

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSingleFileAsync();
            if (file == null)
            {
                return;
            }

            var movies = ViewModel.ReadMoviesFromExcel(file.Path);
            await ViewModel.ImportMoviesAsync(movies);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Import Excel Error] {ex.Message}");
        }
    }

    private async void OnAddMovieButtonClick(object sender, RoutedEventArgs e)
    {
        await ViewModel.AddMovieCommand.ExecuteAsync(null);
    }

    private async void OnSearchQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        await ViewModel.SearchCommand.ExecuteAsync(null);
    }

    private async void OnViewEditClick(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement element && element.DataContext is AdminMovie movie)
        {
            await ViewModel.GetMovieDetailForEditAsync(movie.Id);
        }
    }

    private async void OnDeleteClick(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement element && element.DataContext is AdminMovie movie)
        {
            await ViewModel.ToggleDeleteCommand.ExecuteAsync(movie);
        }
    }
}
