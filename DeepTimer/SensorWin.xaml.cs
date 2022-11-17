using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO.Ports;

namespace DeepTimer
{
    /// <summary>
    /// Interaction logic for SensorWin.xaml
    /// </summary>
    public partial class SensorWin : Window
    {
        public string Port { get; set; }
        public SensorWin()
        {
            InitializeComponent();

            var list = SerialPort.GetPortNames();

            this.cp_box.ItemsSource = list;

            if (list.Length > 0)
            {
                this.cp_box.SelectedIndex = 0;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Port = this.cp_box.SelectedValue?.ToString();

            this.DialogResult = true;
        }
    }
}
