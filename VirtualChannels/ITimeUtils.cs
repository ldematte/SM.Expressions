namespace VirtualChannels
{
    public interface ITimeUtils<TTime>
    {
        TTime IndexToTime(TTime time0, int index, int rateInMilliHz);
        long TimeToIndex(TTime time0, TTime currentTime, int rateInMilliHz);
        TTime Zero();
    }
}