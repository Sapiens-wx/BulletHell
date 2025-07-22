using System;

namespace ResearchUtilities.DataStructure
{
    public class ShadowWrappedBuffer<T>
    {
        private readonly T[] buffer;
        private readonly int capacity;
        private int writeIndex;
        private int count;
        private int dirtyStart = -1;  // -1: 无脏数据, capacity: 全部脏

        public int Capacity => capacity;
        public int Count => count;
        public bool HasDirty => dirtyStart != -1;

        public ShadowWrappedBuffer(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException("Capacity must be positive", nameof(capacity));

            this.capacity = capacity;
            buffer = new T[capacity * 2];
            writeIndex = 0;
            count = 0;
        }

        /// <summary>
        /// 写入单个元素，自动覆盖最旧数据
        /// </summary>
        public void Write(T data)
        {
            buffer[writeIndex] = data;
            buffer[writeIndex + capacity] = data;  // 镜像写入

            // 更新脏数据标记
            if (dirtyStart == -1)
            {
                // 第一次写入，标记起始位置
                dirtyStart = writeIndex;
            }
            else if (count == capacity && writeIndex == dirtyStart)
            {
                // 如果缓冲区已满且覆盖到脏数据起始位置，标记全部为脏
                dirtyStart = capacity;
            }

            writeIndex = (writeIndex + 1) % capacity;
            if (count < capacity) count++;
        }

        public void WriteRange(T[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            int dataLen = data.Length;

            if (dataLen >= capacity)
            {
                // 如果传入数组比容量还大，直接写最后 capacity 个数据
                int startIndex = dataLen - capacity;
                Array.Copy(data, startIndex, buffer, 0, capacity);
                Array.Copy(data, startIndex, buffer, capacity, capacity);
                writeIndex = 0;
                count = capacity;
                dirtyStart = capacity; // 全部数据都是脏的
            }
            else
            {
                // 如果是第一次写入，记录起始位置
                if (dirtyStart == -1)
                {
                    dirtyStart = writeIndex;
                }
                
                // 写入数据
                int tailLen = Math.Min(capacity - writeIndex, dataLen);
                Array.Copy(data, 0, buffer, writeIndex, tailLen);
                Array.Copy(data, 0, buffer, writeIndex + capacity, tailLen);

                int remain = dataLen - tailLen;
                if (remain > 0)
                {
                    Array.Copy(data, tailLen, buffer, 0, remain);
                    Array.Copy(data, tailLen, buffer, capacity, remain);
                }

                // 检查是否覆盖到脏数据起始位置
                if (count == capacity && 
                    ((writeIndex <= dirtyStart && writeIndex + dataLen > dirtyStart) ||
                     (writeIndex + dataLen > capacity && remain > dirtyStart)))
                {
                    dirtyStart = capacity;
                }

                writeIndex = (writeIndex + dataLen) % capacity;
                count = Math.Min(count + dataLen, capacity);
            }
        }

        public void Clear()
        {
            writeIndex = 0;
            count = 0;
            dirtyStart = -1;
        }

        /// <summary>
        /// 获取并清除脏数据。如果没有脏数据返回空切片。
        /// </summary>
        public ReadOnlySpan<T> ReadDirty()
        {
            if (dirtyStart == -1) return ReadOnlySpan<T>.Empty;

            ReadOnlySpan<T> result;
            if (dirtyStart == capacity)
            {
                // 全部数据都是脏的
                result = GetSpan();
            }
            else
            {
                // 从脏数据起始位置到当前写入位置
                int length = (writeIndex - dirtyStart + capacity) % capacity;
                if (length == 0) length = (count == capacity) ? capacity : 0;
                result = new ReadOnlySpan<T>(buffer, dirtyStart, length);
            }

            // 清除脏标记
            dirtyStart = -1;
            return result;
        }

        /// <summary>
        /// 查看脏数据但不清除标记
        /// </summary>
        public ReadOnlySpan<T> PeekDirty()
        {
            if (dirtyStart == -1) return ReadOnlySpan<T>.Empty;

            if (dirtyStart == capacity)
            {
                return GetSpan();
            }

            int length = (writeIndex - dirtyStart + capacity) % capacity;
            if (length == 0) length = (count == capacity) ? capacity : 0;
            return new ReadOnlySpan<T>(buffer, dirtyStart, length);
        }


        /// <summary>
        /// 获取最新的数据，按时间顺序返回一个连续的只读切片。
        /// 可指定返回长度 length，如果length大于实际数据量，则返回所有可用数据。
        /// 如果buffer为空，返回空切片。
        /// </summary>
        public ReadOnlySpan<T> GetSpan(int length)
        {
            if (count == 0 || length == 0)
                return ReadOnlySpan<T>.Empty;

            // 如果请求长度为负数或超过实际数据量，返回所有可用数据
            length = (length <= 0 || length > count) ? count : length;

            // 计算起始位置：最新数据的位置(writeIndex) - 要返回的长度
            int start = (writeIndex - length + capacity) % capacity;
            return new ReadOnlySpan<T>(buffer, start, length);
        }

        public ReadOnlySpan<T> GetSpan() => GetSpan(count);

        /// <summary>
        /// 清除脏数据标记，但不影响数据内容
        /// </summary>
        public void UnmarkDirty()
        {
            dirtyStart = -1;
        }

        /// <summary>
        /// 清除脏数据并重置标记。
        /// 如果所有数据都是脏的，清除所有数据；
        /// 否则只清除从dirtyStart到writeIndex的数据。
        /// </summary>
        public void ClearDirty()
        {
            if (dirtyStart == -1) return;

            if (dirtyStart == capacity)
            {
                // 如果全部数据都是脏的，清除所有数据
                Clear();
            }
            else
            {
                // 计算需要清除的数据长度
                int length = (writeIndex - dirtyStart + capacity) % capacity;
                if (length == 0) length = (count == capacity) ? capacity : 0;

                // 清除数据
                Array.Clear(buffer, dirtyStart, length);
                Array.Clear(buffer, dirtyStart + capacity, length);

                // 重置写入位置和计数
                writeIndex = dirtyStart;
                count = (dirtyStart + capacity - writeIndex) % capacity;
            }
            dirtyStart = -1;
        }
    }
}