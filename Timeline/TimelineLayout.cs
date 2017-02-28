using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Windows.Automation.Peers;
using Timeline.Annotations;

namespace Timeline
{
    public sealed class TimelineLayout : INotifyPropertyChanged
    {
        public TimelineLayout()
        {
            Zoom = 101;
        }

        private int _zoom;
        public int Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                _zoom = value;
                _whenZoomChanged.OnNext(value);
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly Subject<int> _whenZoomChanged = new Subject<int>();
        public IObservable<int> WhenZoomChanged => _whenZoomChanged;

        public double GetScrollPosition(TimeSpan time)
        {
            return time.TotalMilliseconds/22.33d*(Zoom/100d);
        }

        public double GetPlayerHeadPosition(TimeSpan time)
        {
            return .1;
        }
    }
}