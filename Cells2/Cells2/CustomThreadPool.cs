using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cells
{
    class CustomThreadPool : TaskScheduler, IDisposable
    {
        private readonly BlockingCollection<Task> _taskQueue;
        private readonly List<Thread> _threads;
        private volatile bool _running = true;

        public CustomThreadPool(int maxThreads)
        :base()
        {
            _taskQueue = new BlockingCollection<Task>();
            _threads = new List<Thread>();

            for (int i = 0; i < maxThreads; i++)
            {
                Thread thread = new Thread(WorkerLoop) { IsBackground = true };
                _threads.Add(thread);
                thread.Start();
            }
        }

        private void WorkerLoop()
        {
            foreach (var task in _taskQueue.GetConsumingEnumerable())
            {
                TryExecuteTask(task);
            }
        }

        protected override IEnumerable<Task> GetScheduledTasks() => _taskQueue.ToArray();

        protected override void QueueTask(Task task)
        {
            if (!_running) throw new ObjectDisposedException("CustomThreadPool");
            _taskQueue.Add(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false; // We don't execute tasks inline.
        }

        public void Dispose()
        {
            _running = false;
            _taskQueue.CompleteAdding();
        }
    }
}