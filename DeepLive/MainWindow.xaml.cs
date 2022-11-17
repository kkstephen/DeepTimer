using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DeepCore;
using UnitODB;

namespace DeepLive
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    { 
        private DeepClient client;
        private DeepZoom liveZoom;
        private System.Timers.Timer _timer;

        private IList<DeepMatch> data;

        private string clientName;
               
        private bool is_connect = false;
         
        public MainWindow()
        {
            InitializeComponent();

            this._timer = new System.Timers.Timer(3 * 1000);
            this._timer.Elapsed += _timer_Elapsed; 

            this.liveZoom = new DeepZoom();

            this.data = new List<DeepMatch>();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.is_connect)
            {
                this.disconnect(); 
            }
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!this.is_connect)
            {
                return;
            }

            try
            {
                this.client.KeepAlive(); 
                
                IList<DeepMatch> rs = this.liveZoom.Decode(this.client.Json); 

                if (this.list_update(rs))
                { 
                    this.data.Clear();

                    this.data.AddRange(rs);

                    this.liveZoom.Refresh(rs);
                }
            }
            catch(Exception ex)
            {  
                this.disconnect();
                
                MessageBox.Show(ex.Message, "DeepTimer", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool list_update(IList<DeepMatch> list)
        {
            bool is_update = false;

            if (this.data.Count != list.Count)
            {
                is_update = true;
            }
            else
            {
                for (int i = 0; i < this.data.Count; i++)
                { 
                    if (this.data[i].Lap.Id != list[i].Lap.Id)
                    {
                        is_update = true;

                        break;
                    }  
                }
            }

            return is_update;
        }

        private void btn_connect_Click(object sender, RoutedEventArgs e)
        {
            this.client = new DeepClient()
            {
                Timeout = 10,
                Host = this.tb_host.Text,
                Port = int.Parse(this.tb_port.Text),
                ClientName = this.clientName                
            }; 

            this.client.OnOpen += Client_OnOpen;
            this.client.OnClose += Client_OnClose;
            this.client.OnReceived += Client_OnReceived;

            try
            {
                this.client.Connect();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "DeepLive", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void disconnect()
        {
            if (this.is_connect)
            { 
                try
                {
                    this.client.Logout();
                }
                catch
                {
                }
            }

            if (this.client != null)
            { 
                this.client.Close(); 
                
                this.client.OnOpen -= Client_OnOpen;
                this.client.OnClose -= Client_OnClose;
                this.client.OnReceived -= Client_OnReceived;

                this.client.Dispose();
                this.client = null;
            } 
            
            this.is_connect = false;

            this.setLog("disconnect.");
        }   
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.clientName = System.Environment.MachineName;
        } 
 
        private void Client_OnClose(object sender, CFS.Net.SessionCloseEventArgs e)
        {
            if (this._timer != null)
            {
                this._timer.Stop();
            }
        }

        private void Client_OnOpen(object sender, CFS.Net.SessionOpenEventArgs e)
        {
            this.data.Clear();

            try
            { 
                this.client.Hello(); 
                
                //keep live
                this._timer.Start();
                
                this.is_connect = true;

                this.setLog("connect server ok.");
            }
            catch (Exception ex)
            {
                this.disconnect();

                MessageBox.Show(ex.Message, "DeepLive", MessageBoxButton.OK, MessageBoxImage.Error);
            }  
        }

        private void Client_OnReceived(object sender, CFS.Net.SessionDataEventArgs e)
        { 
            this.setLog(DateTime.Now.ToString() + ": receive " + e.Count); 
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            this.disconnect(); 
        }

        private void setLog(string str)
        {
            this.Dispatcher.InvokeAsync(() => {
                this.lb_status.Text = str;
            });
        }

        private void btn_live_Click(object sender, RoutedEventArgs e)
        { 
            Display dialog = new Display();

            dialog.Broadcast = this.liveZoom;

            dialog.Show();
        }
    }
}
