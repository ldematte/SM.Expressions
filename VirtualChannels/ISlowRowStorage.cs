namespace VirtualChannels
{
    public interface ISlowRowStorage<in TTime>
    {
        double GetValue(TTime atTime, int parameterId, bool raw);
    }
}