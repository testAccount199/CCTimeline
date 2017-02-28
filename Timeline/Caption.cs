using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using Timeline.Annotations;

namespace Timeline
{
    public sealed class Caption : INotifyPropertyChanged
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
            });

            zoomFactor = _layout.Zoom;
            OnPropertyChanged(nameof(MarkerWidth));

            MarkerDuration = TimeSpan.FromSeconds(4);
        }

        public string Text { get; set; }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BackgroundColor));
            }
        }

        public System.Windows.Media.Brush BackgroundColor => System.Windows.Media.Brushes.Red;

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

        //public float MarkerWidth => 180f *zoomFactor/100f;
        public float MarkerWidth => (float)MarkerDuration.TotalSeconds * 45 * zoomFactor / 100f;
        public TimeSpan LeftMargin { get; set; }
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
    }
}