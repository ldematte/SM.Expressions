using System;
using System.Runtime.CompilerServices;

namespace VirtualChannels.DataStructures
{
    public class CircularArray<T>
    {
        private readonly T m_noElement;
        private readonly T[] m_buffer;

        private long m_startIndex;
        private long m_endIndex;

        public CircularArray(int capacity, T noElement)
        {
            if (capacity < 1)
            {
                throw new ArgumentException("Circular buffer cannot have negative or zero capacity.", nameof(capacity));
            }

            m_noElement = noElement;

            m_buffer = new T[capacity];

            for (var i = 0; i < capacity; ++i)
            {
                m_buffer[i] = noElement;
            }

            m_startIndex = 0;
            m_endIndex = capacity - 1;
        }
        
        public T this[long index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index > m_endIndex || index < m_startIndex)
                {
                    return m_noElement;
                }

                var actualIndex = InternalIndex(index);
                return m_buffer[actualIndex];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (index < m_startIndex)
                {
                    return;
                }

                if (index > m_endIndex)
                {
                    var currentEnd = m_endIndex;
                    
                    m_endIndex = index;
                    m_startIndex = index - m_buffer.Length + 1;

                    var eraseFrom = Math.Max(currentEnd + 1, m_startIndex);
                    
                    for (var i = eraseFrom; i < index; ++i)
                    {
                        m_buffer[InternalIndex(i)] = m_noElement;
                    }
                }

                var actualIndex = InternalIndex(index);
                m_buffer[actualIndex] = value;
            }
        }
        
        private long InternalIndex(long index)
        {
            return index % m_buffer.Length;
        }
    }
}
