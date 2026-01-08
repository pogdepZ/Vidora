using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI;

namespace Vidora.Presentation.Gui.Converters;

/// <summary>
/// Converts order status color string to background brush.
/// StatusColor values: "Success", "Warning", "Error", "Default"
/// </summary>
public class StatusToBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var statusColor = value as string ?? "Default";

        return statusColor switch
        {
            "Success" => new SolidColorBrush(Color.FromArgb(40, 16, 185, 129)),   // Green with transparency
            "Warning" => new SolidColorBrush(Color.FromArgb(40, 245, 158, 11)),   // Yellow/Orange with transparency
            "Error" => new SolidColorBrush(Color.FromArgb(40, 239, 68, 68)),      // Red with transparency
            _ => new SolidColorBrush(Color.FromArgb(40, 156, 163, 175))           // Gray with transparency
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
