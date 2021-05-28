using Microsoft.AspNetCore.Mvc;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityShell.Logging
{
    public class LogEventsDataController : ControllerBase, IObserver<LogEvent>
    {
        private static readonly byte[] newLineBytes = System.Text.Encoding.Default.GetBytes(Environment.NewLine.ToCharArray());

        private readonly JsonSerializerOptions jsonSerializerOptions;

        public LogEventsDataController()
        {
            this.jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = false
            };
        }

        [HttpGet, Route("logEvents/data")]
        public async Task Get(CancellationToken cancelled)
        {
            await BeginEventStream();

            try
            {
                using var subscription = Program.LogEventsSink.Subscribe(this);

                // wait until cancelled,
                // subscription is disposes on leaving the try-block
                WaitHandle.WaitAll(new[] { cancelled.WaitHandle });
            }
            catch (InvalidOperationException)
            {
            }
        }

        private async Task BeginEventStream()
        {
            this.HttpContext.Response.ContentType = "text/event-stream";
            await this.HttpContext.Response.Body.FlushAsync();
        }

        private async Task WriteEventStreamMessage(LogEvent currentEvent)
        {
            // await JsonSerializer.SerializeAsync(this.HttpContext.Response.Body, currentEvent, this.jsonSerializerOptions);
            await this.WriteEventStreamData(currentEvent);
            // message ends with 2x newline
            await this.HttpContext.Response.Body.WriteAsync(newLineBytes, 0, newLineBytes.Length);
            await this.HttpContext.Response.Body.WriteAsync(newLineBytes, 0, newLineBytes.Length);
            await this.HttpContext.Response.Body.FlushAsync();
        }

        private async Task WriteEventStreamData(LogEvent currentEvent)
        {
            var data = JsonSerializer.Serialize(new
            {
                level = currentEvent.Level,
                msg = currentEvent.RenderMessage()
            }, this.jsonSerializerOptions);

            var message = new StringBuilder().Append("data: ").AppendLine(data);
            var bytes = System.Text.Encoding.Default.GetBytes(message.ToString());

            await this.HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
        }

        void IObserver<LogEvent>.OnCompleted()
        {
        }

        void IObserver<LogEvent>.OnError(Exception error)
        {
        }

        void IObserver<LogEvent>.OnNext(LogEvent value)
        {
            AsyncHelper.RunSync(() => this.WriteEventStreamMessage(value));
        }
    }

    internal static class AsyncHelper
    {
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
    }
}