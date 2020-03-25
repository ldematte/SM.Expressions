using System;

namespace VirtualChannels
{
    public class DateTimeTimeUtils : ITimeUtils<DateTime>
    {
        public DateTime IndexToTime(DateTime time0, long index, int rateInMilliHz)
        {
            var millis = index * 1000 * 1000 / rateInMilliHz;
            return time0.AddMilliseconds(millis);
        }

        public long TimeToIndex(DateTime time0, DateTime currentTime, int rateInMilliHz)
        {
            var millis = currentTime.Subtract(time0).TotalMilliseconds;
            var index = (long)Math.Round(rateInMilliHz / 1000.0 * millis / 1000.0);
            return index;
        }

        public DateTime Zero()
        {
            return DateTime.Today;
        }
    }
}