using Serilog.Events;
using System;
using System.Diagnostics;
using System.Reactive.Subjects;

namespace IdentityShell.Logging
{
    public sealed class LogEventsSink : IObserver<LogEvent>, IObservable<LogEvent>
    {
        private Subject<LogEvent> logEvents = new Subject<LogEvent>();

        #region IObserver<LogEvent>

        void IObserver<LogEvent>.OnCompleted()
        {
            this.logEvents.OnCompleted();
        }

        void IObserver<LogEvent>.OnError(Exception error)
        {
            this.logEvents.OnError(error);
        }

        void IObserver<LogEvent>.OnNext(LogEvent value)
        {
            Debug.Print(value.MessageTemplate.Text);
            this.logEvents.OnNext(value);
        }

        #endregion IObserver<LogEvent>

        #region IObservable<LogEvent>

        public IDisposable Subscribe(IObserver<LogEvent> observer)
        {
            return this.logEvents.Subscribe(observer);
        }

        #endregion IObservable<LogEvent>
    }
}