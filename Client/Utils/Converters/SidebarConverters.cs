using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Client.Utils.Converters;

public class SidebarBackgroundConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string currentPage && parameter is string targetPage)
        {
            if (string.Equals(currentPage, targetPage, StringComparison.OrdinalIgnoreCase))
            {
                return new SolidColorBrush(Color.Parse("#1F2937"));
            }
        }
        return Brushes.Transparent;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class SidebarForegroundConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string currentPage && parameter is string targetPage)
        {
            if (string.Equals(currentPage, targetPage, StringComparison.OrdinalIgnoreCase))
            {
                return Brushes.White;
            }
        }
        return new SolidColorBrush(Color.Parse("#374151"));
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class SidebarIconStrokeConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string currentPage && parameter is string targetPage)
        {
            if (string.Equals(currentPage, targetPage, StringComparison.OrdinalIgnoreCase))
            {
                return Brushes.White;
            }
        }
        return new SolidColorBrush(Color.Parse("#374151"));
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
