using System;
using System.Collections.Generic;
using System.Linq;
using SM.Expressions;

namespace VirtualChannels
{
    public class VirtualChannelBuilder<TTime>
    {
        private readonly IParametersSymbolTable m_symbolTable;
        private readonly int m_slowRowRateInMilliHz;
        private readonly ISlowRowStorage<TTime> m_slowRowStorage;
        private readonly ITimeUtils<TTime> m_timeUtils;

        public VirtualChannelBuilder(IParametersSymbolTable symbolTable, int slowRowRateInMilliHz, ISlowRowStorage<TTime> slowRowStorage, ITimeUtils<TTime> timeUtils)
        {
            m_symbolTable = symbolTable;
            m_slowRowRateInMilliHz = slowRowRateInMilliHz;
            m_slowRowStorage = slowRowStorage;
            m_timeUtils = timeUtils;
        }

        public void Build(string virtualParameterName, string formula, int rateInMilliHz, Action<IVirtualChannel<TTime>, bool> built, Action<Exception> failed)
        {
            try
            {
                m_symbolTable.GetId(virtualParameterName, string.Empty, virtualParameterId =>
                    {
                        var expression = SMExpressionParser.ParseOrThrow(formula);

                        var visitor = new VirtualChannelBuilderVisitor(m_symbolTable);
                        expression.Accept(visitor);

                        if (visitor.IsAlias())
                        {
                            built(new AliasVirtualChannel<TTime>(visitor.LoggedParameters.First(), virtualParameterId), false);
                        }
                        else if (visitor.IsSlowRow())
                        {
                            if (m_slowRowRateInMilliHz != rateInMilliHz)
                            {
                                failed(new ArgumentException($"{virtualParameterName} has rate {rateInMilliHz}, but it is slow row"));
                            }
                            built(new SlowRowVirtualChannel<TTime>(expression, virtualParameterId, rateInMilliHz, m_symbolTable, m_slowRowStorage, m_timeUtils), true);
                        }
                        else if (visitor.LoggedParameters.Count() == 1)
                        {
                            built(new SingleLoggedParameterVirtualChannel<TTime>(expression, virtualParameterId, visitor.LoggedParameters.First(), 
                                rateInMilliHz, m_symbolTable, m_slowRowStorage, m_timeUtils), false);
                        }
                        else
                        {
                            built(new MultiLoggedParametersVirtualChannel<TTime>(expression, virtualParameterId, visitor.LoggedParameters.ToArray(), 
                                rateInMilliHz, m_symbolTable, m_slowRowStorage, m_timeUtils), false);
                        }
                    },
                    () =>
                    {
                        failed(new KeyNotFoundException($"Virtual parameter name {virtualParameterName} not found"));
                    });

            }
            catch (Exception e)
            {
                failed(e);
            }
        }
    }
}