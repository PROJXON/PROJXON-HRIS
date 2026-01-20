using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Client.Utils.Converters;

/// <summary>
/// Converter that returns true if the value does NOT match the parameter string.
/// Used to hide the "Hire" button when candidate is already in "hired" stage.
/// </summary>
public class InverseStringMatchConverter : IValueConverter
{
    public static readonly InverseStringMatchConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string stringValue && parameter is string parameterValue)
        {
            return !string.Equals(stringValue, parameterValue, StringComparison.OrdinalIgnoreCase);
        }
        return true;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
