using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Threading;

namespace Timeline
{
    public sealed class TimelineData
    {
        private readonly TimelineLayout _layout;
        private readonly TimelinePlayback _playback;
        public ObservableCollection<Caption> Captions { get; set; }

        private Caption lastSelectedCaption = null;

        public Caption PlaybackCaption
        {
            get
            {
                int count = 0;
                foreach (var caption in Captions)
                {
                    if(caption.StartTime <= _playback.Value && _playback.Value < caption.EndTime)
                    {
                        SelectCaption(caption);

                        return caption;
                    }

                    if (caption.StartTime > _playback.Value)
                    {
                        return null;
                    }
                }

                return null;
            }
        }

        public void SelectCaption(Caption caption)
        {
            if (caption != lastSelectedCaption)
            {
                if (lastSelectedCaption != null)
                {
                    lastSelectedCaption.IsSelected = false;
                }

                caption.IsSelected = true;
                lastSelectedCaption = caption;
            }
        }

        private int? lastSession = null;

        private TimeSpan sessionPosition;

        public void SetCaptionStart(Caption caption, TimeSpan start, int dragSession)
        {
            var next = NextCaption(caption);

            TimeSpan start2;

            if (lastSession == null || dragSession != lastSession.Value)
            {
                start2 = caption.StartTime;
                sessionPosition = start2;
                lastSession = dragSession;
            }
            else
            {
                start2 = sessionPosition;
            }

            var diff = start - start2;

            diff = TimeSpan.FromMilliseconds(Math.Max(0, diff.TotalMilliseconds));

            var marginDiff = caption.LeftMargin - diff;

            if (next != null && next.LeftMargin + marginDiff < TimeSpan.Zero)
            {
                diff += (next.LeftMargin + marginDiff);
            }

            marginDiff = caption.LeftMargin - diff;
            caption.LeftMargin = diff;
            sessionPosition += caption.LeftMargin - diff;

            if (next != null)
            {
                next.LeftMargin += marginDiff;
            }
        }

        public TimelineData(TimelineLayout layout, TimelinePlayback playback)
        {
            _layout = layout;


            _playback = playback;
            Captions = new ObservableCollection<Caption>();
        }


        public void AddCaption(Caption caption)
        {
            Captions.Add(caption);
        }

        public static TimelineData FakeData(TimelineLayout layout, TimelinePlayback playback)
        {
            var timeline = new TimelineData(layout, playback);

            for (var i = 0; i < 30; i++)
            {
                timeline.AddCaption(Caption.FakeData(layout, timeline));
            }

            return timeline;
        }

        public void DeleteSelectedCaption()
        {
            if (lastSelectedCaption == null)
            {
                return;
            }


            var next = NextCaption(lastSelectedCaption);
            if (next != null)
            {
                next.LeftMargin += lastSelectedCaption.LeftMargin + lastSelectedCaption.MarkerDuration;
            }

            this.Captions.Remove(lastSelectedCaption);

            next.LayoutUpdated();


            lastSelectedCaption.Dispose();
            lastSelectedCaption = null;


            //var elapsed = TimeSpan.Zero;
            //foreach (var caption in Captions)
            //{
            //    caption.LeftMargin = caption.StartTime - elapsed;
            //    elapsed += caption.LeftMargin + caption.MarkerDuration;
            //}
        }

        public Caption NextCaption(Caption caption)
        {
            var idx = Captions.IndexOf(caption);

            if (idx >= Captions.Count - 1)
            {
                return null;
            }

            return Captions[idx + 1];
        }

        public Caption PreviousCaption(Caption caption)
        {
            var idx = Captions.IndexOf(caption);

            if (idx == 0)
            {
                return null;
            }

            return Captions[idx - 1];
        }
    }
}