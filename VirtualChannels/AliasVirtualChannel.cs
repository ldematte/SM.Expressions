namespace VirtualChannels
{
    public class AliasVirtualChannel<TTime>: IVirtualChannel<TTime>
    {
        private readonly int m_originalChannelId;
        private readonly int m_aliasedChannelId;

        public AliasVirtualChannel(int originalChannelId, int aliasedChannelId)
        {
            m_originalChannelId = originalChannelId;
            m_aliasedChannelId = aliasedChannelId;
        }

        public void AddValues(TTime time, int channelId, double[] values, Computed<TTime> computed)
        {
            if (channelId == m_originalChannelId)
            {
                computed(time, m_aliasedChannelId, values);
            }
        }
    }
}