using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace Vidora.Presentation.Gui.Converters;

/// <summary>
/// Converts an integer to Visibility.
/// 0 or negative = Collapsed, positive = Visible
/// </summary>
public class IntToVisibilityConverter : IValueConverter
{
    public bool Invert { get; set; } = false;

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var count = value is int i ? i : 0;
        var isVisible = count > 0;

        if (Invert)
            isVisible = !isVisible;

        return isVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
