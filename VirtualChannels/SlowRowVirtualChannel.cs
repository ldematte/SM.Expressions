using System;
using SM.Expressions;

namespace VirtualChannels
{
    public class SlowRowVirtualChannel<TTime> : IVirtualChannel<TTime>
    {
        private readonly int m_virtualParameterId;
        private readonly int m_slowRowRateInMilliHz;
        private readonly ITimeUtils<TTime> m_timeUtils;
        private readonly SlowRowExpressionContext<TTime> m_context;
        private readonly Func<double> m_evaluate;

        public SlowRowVirtualChannel(IExpression virtualExpression, int virtualParameterId, int slowRowRateInMilliHz,
            IParametersSymbolTable symbolTable, ISlowRowStorage<TTime> slowRowStorage, ITimeUtils<TTime> timeUtils)
        {
            m_virtualParameterId = virtualParameterId;
            m_slowRowRateInMilliHz = slowRowRateInMilliHz;
            m_timeUtils = timeUtils;
            m_context = new SlowRowExpressionContext<TTime>(slowRowStorage);
            var compiler = new CompileVisitor(m_context, symbolTable, new DefaultCallContext());
            virtualExpression.Accept(compiler);
            m_evaluate = compiler.GetCompiledExpression();
        }

        public void AddValues(TTime time, int channelId, double[] values, Computed<TTime> computed)
        {
            var computedValues = new double[values.Length];
            for (var i = 0; i < values.Length; i++)
            {
                var sampleTime = m_timeUtils.IndexToTime(time, i, m_slowRowRateInMilliHz);
                m_context.SetCurrentTime(sampleTime);
                computedValues[i] = m_evaluate();
            }

            computed(time, m_virtualParameterId, computedValues);
        }
    }
}