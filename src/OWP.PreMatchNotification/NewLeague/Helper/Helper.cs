using OWP.PreMatchNotification.NewLeague.Criteria;

namespace OWP.PreMatchNotification.NewLeague.Helper
{
    public static class Helper
    {
        public static QueryShiftPreMatchCriteria CreateShiftCriteria(DateTime eventDate, bool isDay, TimeSpan startTime = new())
        {
            var eventToDateTime = eventDate.AddDays(1).AddHours(11).AddMinutes(59).AddSeconds(59);
            var eventToRequestStartTime = isDay
                ? eventDate.AddHours(12)
                : CalculateStartTime(startTime, eventDate);

            return new QueryShiftPreMatchCriteria
            {
                EventDate = eventDate,
                EventToDateTime = eventToDateTime,
                EventFromDateTime = eventToRequestStartTime,
                IsDay = isDay
            };
        }

        private static DateTime CalculateStartTime(TimeSpan time, DateTime eventDate)
        {
            var twelvePm = new TimeSpan(12, 00, 00);
            var zero = new TimeSpan(00, 00, 00);

            if (time.CompareTo(zero) >= 0 && time.CompareTo(twelvePm) < 0)
            {
                return eventDate.Date.AddDays(1).Add(time);
            }

            return eventDate.Date.Add(time);
        }
    }
}
