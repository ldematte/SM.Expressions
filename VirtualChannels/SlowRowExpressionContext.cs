using SM.Expressions;

namespace VirtualChannels
{
    public class SlowRowExpressionContext<TTime>: IExpressionContext
    {
        private readonly ISlowRowStorage<TTime> m_slowRowStorage;
        private TTime m_currentTime;

        public SlowRowExpressionContext(ISlowRowStorage<TTime> slowRowStorage)
        {
            m_slowRowStorage = slowRowStorage;
        }

        internal void SetCurrentTime(TTime sampleTime)
        {
            m_currentTime = sampleTime;
        }

        public double GetValue(int parameterId, bool raw, bool noLog)
        {
            if (noLog)
            {
                return m_slowRowStorage.GetValue(m_currentTime, parameterId, raw);
            }
            
            return double.NaN;
        }
    }
}