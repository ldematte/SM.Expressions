using System;
using System.Collections.Generic;

namespace VirtualChannels.DataStructures
{

    public delegate void SegmentCompleted<in TTime>(TTime startTime, IEnumerable<ArraySegment<double>> completeDataSegments);
    public class CoverageCircularBuffer<TTime>
    {
        public CoverageCircularBuffer(int[] loggerParametersIds, int loggerParametersRateInMilliHz,
            int bufferWidthInSeconds, ITimeUtils<TTime> timeUtils)
        {
            
        }


        public void AddSamples()
        {

        }

        public void AddSamples(TTime time, int channelId, double[] values, SegmentCompleted<TTime> segmentCompleted)
        {
            throw new NotImplementedException();
        }
    }
}
