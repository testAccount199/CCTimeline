using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Timeline.Annotations;

namespace Timeline
{
    /// <summary>
    /// Interaction logic for Overlay.xaml
    /// </summary>
    public partial class Overlay : Window, INotifyPropertyChanged
    {
        public Overlay()
        {
            InitializeComponent();
        }

        public void Set(double val)
        {
            Dispatcher.Invoke(() =>
            {
                PlayerHead.Margin = new Thickness(ActualWidth * val, 0, 0, 0);
            });
        }

        private string captionValue;
        public string CaptionValue
        {
            get { return captionValue; }
            set
            {
                captionValue = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
