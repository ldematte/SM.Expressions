using System;

namespace VirtualChannels
{
    public class MillisTimeUtils : ITimeUtils<long>
    {
        public long IndexToTime(long time0, long index, int rateInMilliHz)
        {
            var millis = (long)index * 1000 * 1000 / rateInMilliHz;
            return time0 + millis;
        }

        public long TimeToIndex(long time0, long currentTime, int rateInMilliHz)
        {
            var millis = currentTime - time0;
            var index = (long)Math.Round(rateInMilliHz / 1000.0 * millis / 1000.0);
            return index;
        }

        public long Zero()
        {
            return 0;
        }
    }
}