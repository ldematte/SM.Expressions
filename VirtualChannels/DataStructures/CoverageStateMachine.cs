using System;

namespace VirtualChannels.DataStructures
{
    internal interface ICoverageStateFactory
    {
        ICoverageState Extend(long startSampleIdx);
        ICoverageState Searching();
    }

    
    internal class FixedCoverageStateFactory : ICoverageStateFactory
    {
        private readonly ExtendState m_extendState;
        private readonly SearchState m_searchState;

        public FixedCoverageStateFactory()
        {
            m_extendState = new ExtendState(this);
            m_searchState = new SearchState(this);
        }

        public ICoverageState Extend(long startSampleIdx)
        {
            m_extendState.Start(startSampleIdx);
            return m_extendState;
        }

        public ICoverageState Searching()
        {
            return m_searchState;
        }
    }

    internal interface ICoverageState
    {
        void Start(long sampleIdx);
        ICoverageState Consume(CircularArray<double>[] buffers, int startBufferIdx, long sampleIdx, Action<long, long> segmentFound);
        ICoverageState Done(Action<long, long> segmentFound);
    }

    internal class ExtendState: ICoverageState
    {
        private readonly ICoverageStateFactory m_stateFactory;

        private long m_startIdx;
        private long m_endIdx;

        public ExtendState(ICoverageStateFactory stateFactory)
        {
            m_stateFactory = stateFactory;
        }

        public void Start(long sampleIdx)
        {
            m_startIdx = sampleIdx;
            m_endIdx = sampleIdx;
        }

        public ICoverageState Consume(CircularArray<double>[] buffers, int startBufferIdx, long sampleIdx, Action<long, long> segmentFound)
        {
            var complete = CoverageStateMachine.IsCompleteAtIndex(sampleIdx, buffers, startBufferIdx);
            if (complete)
            {
                m_endIdx = sampleIdx;
                return this;
            }

            return Done(segmentFound);
        }

        public ICoverageState Done(Action<long, long> segmentFound)
        {
            segmentFound(m_startIdx, m_endIdx);
            return m_stateFactory.Searching();
        }
    }

    internal class SearchState: ICoverageState
    {
        private readonly ICoverageStateFactory m_stateFactory;

        public SearchState(ICoverageStateFactory stateFactory)
        {
            m_stateFactory = stateFactory;
        }

        public void Start(long sampleIdx)
        {
        }

        public ICoverageState Consume(CircularArray<double>[] buffers, int startBufferIdx, long sampleIdx, Action<long, long> segmentFound)
        {
            var isComplete = CoverageStateMachine.IsCompleteAtIndex(sampleIdx, buffers, startBufferIdx);

            if (isComplete)
                return m_stateFactory.Extend(sampleIdx);
                
            return this;
        }

        public ICoverageState Done(Action<long, long> segmentFound)
        {
            return this;
        }
    }

    internal class CoverageStateMachine
    {
        private ICoverageState m_currentState;

        public CoverageStateMachine(ICoverageStateFactory stateFactory)
        {
            m_currentState = stateFactory.Searching();
        }

        public void Consume(CircularArray<double>[] buffers, int startBufferIdx, long sampleIdx, Action<long, long> segmentFound)
        {
            m_currentState = m_currentState.Consume(buffers, startBufferIdx, sampleIdx, segmentFound);
        }

        public void Done(Action<long, long> segmentFound)
        {
            m_currentState = m_currentState.Done(segmentFound);
        }

        internal static bool IsCompleteAtIndex(long sampleIdx, CircularArray<double>[] buffers, int skipBufferIdx)
        {
            var isComplete = true;
            for (int i = 0; i < buffers.Length; ++i)
            {
                if (i != skipBufferIdx)
                {
                    if (double.IsNaN(buffers[i][sampleIdx]))
                    {
                        isComplete = false;
                        break;
                    }
                }
            }

            return isComplete;
        }
    }
}