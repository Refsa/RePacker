using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Refsa.Repacker.Buffers
{
    public class BufferPool
    {
        const int EXPAND_BY = 64;

        List<byte[]> bufferCache;
        Queue<int> free;

        int currentCapacity = 0;
        int bufferSize;

        public int Capacity => currentCapacity;
        public int Available => free.Count;

        public BufferPool(int size = 1024, int capacity = 1024)
        {
            bufferSize = size;

            bufferCache = new List<byte[]>();
            free = new Queue<int>(capacity);

            ExpandPool(capacity);
        }

        void ExpandPool(int by)
        {
            for (int i = 0; i < by; i++)
            {
                bufferCache.Add(new byte[bufferSize]);
                free.Enqueue(currentCapacity + i);
            }
            currentCapacity += by;
        }

        public void GetBuffer(out Buffer buffer)
        {
            if (free.Count == 0)
            {
                ExpandPool(EXPAND_BY);
            }

            int index = free.Dequeue();
            buffer = new Buffer(new Memory<byte>(bufferCache[index]), index);
        }

        public void FreeBuffer(Buffer buffer, bool flush = false)
        {
            if (flush)
            {
                buffer.Flush();
            }

            buffer.SetCount(0);

            free.Enqueue(buffer.Index);
        }
    }
}