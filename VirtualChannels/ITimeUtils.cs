namespace VirtualChannels
{
    public interface ITimeUtils<TTime>
    {
        TTime AddMillis(TTime time, int millis);

        long TimeToIndex(TTime time0, TTime currentTime, int rateInMilliHz);
    }
}