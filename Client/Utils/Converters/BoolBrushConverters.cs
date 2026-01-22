using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Client.Utils.Converters;

/// <summary>
/// Provides boolean to brush converters
/// </summary>
public static class BoolBrushConverters
{
    /// <summary>
    /// Returns gray brush if false, dark gray if true
    /// </summary>
    public static readonly IValueConverter GrayIfFalse = new FuncValueConverter<bool, IBrush>(value =>
    {
        return value 
            ? new SolidColorBrush(Color.Parse("#374151")) // Dark gray when enabled
            : new SolidColorBrush(Color.Parse("#9CA3AF")); // Light gray when disabled
    });
}
