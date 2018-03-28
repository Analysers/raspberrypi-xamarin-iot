using System;
using NodaTime;

namespace Appliance.Helpers
{
    public static class DateTimeHelpers
    {
        public static DateTimeZone Zone => DateTimeZoneProviders.Tzdb["Australia/Melbourne"];
        
        public static TimeSpan TimeSpan(this DateTime dt)
        {
            return new TimeSpan(dt.Hour, dt.Minute, dt.Second);
        }

        public static TimeSpan AddTimeSpan(this DateTime dt, TimeSpan time)
        {
            return dt.TimeSpan().Add(time);
        }

        public static TimeSpan LocalTimeSpanFromUnixTime(this long unixTimeSeconds)
        {
            var localTime = Instant.FromUnixTimeSeconds(unixTimeSeconds).InZone(Zone);
            return new TimeSpan(localTime.Hour, localTime.Minute, 00);
        }

        public static bool IsEmpty(this TimeSpan timeSpan)
        {
            return timeSpan == new TimeSpan(0);
        }

        public static bool Within(this TimeSpan dtNow, TimeSpan from, TimeSpan to)
        {
            return dtNow > from || dtNow < to;
        }

        public static bool Within(this DateTime dateTime, TimeSpan from, TimeSpan to)
        {
            var dtNow = dateTime.TimeSpan();
            return dtNow > from || dtNow < to;
        }

        public static TimeSpan AddMinutes(this TimeSpan dtNow, int minutes)
        {
            return dtNow.Add(new TimeSpan(00, minutes, 00));
        }

        public static TimeSpan Add(this TimeSpan dtNow, TimeSpan timeSpan)
        {
            return dtNow.Add(timeSpan);
        }

        public static bool TimeFallsOnTheHourMark(TimeSpan timeSpan)
        {
            return int.Parse($"{timeSpan.Hours}{timeSpan.Minutes}{timeSpan.Seconds}") % 100 == 0;
        }

        public static bool TimeFallsOnThe5SecondMark(TimeSpan timeSpan)
        {
            return timeSpan.Seconds % 5 == 0;
        }
    }
}
