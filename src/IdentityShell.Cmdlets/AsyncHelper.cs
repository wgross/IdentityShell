using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityShell.Cmdlets
{
    internal static class AsyncHelper
    {
        private sealed class SynchronizationContextGuard : IDisposable
        {
            private SynchronizationContext context;
            private bool disposedValue;

            public SynchronizationContextGuard()
            {
                this.context = SynchronizationContext.Current;
                SynchronizationContext.SetSynchronizationContext(null);
            }

            private void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        SynchronizationContext.SetSynchronizationContext(this.context);
                        this.context = null;
                    }

                    disposedValue = true;
                }
            }

            public void Dispose()
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }

        public static IDisposable DisableSynchronizationContext => new SynchronizationContextGuard();

        #region Experimental

        // https://github.com/donatasm/ParallelExtensionsExtras/blob/master/src/TaskSchedulers/CurrentThreadTaskScheduler.cs

        public sealed class CurrentThreadTaskScheduler : TaskScheduler
        {
            protected override void QueueTask(Task task) => TryExecuteTask(task);

            protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => TryExecuteTask(task);

            protected override IEnumerable<Task> GetScheduledTasks() => Enumerable.Empty<Task>();

            public override int MaximumConcurrencyLevel => 1;
        }

        private static readonly TaskFactory _currentTreadTaskFactory = new TaskFactory(
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskContinuationOptions.None,
            new CurrentThreadTaskScheduler());

        public static TResult RunSyncInCurrentThread<TResult>(Func<Task<TResult>> func)
        {
            return AsyncHelper._currentTreadTaskFactory
              .StartNew<Task<TResult>>(func)
              .Unwrap<TResult>()
              .GetAwaiter()
              .GetResult();
        }

        public static void RunSyncInCurrentThread(Func<Task> func)
        {
            AsyncHelper._currentTreadTaskFactory
              .StartNew<Task>(func)
              .Unwrap()
              .GetAwaiter()
              .GetResult();
        }

        #endregion Experimental

        // https://stackoverflow.com/questions/9343594/how-to-call-asynchronous-method-from-synchronous-method-in-c

        private static readonly TaskFactory _threadPoolTaskFactory = new TaskFactory(
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskContinuationOptions.None,
            TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return AsyncHelper._threadPoolTaskFactory
              .StartNew<Task<TResult>>(func)
              .Unwrap<TResult>()
              .GetAwaiter()
              .GetResult();
        }

        public static void RunSync(Func<Task> func)
        {
            AsyncHelper._threadPoolTaskFactory
              .StartNew<Task>(func)
              .Unwrap()
              .GetAwaiter()
              .GetResult();
        }

        public static T AwaitWithoutOriginalContext<T>(Task<T> task)
        {
            return task
                .ConfigureAwait(continueOnCapturedContext: false)
                .GetAwaiter()
                .GetResult();
        }

        public static void AwaitWithoutOriginalContext<T>(Task task)
        {
            task
                .ConfigureAwait(continueOnCapturedContext: false)
                .GetAwaiter()
                .GetResult();
        }
    }
}