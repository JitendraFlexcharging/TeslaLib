using System;

namespace TeslaLib.Converters
{
    public static class UnixTimeConverter
    {
        public static DateTimeOffset FromUnixTime(long unixTimeStamp)
        {
            DateTimeOffset time = new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, new TimeSpan(0));
            time = time.AddSeconds(unixTimeStamp).ToLocalTime();
            return time;
        }

        public static TimeSpan FromUnixTimeSpan(long unixTimeSpan) => TimeSpan.FromSeconds(unixTimeSpan);

        public static long ToUnixTimeSpan(TimeSpan timeSpan) => (long)timeSpan.TotalSeconds;

        public static DateTime FromUnixTimeStamp(long unixTimestamp) => new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimestamp);

        public static long ToUnixTimeStamp(DateTime dateTime) => (long) (dateTime - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    }
}
