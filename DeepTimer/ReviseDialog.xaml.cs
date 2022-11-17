using System;
using System.Collections.Generic;
using System.Linq;
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

namespace DeepTimer
{
    /// <summary>
    /// Interaction logic for ReviseDialog.xaml
    /// </summary>
    public partial class ReviseDialog : Window
    {
        private long ticks;
        public long Ticks 
        { 
            get
            {
                int m = int.Parse(this.tb_min.Text);
                int s = int.Parse(this.tb_sec.Text);
                int ns = int.Parse(this.tb_ns.Text);

                try
                {

                    TimeSpan t = new TimeSpan(0, 0, m, s, ns);

                    this.ticks = t.Ticks;
                }
                catch
                {
                    this.ticks = 0;
                }

                return this.ticks;
            }

            set
            {
                this.ticks = value;               
            }
        }

        public ReviseDialog()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TimeSpan t = TimeSpan.FromTicks(this.ticks);

            this.tb_min.Text = t.Minutes.ToString();
            this.tb_sec.Text = t.Seconds.ToString();
            this.tb_ns.Text = t.Milliseconds.ToString();
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
