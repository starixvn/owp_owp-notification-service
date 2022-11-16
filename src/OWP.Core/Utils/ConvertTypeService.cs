using System.Globalization;

namespace OWP.Core.Utils
{
    public static class ConvertTypeService
    {
        public static string ToOWPSpreadFormat(this decimal? spread)
        =>  spread.HasValue ? (spread.Value * 100).ToString("0.##", CultureInfo.InvariantCulture) : string.Empty;

        public static string ToOWPMaxBetFormat(this decimal? maxbet)
        => maxbet.HasValue ? (maxbet.Value / 1000).ToString("0.####", CultureInfo.InvariantCulture) + "K" : string.Empty;
    }
}