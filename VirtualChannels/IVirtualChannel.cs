namespace VirtualChannels
{

    public delegate void Computed<in TTime>(TTime time, int parameterId, double[] computedValues);

    public interface IVirtualChannel<TTime>
    {
        void AddValues(TTime time, int parameterId, double[] values, Computed<TTime> computed);
    }
}
