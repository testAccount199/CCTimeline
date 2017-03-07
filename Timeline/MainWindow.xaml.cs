using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

            this.root.WhenPlaybackCaptionChanged.Subscribe(caption =>
            {
                if (caption != null)
                {
                    root.CaptionValue = caption.Text;
                }
                else
                {
                    root.CaptionValue = "";
                }

            });

            IDisposable subscription;

            subscription = this.root.WhenCaptionSelected.Subscribe(caption =>
            {
                caption.UpdateText(this.root.WhenCaptionValueChanged);

                root.CaptionValue = caption.Text;
            });
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    ///TODO:
    /// Insert new caption
}
