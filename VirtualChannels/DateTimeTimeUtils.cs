using System;

namespace VirtualChannels
{
    public class DateTimeTimeUtils : ITimeUtils<DateTime>
    {
        public DateTime AddMillis(DateTime time, int millis)
        {
            return time.AddMilliseconds(millis);
        }

        public long TimeToIndex(DateTime time0, DateTime currentTime, int rateInMilliHz)
        {
            var millis = currentTime.Subtract(time0).TotalMilliseconds;
            var index = (long)Math.Round(1000.0 / rateInMilliHz * millis);
            return index;
        }

        public DateTime Zero()
        {
            return DateTime.Today;
        }
    }
}