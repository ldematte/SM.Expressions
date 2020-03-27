using System;
using SM.Expressions;
using VirtualChannels.DataStructures;

namespace VirtualChannels
{
    public class MultiLoggedParametersVirtualChannel<TTime> : IVirtualChannel<TTime>
    {
        private readonly int m_virtualParameterId;
        private readonly int m_loggedParametersFrequencyInMilliHz;
        private readonly ITimeUtils<TTime> m_timeUtils;
        private readonly Func<double> m_evaluate;
        private readonly MultiValueWithSlowRowExpressionContext<TTime> m_context;

        private readonly CoverageCircularBuffer<TTime> m_coverageCircularBuffer;

        private TTime m_timeZero;

        public MultiLoggedParametersVirtualChannel(IExpression virtualExpression, int virtualParameterId, int[] loggedParametersIds, int loggedParametersFrequencyInMilliHz, 
            IParametersSymbolTable symbolTable, ISlowRowStorage<TTime> slowRowStorage, ITimeUtils<TTime> timeUtils)
        {
            m_virtualParameterId = virtualParameterId;
            m_loggedParametersFrequencyInMilliHz = loggedParametersFrequencyInMilliHz;
            m_timeUtils = timeUtils;
            m_coverageCircularBuffer = new CoverageCircularBuffer<TTime>(loggedParametersIds, loggedParametersFrequencyInMilliHz, 30, timeUtils);
            m_context = new MultiValueWithSlowRowExpressionContext<TTime>(slowRowStorage, loggedParametersIds);
            var compiler = new CompileVisitor(m_context, symbolTable, new DefaultCallContext());
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
            m_coverageCircularBuffer.AddSamples(time, channelId, values,
                (t, count, segments) =>
                {
                    var computedValues = new double[count];
                    for (var i = 0; i < count; i++)
                    {
                        var sampleTime = m_timeUtils.IndexToTime(t, i, m_loggedParametersFrequencyInMilliHz);
                        m_context.SetCurrentValue(sampleTime, segments, i);
                        computedValues[i] = m_evaluate();
                    }

                    computed(t, m_virtualParameterId, computedValues);
                });
        }
    }
}