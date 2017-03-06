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

        private int zoomFactor;

        public Caption(TimelineLayout layout)
        {
            _layout = layout;

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
                OnPropertyChanged();
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
                OnPropertyChanged(nameof(LeftMarginWidth));
            }
        }

        public float LeftMarginWidth => (float)LeftMargin.TotalSeconds * 45 * zoomFactor / 100f;

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime => StartTime + MarkerDuration;

        public static Caption FakeData(TimelineLayout layout)
        {
            var text = File.ReadAllText(@"..\..\SourceText.txt").Replace(Environment.NewLine, " ");

            const int captionLength = 50;

            var idx1 = random.Next(0, text.Length - captionLength);

            var idx2 = text.IndexOf(' ', idx1) + 1;

            return new Caption(layout)
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