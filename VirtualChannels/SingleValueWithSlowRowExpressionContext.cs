using SM.Expressions;

namespace VirtualChannels
{
    internal class SingleValueWithSlowRowExpressionContext<TTime> : IExpressionContext
    {
        private readonly ISlowRowStorage<TTime> m_slowRowStorage;
        private double m_sample;
        private TTime m_currentTime;

        public SingleValueWithSlowRowExpressionContext(ISlowRowStorage<TTime> slowRowStorage)
        {
            m_slowRowStorage = slowRowStorage;
            m_sample = double.NaN;
        }

        internal void SetCurrentValue(double sample, TTime sampleTime)
        {
            m_sample = sample;
            m_currentTime = sampleTime;
        }

        public double GetValue(string identifier, string appName, bool raw, bool noLog)
        {
            if (noLog)
            {
                return m_slowRowStorage.GetValue(m_currentTime, identifier, appName, raw);
            }
            else
            {
                return m_sample;
            }
        }
    }
}