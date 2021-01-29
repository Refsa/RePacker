using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Refsa.Repacker.Buffers
{
    public class BufferPool
    {
        List<byte[]> bufferCache;
        Queue<int> free;

        public BufferPool(int size = 1024, int capacity = 1024)
        {
            bufferCache = new List<byte[]>();
            free = new Queue<int>(capacity);

            for (int i = 0; i < capacity; i++)
            {
                bufferCache.Add(new byte[size]);
                free.Enqueue(i);
            }
        }

        public void GetBuffer(out Buffer buffer)
        {
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