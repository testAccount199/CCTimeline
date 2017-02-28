using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using Timeline;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var a = new System.Windows.Controls.UserControl();
            var s = new StackPanel() {Background = System.Windows.Media.Brushes.Aquamarine};
            a.Content = s;
            s.Children.Add(new TextBlock() {Text = "testing"});

            this.elementHost1.Child = new RootControl();
        }
    }
}
