using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using Vidora.Core.Entities;
using Vidora.Core.Contracts.Results;
using Vidora.Presentation.Gui.ViewModels;

namespace Vidora.Presentation.Gui.Views;

public sealed partial class ManageMoviesPage : Page
{
    public ManageMoviesViewModel ViewModel { get; } = App.GetService<ManageMoviesViewModel>();
    public ManageMoviesPage()
    {
        InitializeComponent();
    }

    private int _notificationId = 0;

    /// <summary>
    /// Hiển thị thông báo InfoBar
    /// </summary>
    private async void ShowNotification(string title, string message, InfoBarSeverity severity = InfoBarSeverity.Success, int autoHideSeconds = 3)
    {
        NotificationInfoBar.Title = title;
        NotificationInfoBar.Message = message;
        NotificationInfoBar.Severity = severity;
        NotificationInfoBar.IsOpen = true;

        int currentId = ++_notificationId;

        // Tự động ẩn sau vài giây
        if (autoHideSeconds > 0)
        {
            await Task.Delay(autoHideSeconds * 1000);
            if (currentId == _notificationId)
            {
                NotificationInfoBar.IsOpen = false;
            }
        }
    }

    private async void OnAddMovieButtonClick(object sender, RoutedEventArgs e)
    {
        // 1. Khởi tạo stackPanel CHÍNH trước tiên
        var stackPanel = new StackPanel { Width = 500, Padding = new Thickness(0, 0, 15, 0) };

        // 2. Tải dữ liệu từ API
        await ViewModel.LoadAvailableGenresAsync();
        await ViewModel.LoadAvailableMembersAsync();

        // QUAN TRỌNG: Clear dữ liệu cũ khi mở dialog mới
        ViewModel.SelectedGenres.Clear();
        ViewModel.SelectedCast.Clear();

        // 3. Tạo các thành phần nhập liệu cơ bản
        var titleBox = new TextBox { Header = "Tiêu đề phim", Margin = new Thickness(0, 0, 0, 10) };
        var descBox = new TextBox { Header = "Mô tả phim", AcceptsReturn = true, Height = 60, Margin = new Thickness(0, 0, 0, 10) };
        var posterBox = new TextBox { Header = "Link Poster", Margin = new Thickness(0, 0, 0, 10) };
        var bannerBox = new TextBox { Header = "Link Banner", Margin = new Thickness(0, 0, 0, 10) };
        var trailerBox = new TextBox { Header = "Link Trailer", Margin = new Thickness(0, 0, 0, 10) };
        var movieUrlBox = new TextBox { Header = "Link Phim (Stream)", Margin = new Thickness(0, 0, 0, 10) };
        var yearBox = new NumberBox { Header = "Năm phát hành", Value = DateTime.Now.Year, Margin = new Thickness(0, 0, 0, 10) };

        // Thêm các thành phần cơ bản vào stackPanel
        stackPanel.Children.Add(titleBox);
        stackPanel.Children.Add(descBox);
        stackPanel.Children.Add(posterBox);
        stackPanel.Children.Add(bannerBox);
        stackPanel.Children.Add(trailerBox);
        stackPanel.Children.Add(movieUrlBox);
        stackPanel.Children.Add(yearBox);

        // 4. PHẦN THỂ LOẠI
        var genreHeader = new TextBlock { Text = "Chọn thể loại", Margin = new Thickness(0, 10, 0, 5), FontWeight = Microsoft.UI.Text.FontWeights.SemiBold };
        var genreContainer = new StackPanel { Margin = new Thickness(5, 0, 0, 15) };

        if (ViewModel.AvailableGenres != null)
        {
            foreach (var genre in ViewModel.AvailableGenres)
            {
                var cb = new CheckBox { Content = genre.Name, Tag = genre, Margin = new Thickness(0, 0, 0, 5) };
                cb.Checked += (s, arg) => {
                    if (s is CheckBox b && b.Tag is Genre g)
                        if (!ViewModel.SelectedGenres.Contains(g)) ViewModel.SelectedGenres.Add(g);
                };
                cb.Unchecked += (s, arg) => {
                    if (s is CheckBox b && b.Tag is Genre g)
                        ViewModel.SelectedGenres.Remove(g);
                };
                genreContainer.Children.Add(cb);
            }
        }
        stackPanel.Children.Add(genreHeader);
        stackPanel.Children.Add(genreContainer);

        // --- PHẦN CAST & CREW ---
        var selectedCastList = new StackPanel { Margin = new Thickness(5, 5, 0, 15), Spacing = 4 };
        BuildCastAndCrewSection(stackPanel, selectedCastList, ViewModel.SelectedCast);

        // 6. Thiết lập Binding
        titleBox.SetBinding(TextBox.TextProperty, new Binding { Source = ViewModel, Path = new PropertyPath("NewMovieTitle"), Mode = BindingMode.TwoWay });
        descBox.SetBinding(TextBox.TextProperty, new Binding { Source = ViewModel, Path = new PropertyPath("NewMovieDescription"), Mode = BindingMode.TwoWay });
        posterBox.SetBinding(TextBox.TextProperty, new Binding { Source = ViewModel, Path = new PropertyPath("NewMoviePosterUrl"), Mode = BindingMode.TwoWay });
        bannerBox.SetBinding(TextBox.TextProperty, new Binding { Source = ViewModel, Path = new PropertyPath("NewMovieBannerUrl"), Mode = BindingMode.TwoWay });
        trailerBox.SetBinding(TextBox.TextProperty, new Binding { Source = ViewModel, Path = new PropertyPath("NewMovieTrailerUrl"), Mode = BindingMode.TwoWay });
        movieUrlBox.SetBinding(TextBox.TextProperty, new Binding { Source = ViewModel, Path = new PropertyPath("NewMovieUrl"), Mode = BindingMode.TwoWay });
        yearBox.SetBinding(NumberBox.ValueProperty, new Binding { Source = ViewModel, Path = new PropertyPath("NewMovieReleaseYear"), Mode = BindingMode.TwoWay });

        // 7. Hiển thị Dialog
        var scrollViewer = new ScrollViewer { Content = stackPanel, MaxHeight = 600, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };

        ContentDialog dialog = new ContentDialog
        {
            Title = "Thêm phim mới",
            Content = scrollViewer,
            PrimaryButtonText = "Lưu phim",
            CloseButtonText = "Hủy",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = this.Content.XamlRoot
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            try
            {
                // Giả sử AddMovieCommand trả về một kết quả (Result)
                await ViewModel.AddMovieCommand.ExecuteAsync(null);

                if (ViewModel.IsSuccess) // Bạn nên có thuộc tính này trong ViewModel
                {
                    ShowNotification("Thành công", "Đã thêm phim mới vào hệ thống.", InfoBarSeverity.Success);
                }
                else
                {
                    // Thông báo lỗi nếu API trả về thất bại (ví dụ: trùng tên, link phim hỏng)
                    ShowNotification("Lỗi nghiệp vụ", ViewModel.ErrorMessage ?? "Không thể lưu phim.", InfoBarSeverity.Warning);
                }
            }
            catch (Exception ex)
            {
                // Thông báo lỗi nghiêm trọng (ví dụ: mất mạng, lỗi server 500)
                ShowNotification("Lỗi hệ thống", $"Đã xảy ra lỗi: {ex.Message}", InfoBarSeverity.Error, 5);
            }
        }
    }

    private async void OnViewEditClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn || btn.DataContext is not AdminMovie movie)
            return;

        // 1. Load movie detail and available data
        await ViewModel.LoadAvailableGenresAsync();
        await ViewModel.LoadAvailableMembersAsync();
        var movieDetail = await ViewModel.GetMovieDetailForEditAsync(movie.Id);

        if (movieDetail == null)
        {
            ShowNotification("Lỗi", "Không thể tải thông tin chi tiết phim.", InfoBarSeverity.Error);
            return;
        }

        // Clear and populate edit collections
        ViewModel.EditSelectedGenres.Clear();
        ViewModel.EditSelectedCast.Clear();

        // 2. Build the edit dialog UI
        var stackPanel = new StackPanel { Width = 500, Padding = new Thickness(0, 0, 15, 0) };

        // Basic info fields
        var titleBox = new TextBox { Header = "Tiêu đề phim", Text = ViewModel.EditMovieTitle, Margin = new Thickness(0, 0, 0, 10) };
        var descBox = new TextBox { Header = "Mô tả phim", Text = ViewModel.EditMovieDescription, AcceptsReturn = true, Height = 60, Margin = new Thickness(0, 0, 0, 10) };
        var posterBox = new TextBox { Header = "Link Poster", Text = ViewModel.EditMoviePosterUrl, Margin = new Thickness(0, 0, 0, 10) };
        var bannerBox = new TextBox { Header = "Link Banner", Text = ViewModel.EditMovieBannerUrl, Margin = new Thickness(0, 0, 0, 10) };
        var trailerBox = new TextBox { Header = "Link Trailer", Text = ViewModel.EditMovieTrailerUrl, Margin = new Thickness(0, 0, 0, 10) };
        var movieUrlBox = new TextBox { Header = "Link Phim (Stream)", Text = ViewModel.EditMovieUrl, Margin = new Thickness(0, 0, 0, 10) };
        var yearBox = new NumberBox { Header = "Năm phát hành", Value = ViewModel.EditMovieReleaseYear, Margin = new Thickness(0, 0, 0, 10) };

        stackPanel.Children.Add(titleBox);
        stackPanel.Children.Add(descBox);
        stackPanel.Children.Add(posterBox);
        stackPanel.Children.Add(bannerBox);
        stackPanel.Children.Add(trailerBox);
        stackPanel.Children.Add(movieUrlBox);
        stackPanel.Children.Add(yearBox);

        // Genre section with pre-selected genres
        var genreHeader = new TextBlock { Text = "Chọn thể loại", Margin = new Thickness(0, 10, 0, 5), FontWeight = Microsoft.UI.Text.FontWeights.SemiBold };
        var genreContainer = new StackPanel { Margin = new Thickness(5, 0, 0, 15) };

        if (ViewModel.AvailableGenres != null)
        {
            foreach (var genre in ViewModel.AvailableGenres)
            {
                var isChecked = movieDetail.Genres?.Contains(genre.Name) ?? false;
                var cb = new CheckBox { Content = genre.Name, Tag = genre, IsChecked = isChecked, Margin = new Thickness(0, 0, 0, 5) };
                
                // Pre-add checked genres to EditSelectedGenres
                if (isChecked)
                {
                    ViewModel.EditSelectedGenres.Add(genre);
                }

                cb.Checked += (s, arg) => {
                    if (s is CheckBox b && b.Tag is Genre g)
                        if (!ViewModel.EditSelectedGenres.Contains(g)) ViewModel.EditSelectedGenres.Add(g);
                };
                cb.Unchecked += (s, arg) => {
                    if (s is CheckBox b && b.Tag is Genre g)
                        ViewModel.EditSelectedGenres.Remove(g);
                };
                genreContainer.Children.Add(cb);
            }
        }
        stackPanel.Children.Add(genreHeader);
        stackPanel.Children.Add(genreContainer);

        // Cast & Crew section with pre-populated data
        var selectedCastList = new StackPanel { Margin = new Thickness(5, 5, 0, 15), Spacing = 4 };
        BuildCastAndCrewSection(stackPanel, selectedCastList, ViewModel.EditSelectedCast);

        // Pre-populate existing cast
        if (movieDetail.Actors != null)
        {
            foreach (var actor in movieDetail.Actors)
            {
                var member = ViewModel.AvailableMembers.FirstOrDefault(m => m.Id == actor.MemberId);
                if (member != null)
                {
                    ViewModel.EditSelectedCast.Add((member, actor.Role));
                    AddCastRowToUI(selectedCastList, member, actor.Role, ViewModel.EditSelectedCast);
                }
            }
        }

        // Bindings for edit properties
        titleBox.SetBinding(TextBox.TextProperty, new Binding { Source = ViewModel, Path = new PropertyPath("EditMovieTitle"), Mode = BindingMode.TwoWay });
        descBox.SetBinding(TextBox.TextProperty, new Binding { Source = ViewModel, Path = new PropertyPath("EditMovieDescription"), Mode = BindingMode.TwoWay });
        posterBox.SetBinding(TextBox.TextProperty, new Binding { Source = ViewModel, Path = new PropertyPath("EditMoviePosterUrl"), Mode = BindingMode.TwoWay });
        bannerBox.SetBinding(TextBox.TextProperty, new Binding { Source = ViewModel, Path = new PropertyPath("EditMovieBannerUrl"), Mode = BindingMode.TwoWay });
        trailerBox.SetBinding(TextBox.TextProperty, new Binding { Source = ViewModel, Path = new PropertyPath("EditMovieTrailerUrl"), Mode = BindingMode.TwoWay });
        movieUrlBox.SetBinding(TextBox.TextProperty, new Binding { Source = ViewModel, Path = new PropertyPath("EditMovieUrl"), Mode = BindingMode.TwoWay });
        yearBox.SetBinding(NumberBox.ValueProperty, new Binding { Source = ViewModel, Path = new PropertyPath("EditMovieReleaseYear"), Mode = BindingMode.TwoWay });

        var scrollViewer = new ScrollViewer { Content = stackPanel, MaxHeight = 600, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };

        // Lưu tên phim để hiển thị thông báo
        var movieTitle = movie.Title;

        ContentDialog dialog = new ContentDialog
        {
            Title = $"Chi tiết phim: {movieTitle}",
            Content = scrollViewer,
            PrimaryButtonText = "Lưu thay đổi",
            CloseButtonText = "Hủy",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = this.Content.XamlRoot
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            if (ViewModel.UpdateMovieCommand.CanExecute(null))
            {
                await ViewModel.UpdateMovieCommand.ExecuteAsync(null);
                
                // Kiểm tra kết quả trước khi hiển thị thông báo
                if (ViewModel.IsSuccess)
                {
                    ShowNotification("Thành công", $"Đã cập nhật phim '{movieTitle}' thành công!", InfoBarSeverity.Success);
                }
                else
                {
                    ShowNotification("Lỗi", ViewModel.ErrorMessage ?? "Không thể cập nhật phim.", InfoBarSeverity.Error);
                }
            }
        }
    }

    private void BuildCastAndCrewSection(StackPanel stackPanel, StackPanel selectedCastList, System.Collections.ObjectModel.ObservableCollection<(MemberResult Member, string Role)> targetCastCollection)
    {
        var castHeader = new TextBlock
        {
            Text = "Diễn viên & Đoàn làm phim",
            Margin = new Thickness(0, 10, 0, 5),
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
        };

        var memberCombo = new ComboBox
        {
            Header = "1. Chọn Nghệ sĩ",
            PlaceholderText = "Chọn nghệ sĩ...",
            ItemsSource = ViewModel.AvailableMembers,
            DisplayMemberPath = "Name",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(0, 0, 0, 10)
        };

        var roleCombo = new ComboBox
        {
            Header = "2. Chọn Vai trò",
            ItemsSource = new string[] { "Director", "Actor", "Producer" },
            SelectedIndex = 1,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(0, 0, 0, 10)
        };

        var addButton = new Button
        {
            Content = "Thêm",
            Style = (Style)Application.Current.Resources["AccentButtonStyle"],
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 0, 0, 10)
        };

        addButton.Click += (s, arg) =>
        {
            var selectedMember = memberCombo.SelectedItem as MemberResult;
            var selectedRole = roleCombo.SelectedItem as string;

            if (selectedMember != null && !string.IsNullOrEmpty(selectedRole))
            {
                targetCastCollection.Add((selectedMember, selectedRole));
                AddCastRowToUI(selectedCastList, selectedMember, selectedRole, targetCastCollection);
                memberCombo.SelectedIndex = -1;
            }
        };

        var selectedCastScrollViewer = new ScrollViewer
        {
            Content = selectedCastList,
            MaxHeight = 150,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        };

        stackPanel.Children.Add(castHeader);
        stackPanel.Children.Add(memberCombo);
        stackPanel.Children.Add(roleCombo);
        stackPanel.Children.Add(addButton);
        stackPanel.Children.Add(selectedCastScrollViewer);
    }

    private void AddCastRowToUI(StackPanel selectedCastList, MemberResult member, string role, System.Collections.ObjectModel.ObservableCollection<(MemberResult Member, string Role)> targetCastCollection)
    {
        var row = new Grid { Margin = new Thickness(0, 2, 0, 2) };
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var roleBadge = new Border
        {
            Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightGray),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(6, 2, 6, 2),
            Margin = new Thickness(0, 0, 10, 0),
            Child = new TextBlock { Text = role, FontSize = 11, FontWeight = Microsoft.UI.Text.FontWeights.Bold }
        };

        var nameTxt = new TextBlock { Text = member.Name, VerticalAlignment = VerticalAlignment.Center };

        var removeBtn = new Button { Content = "X", FontSize = 10, Padding = new Thickness(5, 2, 5, 2) };
        removeBtn.Click += (btnSender, btnArgs) =>
        {
            var itemToRemove = targetCastCollection.FirstOrDefault(x => x.Member.Id == member.Id && x.Role == role);
            if (itemToRemove.Member != null)
            {
                targetCastCollection.Remove(itemToRemove);
            }
            selectedCastList.Children.Remove(row);
        };

        row.Children.Add(roleBadge);
        row.Children.Add(nameTxt);
        row.Children.Add(removeBtn);
        Grid.SetColumn(nameTxt, 1);
        Grid.SetColumn(removeBtn, 2);

        selectedCastList.Children.Add(row);
    }

    private async void OnDeleteClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is AdminMovie movie)
        {
            ContentDialog deleteDialog = new ContentDialog
            {
                Title = "Xác nhận xóa",
                Content = $"Bạn có chắc muốn xóa phim '{movie.Title}' không?",
                PrimaryButtonText = "Xóa",
                CloseButtonText = "Hủy",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.Content.XamlRoot
            };

            var result = await deleteDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var movieTitle = movie.Title;
                await ViewModel.ToggleDeleteCommand.ExecuteAsync(movie);
                
                // Kiểm tra kết quả trước khi hiển thị thông báo
                if (ViewModel.IsSuccess)
                {
                    ShowNotification("Thành công", $"Đã xóa phim '{movieTitle}' thành công!", InfoBarSeverity.Success);
                }
                else
                {
                    ShowNotification("Lỗi", ViewModel.ErrorMessage ?? "Không thể xóa phim.", InfoBarSeverity.Error);
                }
            }
        }
    }

    private async void OnSearchQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (ViewModel.SearchCommand.CanExecute(null))
        {
            await ViewModel.SearchCommand.ExecuteAsync(null);
        }
    }
}
