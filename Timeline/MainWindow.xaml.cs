﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio.Gui;
using NAudio.Wave;
using Timeline.Annotations;

namespace Timeline
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
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
            }
        }

        public MainWindow()
        {
            TimelineLayout = new TimelineLayout();
            Playback = new TimelinePlayback(TimeSpan.FromMinutes(3));
            Timeline = TimelineData.FakeData(TimelineLayout, Playback);

            InitializeComponent();

            Playback.WhenTimeChanged.Subscribe(time =>
            {
                var scrollPosition = TimelineLayout.GetScrollPosition(time);

                var playerHeadPosition = TimelineLayout.GetPlayerHeadPosition(time);

                overlay.Set(playerHeadPosition);

                ScrollTo(scrollPosition);

                var caption = Timeline.SelectedCaption;

                if (caption != null)
                {
                    overlay.CaptionValue = caption.Text;
                }
                else
                {
                    overlay.CaptionValue = "";
                }

                Thread.Sleep(100);
            });

            var viewer = new WaveViewer();
            viewer.WaveStream = new WaveFileReader(@"C:\Users\Amichai\Desktop\The Zahir.wav");

            this.Host.Child = viewer;

            TimelineLayout.WhenZoomChanged.Subscribe(zoom =>
            {
                var scale = TimelineLayout.Zoom/100f;
                viewer.SamplesPerPixel = (int)Math.Round(128 / scale);
            });

            overlay.Show();
        }

        private Overlay overlay = new Overlay();

        private void ScrollTo(double position)
        {
            Dispatcher.Invoke(() =>
            {
                this.WaveformBorder.Margin = new Thickness(-position, 0, 0, 0);

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

        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            PositionOverlay();
        }

        private void PositionOverlay()
        {
            if (this.Width > 14)
            {
                overlay.Width = this.Width - 14;
            }
            if (this.Height > 220)
            {
                overlay.Height = this.Height - 220;
            }
            overlay.Left = this.Left + 8;
            overlay.Top = this.Top + 80;
            overlay.Owner = this;

            overlay.Activate();
        }

        private void MainWindow_OnLocationChanged(object sender, EventArgs e)
        {
            PositionOverlay();
        }

        private void ScrollViewer_OnMouseDown(object sender, MouseButtonEventArgs e)
        {

            if (IsPlaying)
            {
                PlayPause();
            }
        }
    }
}
