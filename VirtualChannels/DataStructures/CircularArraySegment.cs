using System.Runtime.CompilerServices;

namespace VirtualChannels.DataStructures
{
    public struct CircularArraySegment<T>
    {
        private readonly CircularArray<T> m_circularArray;
        private readonly long m_from;

        public CircularArraySegment(CircularArray<T> circularArray, long from, long to)
        {
            m_circularArray = circularArray;
            m_from = @from;
        }

        public T this[long index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_circularArray[m_from + index];
        }
    }
}