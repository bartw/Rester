using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BeeWee.Rester
{
    internal class Throttler : IDisposable
    {
        private SemaphoreSlim _pool;
        private TimeSpan _resetSpan;
        private Queue<DateTime> _releaseTimes;
        private object _queueLock;

        public Throttler(int count, TimeSpan resetSpan)
        {
            _pool = new SemaphoreSlim(count);
            _resetSpan = resetSpan;
            _releaseTimes = new Queue<DateTime>();
            _queueLock = new object();

            _releaseTimes.Enqueue(DateTime.MinValue);
        }

        public async Task<TResult> RunAsync<T1, T2, TResult>(Func<T1, T2, Task<TResult>> action, T1 param1, T2 param2)
        {
            await _pool.WaitAsync();

            DateTime oldest;
            lock (_queueLock)
            {
                oldest = _releaseTimes.Dequeue();
            }

            DateTime now = DateTime.UtcNow;
            DateTime windowReset = oldest.Add(_resetSpan);
            if (windowReset > now)
            {
                int sleepMilliseconds = Math.Max((int)(windowReset.Subtract(now).Ticks / TimeSpan.TicksPerMillisecond), 1); // sleep at least 1ms to be sure next window has started
            }

            try
            {
                return await action(param1, param2);
            }
            finally
            {
                lock (_queueLock)
                {
                    _releaseTimes.Enqueue(DateTime.UtcNow);
                }
                _pool.Release();
            }
        }

        public void Dispose()
        {
            _pool.Dispose();
        }
    }
}
