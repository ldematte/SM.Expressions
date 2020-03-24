using System;
using SM.Expressions;
using VirtualChannels.DataStructures;

namespace VirtualChannels
{
    public class MultiParameterVirtualChannel<TTime> : IVirtualChannel<TTime>
    {
        private readonly ITimeUtils<TTime> m_timeUtils;
        private readonly Func<double> m_evaluate;
        private readonly MultiValueWithSlowRowExpressionContext<TTime> m_context;

        private readonly CoverageCircularBuffer<TTime> m_coverageCircularBuffer;

        private TTime m_timeZero;

        public MultiParameterVirtualChannel(IExpression virtualExpression, int virtualParameterId, int[] loggedParametersIds, int loggedParametersFrequencyInMilliHz, ISlowRowStorage<TTime> slowRowStorage, ITimeUtils<TTime> timeUtils)
        {
            m_timeUtils = timeUtils;
            m_coverageCircularBuffer = new CoverageCircularBuffer<TTime>(loggedParametersIds, loggedParametersFrequencyInMilliHz, 30, timeUtils);
            m_context = new MultiValueWithSlowRowExpressionContext<TTime>(slowRowStorage);
            var compiler = new CompileVisitor(m_context, new DefaultCallContext());
            virtualExpression.Accept(compiler);
            m_evaluate = compiler.GetCompiledExpression();

            m_timeZero = timeUtils.Zero();
        }

        public void Reset(TTime zero)
        {
            m_timeZero = zero;
        }

        public void AddValues(TTime time, int channelId, double[] values, Computed<TTime> computed)
        {
            m_coverageCircularBuffer.AddSamples(time, channelId, values, (t, segments) =>
            {

            });
        }
    }
}