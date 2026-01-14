using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Client.ViewModels;

namespace Client.Utils.Converters;

/// <summary>
/// Converts RequestStatus to background color for status badge
/// </summary>
public class RequestStatusBackgroundConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is RequestStatus status)
        {
            return status switch
            {
                RequestStatus.Pending => new SolidColorBrush(Color.Parse("#EDE9FE")), // Light purple
                RequestStatus.Approved => new SolidColorBrush(Color.Parse("#1F2937")), // Dark gray/black
                RequestStatus.Declined => new SolidColorBrush(Color.Parse("#DC2626")), // Red
                _ => new SolidColorBrush(Colors.Gray)
            };
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts RequestStatus to foreground color for status badge text
/// </summary>
public class RequestStatusForegroundConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is RequestStatus status)
        {
            return status switch
            {
                RequestStatus.Pending => new SolidColorBrush(Color.Parse("#7C3AED")), // Purple text
                RequestStatus.Approved => new SolidColorBrush(Colors.White), // White text
                RequestStatus.Declined => new SolidColorBrush(Colors.White), // White text
                _ => new SolidColorBrush(Colors.Black)
            };
        }
        return new SolidColorBrush(Colors.Black);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}