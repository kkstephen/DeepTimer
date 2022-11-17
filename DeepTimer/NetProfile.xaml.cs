using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
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
using System.Windows.Media.TextFormatting;

namespace DeepTimer
{
    /// <summary>
    /// Interaction logic for NetProfile.xaml
    /// </summary>
    public partial class NetProfile : Window
    {
        public string Host
        {
            get
            {
                return this.tb_host.Text;
            }
        }

        public string Port
        {
            get
            {
                return this.tb_port.Text;
            }
        }

        public NetProfile()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.tb_host.Text = this.GetLocalIP();
        }

        private string GetLocalIP()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return "0.0.0.0";
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            var ip = host.AddressList.FirstOrDefault(p => p.AddressFamily == AddressFamily.InterNetwork);

            return ip.ToString();
        }
    }
}
