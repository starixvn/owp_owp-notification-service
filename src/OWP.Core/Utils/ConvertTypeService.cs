using System.Globalization;

namespace OWP.Core.Utils;

public static class ConvertTypeService
{
    private const int DefaultSpreadUnit = 100;

    private const int DefaultMaxbetUnit = 1000;

    public static string ToOWPSpreadFormat(this decimal? spread)
        => spread.HasValue ? (spread.Value * DefaultSpreadUnit).ToString("0.##", CultureInfo.InvariantCulture) : string.Empty;

    public static string ToOWPMaxBetFormat(this decimal? maxbet)
        => maxbet.HasValue ? (maxbet.Value / DefaultMaxbetUnit).ToString("0.####", CultureInfo.InvariantCulture) + "K" : string.Empty;
}