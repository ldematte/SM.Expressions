using SM.Expressions;

namespace VirtualChannels
{
    internal class SingleValueWithSlowRowExpressionContext<TTime> : IExpressionContext
    {
        private readonly ISlowRowStorage<TTime> m_slowRowStorage;
        private readonly int m_loggedParameterId;
        private double m_sample;
        private TTime m_currentTime;

        public SingleValueWithSlowRowExpressionContext(ISlowRowStorage<TTime> slowRowStorage, int loggedParameterId)
        {
            m_slowRowStorage = slowRowStorage;
            m_loggedParameterId = loggedParameterId;
            m_sample = double.NaN;
        }

        internal void SetCurrentValue(double sample, TTime sampleTime)
        {
            m_sample = sample;
            m_currentTime = sampleTime;
        }

        public double GetValue(int parameterId, bool raw, bool noLog)
        {
            if (m_loggedParameterId == parameterId)
            {
                return m_sample;
            }

            if (noLog)
            {
                return m_slowRowStorage.GetValue(m_currentTime, parameterId, raw);
            }
            
            return double.NaN;
        }
    }
}