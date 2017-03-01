using System;
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
using Color = System.Drawing.Color;

namespace Timeline
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();

            this.root.WhenTimeChanged.Subscribe(time =>
            {


            });

            this.root.WhenPlaybackCaptionChanged.Subscribe(caption =>
            {
                if (caption != null)
                {
                    overlay.CaptionValue = caption.Text;
                }
                else
                {
                    overlay.CaptionValue = "";
                }

            });

            this.root.WhenCaptionSelected.Subscribe(caption =>
            {
                caption.UpdateText(overlay.WhenCaptionValueChanged);

                overlay.CaptionValue = caption.Text;
            });

            overlay.Show();

        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && !this.root.IsPlaying)
            {

                var result = MessageBox.Show(this, "Are you sure you want to delete the selected caption?", "Warning", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    this.root.DeleteSelectedCaption();
                }
            }
        }

        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            PositionOverlay();
        }

        private Overlay overlay = new Overlay();


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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    ///TODO:
    /// Insert new caption
    /// Drag caption
    /// video playback
}
