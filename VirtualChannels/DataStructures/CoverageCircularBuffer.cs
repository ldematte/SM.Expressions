using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualChannels.DataStructures
{

    public delegate void SegmentCompleted<in TTime>(TTime startTime, int count, IList<CircularArraySegment<double>> completeDataSegments);
    public class CoverageCircularBuffer<TTime>
    {
        private readonly int[] m_loggedParametersIds;
        private readonly int m_loggedParametersRateInMilliHz;
        private readonly ITimeUtils<TTime> m_timeUtils;

        private readonly CircularArray<double>[] m_buffers;
        private readonly CoverageStateMachine m_coverageStateMachine;

        private readonly TTime m_timeZero;

        public CoverageCircularBuffer(int[] loggedParametersIds, int loggedParametersRateInMilliHz,
            int bufferWidthInSeconds, ITimeUtils<TTime> timeUtils)
        {
            m_loggedParametersIds = loggedParametersIds;
            m_loggedParametersRateInMilliHz = loggedParametersRateInMilliHz;
            m_timeUtils = timeUtils;

            m_coverageStateMachine = new CoverageStateMachine(new FixedCoverageStateFactory());

            var bufferSize = loggedParametersRateInMilliHz * bufferWidthInSeconds / 1000;
            m_buffers = new CircularArray<double>[loggedParametersIds.Length];
            for (int i = 0; i < m_buffers.Length; i++)
            {
                m_buffers[i] = new CircularArray<double>(bufferSize, double.NaN);
            }

            m_timeZero = timeUtils.Zero();
        }

        public void AddSamples(TTime time, int parameterId, double[] values, SegmentCompleted<TTime> segmentCompleted)
        {
            var parameterIdx = Array.IndexOf(m_loggedParametersIds, parameterId);

            if (parameterIdx >= 0)
            {
                var timeIndex = m_timeUtils.TimeToIndex(m_timeZero, time, m_loggedParametersRateInMilliHz);
                m_buffers[parameterIdx].Insert(values, timeIndex);

                SearchCompletedSegments(parameterIdx, timeIndex, timeIndex + values.Length,
                    (from, to) =>
                    {
                        var relativeIndex = (int)(from - timeIndex);
                        var segmentStartTime = m_timeUtils.IndexToTime(time, relativeIndex, m_loggedParametersRateInMilliHz);
                        segmentCompleted(segmentStartTime, (int)(to - from + 1),
                            m_buffers.Select(b => new CircularArraySegment<double>(b, from, to)).ToArray());
                    });
            }
        }

        private void SearchCompletedSegments(int startBufferIdx, long from, long to, Action<long, long> segmentFound)
        {
            for (var i = from; i < to; i++)
            {
                m_coverageStateMachine.Consume(m_buffers, startBufferIdx, i, segmentFound);
            }
            m_coverageStateMachine.Done(segmentFound);
        }
    }

}
