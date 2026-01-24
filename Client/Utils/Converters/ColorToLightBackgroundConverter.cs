using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Client.Utils.Converters;

/// <summary>
/// Converts a status color to a lighter background color
/// Example: #22C55E (green) -> #DCFCE7 (light green)
/// </summary>
public class ColorToLightBackgroundConverter : IValueConverter
{
    public static readonly ColorToLightBackgroundConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string colorString)
            return new SolidColorBrush(Colors.Transparent);

        // Map status colors to their light background equivalents
        return colorString switch
        {
            "#22C55E" => new SolidColorBrush(Color.Parse("#DCFCE7")), // Green -> Light green
            "#EF4444" => new SolidColorBrush(Color.Parse("#FEE2E2")), // Red -> Light red
            "#F59E0B" => new SolidColorBrush(Color.Parse("#FEF3C7")), // Amber -> Light amber
            _ => new SolidColorBrush(Colors.LightGray) // Fallback
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}