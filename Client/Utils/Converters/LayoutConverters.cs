using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Client.Utils.Converters;

public class BoolToColumnConverter : IValueConverter
{
    public static readonly BoolToColumnConverter Instance = new();
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => (bool)value! ? 1 : 0;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class BoolToColumnSpanConverter : IValueConverter
{
    public static readonly BoolToColumnSpanConverter Instance = new();
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => (bool)value! ? 1 : 2;
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}