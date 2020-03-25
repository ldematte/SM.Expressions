using System;
using System.Collections.Generic;
using SM.Expressions;
using VirtualChannels.DataStructures;

namespace VirtualChannels
{
    internal class MultiValueWithSlowRowExpressionContext<TTime> : IExpressionContext
    {
        private readonly ISlowRowStorage<TTime> m_slowRowStorage;
        private readonly int[] m_loggedParametersIds;
        private readonly double[] m_currentValues;
        
        private TTime m_currentTime;

        public MultiValueWithSlowRowExpressionContext(ISlowRowStorage<TTime> slowRowStorage, int[] loggedParametersIds)
        {
            m_slowRowStorage = slowRowStorage;
            m_loggedParametersIds = loggedParametersIds;
            m_currentValues = new double[loggedParametersIds.Length];
        }

        public double GetValue(int parameterId, bool raw, bool noLog)
        {
            var idx = Array.IndexOf(m_loggedParametersIds, parameterId);
            if (idx >= 0)
            {
                return m_currentValues[idx];
            }
            
            
            if (noLog)
            {
                return m_slowRowStorage.GetValue(m_currentTime, parameterId, raw);
            }

            return double.NaN;
        }

        public void SetCurrentValue(TTime samplesTime, IList<CircularArraySegment<double>> completeDataSegments, int samplesIndex)
        {
            m_currentTime = samplesTime;
            for (int i = 0; i < m_currentValues.Length; ++i)
            {
                m_currentValues[i] = completeDataSegments[i][samplesIndex];
            }
        }
    }
}