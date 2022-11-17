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
using DeepCore;

namespace DeepTimer
{
    /// <summary>
    /// Interaction logic for Monitor.xaml
    /// </summary>
    public partial class Monitor : Window
    {
        public DeepRacer Racer { get; set; }

        public Monitor()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Racer != null)
            {
                this.Racer.OnObserve += Racer_OnObserve; 
            }
        }

        private void Racer_OnObserve(object sender, EventActivityArgs e)
        {
            this.Dispatcher.InvokeAsync(() => {
                this.tb_activity.AppendText(DateTime.Now.ToString() + ": " + e.Data + "\n");            
            }); 
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.Racer != null)
            {
                this.Racer.OnObserve -= Racer_OnObserve;
            }
        }
    }
}
