using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Client.Utils.Converters;

/// <summary>
/// Converts IsSelected boolean to background color for stage options
/// </summary>
public class StageOptionBackgroundConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSelected && isSelected)
        {
            return new SolidColorBrush(Color.Parse("#1F2937")); // Dark background when selected
        }
        return new SolidColorBrush(Colors.White); // White when not selected
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts IsSelected boolean to foreground color for stage options
/// </summary>
public class StageOptionForegroundConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSelected && isSelected)
        {
            return new SolidColorBrush(Colors.White); // White text when selected
        }
        return new SolidColorBrush(Color.Parse("#374151")); // Dark text when not selected
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
