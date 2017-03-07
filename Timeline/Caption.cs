using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using Timeline.Annotations;

namespace Timeline
{
    public sealed class Caption : INotifyPropertyChanged, IDisposable
    {
        private readonly TimelineLayout _layout;
        private readonly TimelineData _timeline;

        private int zoomFactor;

        public Caption(TimelineLayout layout, TimelineData timeline)
        {
            _layout = layout;
            _timeline = timeline;

            _layout.WhenZoomChanged.Subscribe(i =>
            {
                zoomFactor = i;
                OnPropertyChanged(nameof(MarkerWidth));
                OnPropertyChanged(nameof(LeftMarginWidth));
            });

            zoomFactor = _layout.Zoom;
            OnPropertyChanged(nameof(MarkerWidth));
            OnPropertyChanged(nameof(LeftMarginWidth));

            MarkerDuration = TimeSpan.FromSeconds(4);


        }

        private string text;
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                OnPropertyChanged();
            }
        }

        public string DiagnosticText => $"Start: {StartTime}, Left: {LeftMargin}\nDur: {MarkerDuration}, End: {EndTime}";

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (!value)
                {
                    subscription?.Dispose();
                }

                isSelected = value;
                OnPropertyChanged();
            }
        }

        private static Random random = new Random();

        private TimeSpan markerDuration;
        public TimeSpan MarkerDuration
        {
            get { return markerDuration; }
            set
            {
                markerDuration = value;
                LayoutUpdated();
            }
        }

        public float MarkerWidth => (float)MarkerDuration.TotalSeconds * 45 * zoomFactor / 100f;

        private TimeSpan _leftMargin;
        public TimeSpan LeftMargin
        {
            get { return _leftMargin; }
            set
            {
                _leftMargin = value;
                LayoutUpdated();
            }
        }

        public float LeftMarginWidth => (float)LeftMargin.TotalSeconds * 45 * zoomFactor / 100f;

        public TimeSpan StartTime
        {
            get
            {
                var previous = _timeline.PreviousCaption(this);

                if (previous == null)
                {
                    return LeftMargin;
                }

                return previous.EndTime + LeftMargin;
            }
        }

        public void LayoutUpdated()
        {
            OnPropertyChanged(nameof(LeftMargin));
            OnPropertyChanged(nameof(LeftMarginWidth));
            OnPropertyChanged(nameof(StartTime));
            OnPropertyChanged(nameof(DiagnosticText));
        }

        public TimeSpan EndTime => StartTime + MarkerDuration;

        public static Caption FakeData(TimelineLayout layout, TimelineData timeline)
        {
            var text = File.ReadAllText(@"..\..\SourceText.txt").Replace(Environment.NewLine, " ");

            const int captionLength = 50;

            var idx1 = random.Next(0, text.Length - captionLength);

            var idx2 = text.IndexOf(' ', idx1) + 1;

            return new Caption(layout, timeline)
            {
                Text = text.Substring(idx2, Math.Min(captionLength, text.Length - captionLength - 1))
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private IDisposable subscription;

        public void UpdateText(IObservable<string> whenCaptionValueChanged)
        {
            if (subscription != null)
            {
                subscription.Dispose();
            }

            subscription = whenCaptionValueChanged.Subscribe(i =>
            {
                Text = i;
            });
        }

        public void Dispose()
        {
            subscription?.Dispose();
        }

        public override string ToString()
        {
            return $"Margin: {LeftMargin}, start: {StartTime}, duration: {MarkerDuration}, end: {EndTime}";
        }
    }
}