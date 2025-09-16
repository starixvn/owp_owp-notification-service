namespace OWP.Core.Utils
{
    public static class DateTimeConverter
    {
        public static DateTime ToGmt8(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().AddHours(8);
        }
    }
}
