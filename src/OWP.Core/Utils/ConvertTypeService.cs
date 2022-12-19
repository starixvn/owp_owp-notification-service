using System.Globalization;

namespace OWP.Core.Utils
{
    public static class ConvertTypeService
    {
        private const int DEFAULT_SPREAD_UNIT = 100;

        private const int DEFAULT_MAXBET_UNIT = 1000;

        public static string ToOWPSpreadFormat(this decimal? spread)
        => spread.HasValue ? (spread.Value * DEFAULT_SPREAD_UNIT).ToString("0.##", CultureInfo.InvariantCulture) : string.Empty;

        public static string ToOWPMaxBetFormat(this decimal? maxbet)
        => maxbet.HasValue ? (maxbet.Value / DEFAULT_MAXBET_UNIT).ToString("0.####", CultureInfo.InvariantCulture) + "K" : string.Empty;
    }
}