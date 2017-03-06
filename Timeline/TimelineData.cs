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
                foreach (var caption in Captions)
                {
                    if (caption.EndTime >= _playback.Value && caption.StartTime <= _playback.Value)
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

        private void PrintCaptions(int count)
        {
            for (var i = 0; i < Math.Min(count, Captions.Count); i++)
            {
                Debug.Print($"{i}:");
                Debug.Print(Captions[i].ToString());
            }
        }

        ///Update on layout changed event
        public void Update()
        {
            var elapsed = TimeSpan.Zero;
            foreach (var caption in Captions)
            {
                caption.StartTime = elapsed + caption.LeftMargin;
                elapsed += caption.MarkerDuration;
            }
        }

        public void SetCaptionStart(Caption caption, TimeSpan start)
        {
            var next = NextCaption(caption);
            var diff = start - caption.StartTime;

            diff = TimeSpan.FromMilliseconds(Math.Max(0, diff.TotalMilliseconds));

            var marginDiff = caption.LeftMargin - diff;

            if (next != null && next.LeftMargin + marginDiff < TimeSpan.Zero)
            {
                diff += (next.LeftMargin + marginDiff);
            }

            marginDiff = caption.LeftMargin - diff;

            caption.LeftMargin = diff;
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
            Update();
        }

        public static TimelineData FakeData(TimelineLayout layout, TimelinePlayback playback)
        {
            var timeline = new TimelineData(layout, playback);

            for (var i = 0; i < 30; i++)
            {
                timeline.AddCaption(Caption.FakeData(layout));
            }

            return timeline;
        }

        public void DeleteSelectedCaption()
        {
            if (lastSelectedCaption == null)
            {
                return;
            }


            this.Captions.Remove(lastSelectedCaption);
            lastSelectedCaption.Dispose();
            lastSelectedCaption = null;

            var elapsed = TimeSpan.Zero;
            foreach (var caption in Captions)
            {
                caption.LeftMargin = caption.StartTime - elapsed;
                elapsed += caption.LeftMargin + caption.MarkerDuration;
            }
        }

        public Caption NextCaption(Caption draggingCaption)
        {
            var idx = Captions.IndexOf(draggingCaption);

            if (idx >= Captions.Count - 1)
            {
                return null;
            }

            return Captions[idx + 1];
        }
    }
}