namespace VirtualChannels
{
    public interface ISlowRowStorage<in TTime>
    {
        double GetValue(TTime atTime, string identifier, string appName, bool raw);
    }
}