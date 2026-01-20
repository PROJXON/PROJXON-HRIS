using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Client.Utils.Converters;

/// <summary>
/// Converts a boolean value to a color - green for success, red for failure.
/// Used primarily for feedback messages in dialogs.
/// </summary>
public class BoolToColorConverter : IValueConverter
{
    /// <summary>
    /// The color to use when the value is true (success).
    /// Default: Green (#22C55E)
    /// </summary>
    public IBrush SuccessColor { get; set; } = new SolidColorBrush(Color.Parse("#22C55E"));
    
    /// <summary>
    /// The color to use when the value is false (failure/error).
    /// Default: Red (#EF4444)
    /// </summary>
    public IBrush FailureColor { get; set; } = new SolidColorBrush(Color.Parse("#EF4444"));

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSuccess)
        {
            return isSuccess ? SuccessColor : FailureColor;
        }
        
        return FailureColor;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a boolean to a background color for status indicators.
/// True = light green background, False = light red background.
/// </summary>
public class BoolToBackgroundColorConverter : IValueConverter
{
    public IBrush SuccessBackground { get; set; } = new SolidColorBrush(Color.Parse("#DCFCE7")); // Light green
    public IBrush FailureBackground { get; set; } = new SolidColorBrush(Color.Parse("#FEE2E2")); // Light red

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSuccess)
        {
            return isSuccess ? SuccessBackground : FailureBackground;
        }
        
        return FailureBackground;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a boolean to a string. Use parameter format: "TrueValue|FalseValue"
/// Example: ConverterParameter='Sending...|Send Invitation'
/// </summary>
public class BoolToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue && parameter is string paramString)
        {
            var parts = paramString.Split('|');
            if (parts.Length == 2)
            {
                return boolValue ? parts[0] : parts[1];
            }
        }
        
        return value?.ToString() ?? string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}