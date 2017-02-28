using System;
using System.Collections.ObjectModel;

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

        ///Update on layout changed event
        private void Update()
        {
            var elapsed = TimeSpan.Zero;
            foreach (var caption in Captions)
            {
                caption.StartTime = elapsed;
                elapsed += caption.LeftMargin + caption.MarkerDuration;
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
    }
}