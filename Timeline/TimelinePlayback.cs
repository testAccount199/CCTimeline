using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using Timeline.Annotations;

namespace Timeline
{
    public sealed class TimelinePlayback : INotifyPropertyChanged
    {
        private readonly TimeSpan _duration;

        public TimeSpan Value
        {
            get
            {
                if (IsStopped)
                {
                    return TimeSpan.Zero;
                }

                if (!IsPlaying)
                {
                    return Accumulated;
                }

                return DateTime.Now - PlayStart.Value + Accumulated;
            }
        }

        private ManualResetEvent resetEvent = new ManualResetEvent(false);

        private readonly Subject<TimeSpan> whenTimeChanged = new Subject<TimeSpan>();
        public IObservable<TimeSpan> WhenTimeChanged => whenTimeChanged;

        public TimelinePlayback(TimeSpan duration)
        {
            _duration = duration;
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    resetEvent.WaitOne();

                    if (IsPlaying)
                    {
                        OnPropertyChanged(nameof(Value));
                        whenTimeChanged.OnNext(Value);
                    }

                    Thread.Sleep(10);
                }
            }, TaskCreationOptions.LongRunning);
        }

        private bool IsPlaying = false;
        private bool IsStopped = true;
        private DateTime? PlayStart = null;

        private TimeSpan Accumulated = TimeSpan.Zero;

        public void PlayPause()
        {
            IsStopped = false;

            if (!IsPlaying)
            {
                PlayStart = DateTime.Now;
                IsPlaying = true;
                resetEvent.Set();
            }
            else
            {
                resetEvent.Reset();

                Accumulated = Value;

                IsPlaying = false;
            }

            OnPropertyChanged(nameof(Value));

        }

        public void Stop()
        {
            IsStopped = true;
            IsPlaying = false;
            PlayStart = null;
            Accumulated = TimeSpan.Zero;

            OnPropertyChanged(nameof(Value));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}