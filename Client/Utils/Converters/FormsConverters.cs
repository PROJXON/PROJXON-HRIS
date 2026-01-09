using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Client.ViewModels;

namespace Client.Utils.Converters;

/// <summary>
/// Converts FormStatus to background color for status badge
/// </summary>
public class FormStatusBackgroundConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is FormStatus status)
        {
            return status switch
            {
                FormStatus.Active => new SolidColorBrush(Color.Parse("#DCFCE7")), // Light green
                FormStatus.Completed => new SolidColorBrush(Color.Parse("#E5E7EB")), // Light gray
                FormStatus.Draft => new SolidColorBrush(Color.Parse("#F3F4F6")), // Lighter gray
                _ => new SolidColorBrush(Color.Parse("#F3F4F6"))
            };
        }
        return new SolidColorBrush(Color.Parse("#F3F4F6"));
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts FormStatus to foreground color for status badge text
/// </summary>
public class FormStatusForegroundConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is FormStatus status)
        {
            return status switch
            {
                FormStatus.Active => new SolidColorBrush(Color.Parse("#166534")), // Dark green
                FormStatus.Completed => new SolidColorBrush(Color.Parse("#374151")), // Dark gray
                FormStatus.Draft => new SolidColorBrush(Color.Parse("#6B7280")), // Medium gray
                _ => new SolidColorBrush(Color.Parse("#6B7280"))
            };
        }
        return new SolidColorBrush(Color.Parse("#6B7280"));
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts percentage and container width to actual width for progress bar
/// </summary>
public class ProgressWidthConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count >= 2 && 
            values[0] is double percentage && 
            values[1] is double containerWidth)
        {
            // Account for the percentage text column (about 60px)
            var availableWidth = containerWidth - 60;
            if (availableWidth < 0) availableWidth = 0;
            
            return Math.Max(0, (percentage / 100.0) * availableWidth);
        }
        return 0.0;
    }
}
