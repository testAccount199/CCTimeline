using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio.Gui;
using NAudio.Wave;
using Timeline.Annotations;

namespace Timeline
{
    /// <summary>
    /// Interaction logic for RootControl.xaml
    /// </summary>
    public partial class RootControl : UserControl, INotifyPropertyChanged
    {
        public TimelineData Timeline { get; }

        public TimelineLayout TimelineLayout { get; }
        public TimelinePlayback Playback { get; }

        private bool isPlaying = false;

        public bool IsPlaying
        {
            get { return isPlaying; }
            set
            {
                isPlaying = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotPlaying));
            }
        }

        public bool IsNotPlaying
        {
            get { return !IsPlaying; }
        }

        public RootControl()
        {
            TimelineLayout = new TimelineLayout();
            Playback = new TimelinePlayback(TimeSpan.FromMinutes(3));
            Timeline = TimelineData.FakeData(TimelineLayout, Playback);

            InitializeComponent();

            Playback.WhenTimeChanged.Subscribe(time =>
            {
                var scrollPosition = TimelineLayout.GetScrollPosition(time);

                //var playerHeadPosition = TimelineLayout.GetPlayerHeadPosition(time);

                //overlay.Set(playerHeadPosition);

                ScrollTo(scrollPosition);

                whenTimeChanged.OnNext(time);

                var caption = Timeline.PlaybackCaption;

                whenPlaybackCaptionChanged.OnNext(caption);

                Thread.Sleep(100);
            });

            //var viewer = new WaveViewer();
            //viewer.BackColor = System.Drawing.Color.LightGray;
            //viewer.WaveStream = new WaveFileReader(@"C:\Users\Amichai\Desktop\The Zahir.wav");

            //this.Host.Child = viewer;

            TimelineLayout.WhenZoomChanged.Subscribe(zoom =>
            {
                var scale = TimelineLayout.Zoom / 100f;
                //viewer.SamplesPerPixel = (int)Math.Round(128 / scale);
            });
        }

        private readonly Subject<TimeSpan> whenTimeChanged = new Subject<TimeSpan>();
        public IObservable<TimeSpan> WhenTimeChanged => whenTimeChanged;

        private readonly Subject<Caption> whenPlaybackCaptionChanged = new Subject<Caption>();
        public IObservable<Caption> WhenPlaybackCaptionChanged => whenPlaybackCaptionChanged;


        private readonly Subject<Caption> whenCaptionSelected = new Subject<Caption>();
        public IObservable<Caption> WhenCaptionSelected => whenCaptionSelected;

        private void ScrollTo(double position)
        {
            Dispatcher.Invoke(() =>
            {
                //this.WaveformBorder.Margin = new Thickness(-position, 0, 0, 0);

                this.ScrollViewer.ScrollToHorizontalOffset(position);
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void PlayPause(object sender, RoutedEventArgs e)
        {
            PlayPause();
        }

        private void PlayPause()
        {
            IsPlaying = !IsPlaying;
            Playback.PlayPause();
        }

        private void Stop(object sender, RoutedEventArgs e)
        {
            IsPlaying = false;
            Playback.Stop();
            ScrollTo(0);
        }

        private void ScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollTo(ScrollViewer.HorizontalOffset);
        }

        private void ScrollViewer_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsPlaying)
            {
                PlayPause();
            }
        }

        private void Caption_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsPlaying)
            {
                return;
            }

            var caption = (sender as Border).DataContext as Caption;
            Timeline.SelectCaption(caption);

            whenCaptionSelected.OnNext(caption);
        }

        private void Caption_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.SizeWE;
        }

        private void Caption_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Cursor = null;
        }

        public void DeleteSelectedCaption()
        {
            Timeline.DeleteSelectedCaption();
        }
    }
}
