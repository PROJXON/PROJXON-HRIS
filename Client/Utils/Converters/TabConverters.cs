using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Client.Utils.Converters;

/// <summary>
/// Converts a boolean (isSelected) to the appropriate tab background color
/// </summary>
public class BoolToTabBackgroundConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSelected && isSelected)
        {
            return new SolidColorBrush(Color.Parse("#FFFFFF")); // White for selected
        }
        return new SolidColorBrush(Color.Parse("#00000000")); // Transparent for unselected
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a boolean (isSelected) to the appropriate tab foreground color
/// </summary>
public class BoolToTabForegroundConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSelected && isSelected)
        {
            return new SolidColorBrush(Color.Parse("#1F2937")); // Dark for selected
        }
        return new SolidColorBrush(Color.Parse("#6B7280")); // Gray for unselected
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a boolean (isSelected) to the appropriate alphabet filter background color
/// </summary>
public class AlphabetFilterBackgroundConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSelected && isSelected)
        {
            return new SolidColorBrush(Color.Parse("#1F2937")); // Dark for selected
        }
        return new SolidColorBrush(Color.Parse("#FFFFFF")); // White for unselected
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a boolean (isSelected) to the appropriate alphabet filter foreground color
/// </summary>
public class AlphabetFilterForegroundConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSelected && isSelected)
        {
            return new SolidColorBrush(Color.Parse("#FFFFFF")); // White for selected
        }
        return new SolidColorBrush(Color.Parse("#374151")); // Dark gray for unselected
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a boolean (isSelected) to the appropriate alphabet filter border color
/// </summary>
public class AlphabetFilterBorderConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSelected && isSelected)
        {
            return new SolidColorBrush(Color.Parse("#1F2937")); // Same as background when selected
        }
        return new SolidColorBrush(Color.Parse("#E5E7EB")); // Light gray border for unselected
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
