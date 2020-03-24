using System;
using SM.Expressions;

namespace VirtualChannels
{
    public class SingleLoggedParameterVirtualChannel<TTime> : IVirtualChannel<TTime>
    {
        private readonly int m_loggedParameterId;
        private readonly int m_loggedParameterFrequencyInMilliHz;
        private readonly ITimeUtils<TTime> m_timeUtils;
        private readonly SingleValueWithSlowRowExpressionContext<TTime> m_context;
        private readonly Func<double> m_evaluate;

        public SingleLoggedParameterVirtualChannel(int loggedParameterId, int loggedParameterFrequencyInMilliHz, ISlowRowStorage<TTime> slowRowStorage, ITimeUtils<TTime> timeUtils)
        {
            m_loggedParameterId = loggedParameterId;
            m_loggedParameterFrequencyInMilliHz = loggedParameterFrequencyInMilliHz;
            m_timeUtils = timeUtils;
            m_context = new SingleValueWithSlowRowExpressionContext<TTime>(slowRowStorage);
            var compiler = new CompileVisitor(m_context, new DefaultCallContext());
            m_evaluate = compiler.GetCompiledExpression();
        }

        public void AddValues(TTime time, int channelId, double[] values, Computed<TTime> computed)
        {
            var computedValues = new double[values.Length];
            for (var i = 0; i < values.Length; i++)
            {
                var sampleTime = m_timeUtils.AddMillis(time, i * 1000  * 1000 / m_loggedParameterFrequencyInMilliHz);
                m_context.SetCurrentValue(values[i], sampleTime);
                computedValues[i] = m_evaluate();
            }

            computed(time, m_loggedParameterId, computedValues);
        }
    }
}