using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Client.Utils.Converters;

/// <summary>
/// Provides number comparison converters
/// </summary>
public static class NumberConverters
{
    /// <summary>
    /// Returns true if the value is greater than the parameter
    /// </summary>
    public static readonly IValueConverter IsGreaterThan = new FuncValueConverter<int, bool>(value =>
    {
        return value > 1;
    });
}
