using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Client.Utils.Converters;

/// <summary>
/// Multi-value converter for calendar day background based on attendance, leave, and selection state
/// </summary>
public class CalendarDayBackgroundMultiConverter : IMultiValueConverter
{
    public static readonly CalendarDayBackgroundMultiConverter Instance = new();

    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 3)
            return new SolidColorBrush(Colors.Transparent);

        var hasAttendance = values[0] as bool? ?? false;
        var isOnLeave = values[1] as bool? ?? false;
        var isSelected = values[2] as bool? ?? false;

        if (isOnLeave)
        {
            return new SolidColorBrush(Color.Parse("#E5E7EB")); // Light gray for leave
        }

        if (hasAttendance)
        {
            return isSelected 
                ? new SolidColorBrush(Color.Parse("#A7F3D0")) // Brighter green when selected
                : new SolidColorBrush(Color.Parse("#D1FAE5")); // Light green
        }

        if (isSelected)
        {
            return new SolidColorBrush(Color.Parse("#E0F2FE")); // Light blue when selected without attendance
        }

        return new SolidColorBrush(Colors.Transparent);
    }
}

/// <summary>
/// Multi-value converter for calendar day border based on today and selection state
/// </summary>
public class CalendarDayBorderMultiConverter : IMultiValueConverter
{
    public static readonly CalendarDayBorderMultiConverter Instance = new();

    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2)
            return new SolidColorBrush(Color.Parse("#E5E7EB"));

        var isToday = values[0] as bool? ?? false;
        var isSelected = values[1] as bool? ?? false;

        if (isToday)
        {
            return new SolidColorBrush(Color.Parse("#1F2937")); // Dark border for today
        }

        if (isSelected)
        {
            return new SolidColorBrush(Color.Parse("#10B981")); // Green border when selected
        }

        return new SolidColorBrush(Color.Parse("#E5E7EB")); // Default light gray
    }
}

/// <summary>
/// Multi-value converter for calendar day text foreground color
/// </summary>
public class CalendarDayForegroundMultiConverter : IMultiValueConverter
{
    public static readonly CalendarDayForegroundMultiConverter Instance = new();

    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2)
            return new SolidColorBrush(Color.Parse("#1F2937"));

        var hasAttendance = values[0] as bool? ?? false;
        var isOnLeave = values[1] as bool? ?? false;

        if (isOnLeave)
        {
            return new SolidColorBrush(Color.Parse("#6B7280")); // Gray text for leave days
        }

        if (hasAttendance)
        {
            return new SolidColorBrush(Color.Parse("#059669")); // Green text for attendance days
        }

        return new SolidColorBrush(Color.Parse("#1F2937")); // Default dark text
    }
}

/// <summary>
/// Converts calendar day state to background color
/// </summary>
public class CalendarDayBackgroundConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 4)
            return new SolidColorBrush(Colors.Transparent);

        var hasAttendance = values[0] as bool? ?? false;
        var isOnLeave = values[1] as bool? ?? false;
        var isSelected = values[2] as bool? ?? false;
        var isHovered = values[3] as bool? ?? false;

        if (isOnLeave)
        {
            // Light gray for leave days
            return isHovered 
                ? new SolidColorBrush(Color.Parse("#D1D5DB")) 
                : new SolidColorBrush(Color.Parse("#E5E7EB"));
        }

        if (hasAttendance)
        {
            // Green for attendance days - darker when hovered or selected
            if (isSelected)
                return new SolidColorBrush(Color.Parse("#86EFAC")); // Brighter green when selected
            if (isHovered)
                return new SolidColorBrush(Color.Parse("#A7F3D0")); // Medium green on hover
            return new SolidColorBrush(Color.Parse("#D1FAE5")); // Light green default
        }

        if (isSelected)
        {
            return new SolidColorBrush(Color.Parse("#D1FAE5")); // Light green when selected
        }

        // Regular day
        if (isHovered)
        {
            return new SolidColorBrush(Color.Parse("#F3F4F6")); // Light gray on hover
        }

        return new SolidColorBrush(Colors.Transparent);
    }
}

/// <summary>
/// Converts calendar day attendance state to background color (simpler version)
/// </summary>
public class AttendanceBackgroundConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool hasAttendance && hasAttendance)
        {
            return new SolidColorBrush(Color.Parse("#D1FAE5")); // Light green
        }
        return new SolidColorBrush(Colors.Transparent);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts leave state to background color
/// </summary>
public class LeaveBackgroundConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isOnLeave && isOnLeave)
        {
            return new SolidColorBrush(Color.Parse("#E5E7EB")); // Light gray
        }
        return new SolidColorBrush(Colors.Transparent);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts selected state to border brush
/// </summary>
public class SelectedBorderConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSelected && isSelected)
        {
            return new SolidColorBrush(Color.Parse("#10B981")); // Green border
        }
        return new SolidColorBrush(Color.Parse("#E5E7EB")); // Light gray border
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts today state to border brush
/// </summary>
public class TodayBorderConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isToday && isToday)
        {
            return new SolidColorBrush(Color.Parse("#1F2937")); // Dark border for today
        }
        return new SolidColorBrush(Color.Parse("#E5E7EB")); // Light gray border
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts boolean to visibility for time display
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool b && b;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Inverts boolean value
/// </summary>
public class InverseBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool b && !b;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool b && !b;
    }
}

/// <summary>
/// Converts AM/PM selection to background
/// </summary>
public class AmPmBackgroundConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var isAm = value as bool? ?? false;
        var checkingAm = parameter?.ToString() == "AM";

        if ((isAm && checkingAm) || (!isAm && !checkingAm))
        {
            return new SolidColorBrush(Color.Parse("#3B82F6")); // Blue when selected
        }
        return new SolidColorBrush(Color.Parse("#F3F4F6")); // Light gray when not selected
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts AM/PM selection to foreground
/// </summary>
public class AmPmForegroundConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var isAm = value as bool? ?? false;
        var checkingAm = parameter?.ToString() == "AM";

        if ((isAm && checkingAm) || (!isAm && !checkingAm))
        {
            return new SolidColorBrush(Colors.White); // White text when selected
        }
        return new SolidColorBrush(Color.Parse("#374151")); // Dark gray when not selected
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts hour/minute selection to background
/// </summary>
public class TimeSelectionBackgroundConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2)
            return new SolidColorBrush(Colors.Transparent);

        var currentValue = values[0] as int? ?? -1;
        var selectedValue = values[1] as int? ?? -1;

        if (currentValue == selectedValue)
        {
            return new SolidColorBrush(Color.Parse("#3B82F6")); // Blue when selected
        }
        return new SolidColorBrush(Colors.Transparent);
    }
}

/// <summary>
/// Converts hour/minute selection to foreground
/// </summary>
public class TimeSelectionForegroundConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2)
            return new SolidColorBrush(Color.Parse("#374151"));

        var currentValue = values[0] as int? ?? -1;
        var selectedValue = values[1] as int? ?? -1;

        if (currentValue == selectedValue)
        {
            return new SolidColorBrush(Colors.White); // White text when selected
        }
        return new SolidColorBrush(Color.Parse("#374151")); // Dark gray when not selected
    }
}
