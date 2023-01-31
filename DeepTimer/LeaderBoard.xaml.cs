using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.IO.Ports;
using Microsoft.Win32;
using System.Windows.Media.Animation;
using System.Windows; 
using DeepCore;
using System.Windows.Controls;
using System.Data;
using System.Threading;

namespace DeepTimer
{
    /// <summary>
    /// Interaction logic for LeaderBoard.xaml
    /// </summary>
    public partial class LeaderBoard : Window
    {
        private ObservableCollection<DeepLap> lapCache;
        private Dictionary<string, DeepLap> teamLaps;
        private IList<Team> teams;
        private IList<DeepMatch> bestLaps;

        private SerialPort sensor;
        private Thread _service;

        private DeepServer server;      
        private DeepManager manager;
        private DeepRacer racer; 

        private volatile bool is_stop = true;

        public LeaderBoard()
        {
            InitializeComponent();

            this.manager = DeepManager.Instance;

            this.server = new DeepServer();

            this.server.OnStart += Server_OnStart;
            this.server.OnStop += Server_OnStop;
            
            this.racer = new DeepRacer();

            this.racer.OnLoad += Racer_OnReady;
            this.racer.OnRecord += Racer_OnRecord;
            this.racer.OnFinish += Racer_OnFinish;
            this.racer.OnClose += Racer_OnDispose; 

            this.lapCache = new ObservableCollection<DeepLap>();
            this.lapCache.CollectionChanged += Cache_CollectionChanged;

            this.listView.ItemsSource = this.lapCache;

            this.teamLaps = new Dictionary<string, DeepLap>();
            this.teams = new List<Team>();
            this.bestLaps = new List<DeepMatch>();
            
            this.cbTeam.ItemsSource = this.teams;
            this.rankView.ItemsSource = this.bestLaps;

            List<int> secs = new List<int>();

            for(int i = 0; i <= 60; i++) 
            {
                secs.Add(i);
            }

            this.cbSecs.ItemsSource = secs; 
        }

        private void Cache_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.Dispatcher.Invoke(() => { 
                this.listView.Items.Refresh(); 
            }); 
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.manager.Database = ConfigurationManager.AppSettings["DB"].ToString();

            try
            {
                if (!File.Exists(this.manager.Database))
                {
                    manager.Init();
                }

                this.loadData();

                this.Dispatcher.InvokeAsync(() => {
                    this.lbDataName.Text = "Databsae: " + this.manager.Database;
                    this.chk_update.IsChecked = this.manager.AutoRanking;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Database", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!this.is_stop || this.server.IsRunning)
            {
                e.Cancel = true;

                return;
            }

            this.stopLive();

            this.DeactivateSensor();

            if (this.racer != null)
            {
                this.racer.OnLoad -= Racer_OnReady;
                this.racer.OnRecord -= Racer_OnRecord;
                this.racer.OnFinish -= Racer_OnFinish; 

                this.racer = null;
            }

            this.manager.Dispose();
        }

        private void Server_OnStop(object sender, CFS.Net.ServerStopEventArgs e)
        {
            if (this._service != null)
            {
                if (this._service.IsAlive)
                {
                    this._service.Join();
                }

                this._service = null;
            }

            try
            {
                this.server.Clear();
            }
            catch (Exception ex)
            {
                this.setLog(ex.Message);
            }

            this.setLog("Live streaam stop.");
        }

        private void Server_OnStart(object sender, CFS.Net.ServerStartEventArgs e)
        {
            try
            {
                //main loop
                this._service = new Thread(new ThreadStart(this.server.Run));
                this._service.Start();

                this.setLog("Live stream start.");
            }
            catch (Exception ex)
            {
                this.setLog(ex.Message);
            }
        }

        private void ActivateSensor(string port, int rates)
        {
            try
            {
                if (string.IsNullOrEmpty(port))
                    throw new ArgumentNullException("port");

                this.sensor = new SerialPort(port, rates);

                this.sensor.Handshake = Handshake.None;
                this.sensor.ReadTimeout = 1000;
                this.sensor.WriteTimeout = 1000;
                this.sensor.ReadBufferSize = 1024;

                this.sensor.DataReceived += port_received;

                if (!this.sensor.IsOpen)
                {
                    this.sensor.Open();

                    this.Dispatcher.InvokeAsync(() => {
                        this.lb_port.Text = port;
                    });
                }
                else
                {
                    throw new Exception("open port error");
                }
            }
            catch (Exception ex)
            {
                if (this.sensor != null)
                {
                    this.sensor.Dispose();
                    this.sensor = null;
                }

                MessageBox.Show(ex.Message, "Serial Port", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeactivateSensor()
        {
            if (this.sensor == null)
                return;

            try
            {
                // Close the active scanner
                this.sensor.DataReceived -= port_received;

                this.sensor.Close();
            }
            finally
            {
                this.sensor.Dispose();
                this.sensor = null;
            }
        }

        private void port_received(object sender, SerialDataReceivedEventArgs e)
        {  
            try
            {
                string data = this.sensor.ReadLine();

                if (!this.is_stop)
                {
                    this.racer.Tap();
                }
                
                this.racer.Activity(data); 
            }
            catch (Exception ex)
            {
                this.Dispatcher.InvokeAsync(() => 
                {
                    this.lb_status.Text = "Error: " + ex.Message;
                });
            }
        }

        private void Racer_OnReady(object sender, EventArgs e)
        {
            this.btn_start.IsEnabled = true;
            this.btn_stop.IsEnabled = true;
            this.btn_reset.IsEnabled = true;

            this.reset_timer();
        }

        private void Racer_OnDispose(object sender, EventArgs e)
        {
            this.btn_start.IsEnabled = false;
            this.btn_stop.IsEnabled = false;
            this.btn_reset.IsEnabled = false;
        }

        private void Racer_OnFinish(object sender, EventFinArgs e)
        {
            this.is_stop = true;

            this.Dispatcher.InvokeAsync(() => {
                this.lb_status.Text = "Finish on " + e.Date.ToString();
            });
        }

        private void Racer_OnRecord(object sender, EventRecordArgs e)
        {
            try
            {
                this.manager.Save(e.Lap);

                this.Dispatcher.InvokeAsync(() => {
                    this.lapCache.Add(e.Lap);

                    if (this.updateTeamLaps(e.Lap))
                    {
                        this.updateRanking(); 
                    }
                });

            }
            catch (Exception ex)
            {
                this.Dispatcher.InvokeAsync(() => {
                    this.lb_status.Text = "Error: " + ex.Message;
                });
            }
        }

        private void setLog(string message)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                this.lb_log.Text = message;
            });
        }

        private void loadData()
        {
            this.lapCache.Clear();
            this.teamLaps.Clear();

            foreach (var p in this.manager.Unit.Cars)
            {
                this.lapCache.Add(p);
                 
                this.updateTeamLaps(p);  
            } 

            this.setTeamLaps();
        }

        private void setTeamLaps()
        {
            foreach (var t in teams)
            {
                if (!this.teamLaps.ContainsKey(t.Name))
                {
                    DeepLap p = new DeepLap() { Team = t.Name, Record = 0, Invalid = false };

                    this.teamLaps.Add(t.Name, p);
                }
            }

            this.updateRanking();
        } 

        private bool updateTeamLaps(DeepLap p)
        {
            if (this.teamLaps.ContainsKey(p.Team))
            {
                var item = this.teamLaps[p.Team];

                if (item.Record <= 0 || item.Record > p.Record)
                {
                    this.teamLaps[p.Team] = p;

                    return true;
                }
            } 
            else
            {
                this.teamLaps.Add(p.Team, p);

                return true;
            }

            return false;
        }

        private void updateRanking()
        {
            this.bestLaps.Clear();

            int i = 1;

            foreach (var item in this.teamLaps.Values)
            {                
                DeepMatch c = new DeepMatch() { No = i++, Lap = item };

                this.bestLaps.Add(c);
            }

            if (this.manager.AutoRanking)
            {
                this.sortLaps();
            }

            this.rankView.Items.Refresh();
        }

        private void sortLaps()
        {  
            IList<DeepMatch> laps = this.bestLaps.OrderBy(x => x.Lap.Record > 0 ? 0 : 1).ThenBy(x => x.Lap.Record).ToList();
            
            int i = 1;

            foreach (var item in laps)
            {
                item.No = i++;  
            }
 
            this.manager.Ranklist = laps;             
        }

        private void reset_timer()
        { 
            this.racer.Team = this.cbTeam.Text;

            var ds = this.lapCache.Where(p => p.Team == this.racer.Team).ToList();

            //total lap
            this.racer.Lap = ds.Count;

            this.racer.BestLap = 0;
            this.racer.LastLap = 0;

            if (this.racer.Lap > 0)
            {
                //sort lap
                var f = ds.Where(p => p.Invalid == false).OrderBy(t => t.Record).FirstOrDefault();

                if (f != null)
                {
                    this.racer.BestLap = f.Record;
                }
            }

            //reset timer duration
            int m = int.Parse(this.cb_down.Text);
            int s = int.Parse(this.cbSecs.Text);
           
            int duration = m * 60 + s;

            this.racer.Reset(duration); 
        } 

        private void btn_export_Click(object sender, RoutedEventArgs e)
        {
            if (this.lapCache == null || this.lapCache.Count == 0)
            {
                MessageBox.Show("No data to export.", "DeepTimer", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog();

            dialog.Filter = "EXCEL (*.xlsx)|*.xlsx";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    ExcelNPOI excel = new ExcelNPOI(this.lapCache);

                    excel.Save(dialog.FileName);

                    MessageBox.Show("Export OK.", "DeepTimer", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "DeepTimer", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.loadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "DeepTimer", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            if (!this.is_stop)
                return;
                         
            this.racer.Start();

            this.is_stop = false;

            this.Dispatcher.InvokeAsync(() => {
                this.lb_status.Text = "Ready to go.";
            });
        }

        private void btn_stop_Click(object sender, RoutedEventArgs e)
        {
            this.is_stop = true;

            if (this.racer.IsRunning)
            {
                this.racer.Stop();
            }

            this.Dispatcher.InvokeAsync(() => {
                this.lb_status.Text = "Stop on " + DateTime.Now.ToString();
            });
        }

        private void btn_reset_Click(object sender, RoutedEventArgs e)
        {
            if (!this.racer.IsRunning)
            {
                this.reset_timer();

                this.Dispatcher.InvokeAsync(() =>
                {
                    this.lb_status.Text = "Reset.";
                });
            }
        }

        private void btn_sensor_Click(object sender, RoutedEventArgs e)
        {
            this.DeactivateSensor();

            SensorWin dialog = new SensorWin();

            dialog.Owner = this;

            dialog.ShowDialog();

            if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
            {
                if (!string.IsNullOrEmpty(dialog.Port))
                {
                    this.ActivateSensor(dialog.Port, 9600);
                }
            }

            dialog.Close();
        }

        private void btn_timer_Click(object sender, RoutedEventArgs e)
        { 
            if (this.teams.Count == 0)
            {
                MessageBox.Show("No team found!");

                return;
            }
           
            LapTimer lpwin = new LapTimer();

            lpwin.Racer = this.racer;

            lpwin.Show();             
        }

        private void btn_stopwatch_Click(object sender, RoutedEventArgs e)
        {
            TimeWatch watch = new TimeWatch();
             
            watch.Racer = this.racer;

            watch.Show();
        }

        private void btn_tigger_Click(object sender, RoutedEventArgs e)
        {
            if (this.racer.IsRunning)
            {
                this.racer.Tap();
            }
        } 

        private void btn_bg_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Image Files(*.bmp;*.jpg;*.png)|*.bmp;*.jpg;*.png|All files (*.*)|*.*";

            if (dialog.ShowDialog() == true)
            {
                this.racer.DrawImage(dialog.FileName);
                
                this.Dispatcher.InvokeAsync(() => {
                    this.lb_status.Text = "Set background: " + dialog.SafeFileName;
                });
            }
        } 

        private void btn_monitor_Click(object sender, RoutedEventArgs e)
        {
            if (this.racer != null)
            {
                Monitor dialog = new Monitor();

                dialog.Racer = this.racer;

                dialog.Show();
            }
        }

        private void listView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
           
        }

        private void menu_dnf_Click(object sender, RoutedEventArgs e)
        { 
            var item = this.listView.SelectedItem as DeepLap;

            item.Invalid = !item.Invalid;
            
            this.manager.Save(item);

            if (item.Invalid)
            {
                this.teamLaps.Remove(item.Team);
            }

            DeepLap p = new DeepLap() { Lap = 0, Invalid = true, Team = item.Team, Record = 0 };

            var laps = this.lapCache.Where(t => t.Team == item.Team && !t.Invalid).OrderBy(x => x.Record).ToList();

            if (laps.Count > 0)
            {
                p = laps[0]; 
            }

            if (this.updateTeamLaps(p)) 
            { 
                this.updateRanking();
                this.racer.Revise(p.Record);
            } 
        }

        private void revise_Click(object sender, RoutedEventArgs e)
        {
            var item = this.listView.SelectedItem as DeepLap;

            ReviseDialog dialog = new ReviseDialog();

            dialog.Ticks = item.Record;

            if (dialog.ShowDialog() == true)
            {
                item.Record = dialog.Ticks;

                if (item.Record != 0)
                {
                    this.manager.Save(item);

                    if (this.updateTeamLaps(item))
                    {
                        this.updateRanking();
                    }
                }
                else
                {
                    MessageBox.Show("Record can not be 0!");
                }
            } 
        }

        private void btn_team_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Excel 2013|*.xlsx|Excel 2007|*.xls|All files (*.*)|*.*";
 
            if (dialog.ShowDialog() == true)
            { 
                this.teams.Clear();

                try
                {
                    var table = ExcelNPOI.ImportXls(dialog.FileName);
                    
                    int n = 1;
                    
                    foreach (DataRow dr in table.Rows)
                    {
                        Team t = new Team();

                        t.Name = dr["Name"].ToString();
                        t.Id = n++;
                    
                        this.teams.Add(t); 
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "DeepTimer", MessageBoxButton.OK, MessageBoxImage.Error);
                } 
                
                this.cbTeam.Items.Refresh();

                this.cbTeam.SelectedIndex = 0;

                this.setTeamLaps();
            }
        } 

        private void new_database_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();

            dialog.Filter = "SQLite (*.db3)|*.db3";

            if (dialog.ShowDialog() == true)
            {
                this.lapCache.Clear();
                this.teamLaps.Clear();

                this.setTeamLaps();

                this.manager.Database = dialog.FileName;

                try
                {
                    this.manager.Dispose();

                    manager.Init();

                    this.Dispatcher.InvokeAsync(() => {
                        this.lbDataName.Text = "Databsae: " + dialog.SafeFileName;
                    });

                    MessageBox.Show("Create DB OK.", "DeepTimer", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Database", MessageBoxButton.OK, MessageBoxImage.Error);
                } 
            }
        }

        private void open_database_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "SQLite 3|*.db3|All files (*.*)|*.*";

            if (dialog.ShowDialog() == true)
            { 
                this.manager.Database = dialog.FileName;
                
                try
                {
                    this.manager.Dispose();

                    this.loadData();

                    this.Dispatcher.InvokeAsync(() => {
                        this.lbDataName.Text = "Databsae: " + dialog.SafeFileName;
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Database", MessageBoxButton.OK, MessageBoxImage.Error);
                } 
            }
        }

        private void server_start_Click(object sender, RoutedEventArgs e)
        { 
            if (this.server.IsRunning)
                return;

            NetProfile dialog = new NetProfile();

            dialog.ShowDialog();

            if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
            {
                this.server.Host = dialog.Host;
                this.server.Port = int.Parse(dialog.Port); 

                this.server.Start();
            } 
        }

        private void server_stop_Click(object sender, RoutedEventArgs e)
        {
            this.stopLive();
        }

        private void stopLive()
        {
            if (this.server.IsRunning)
            { 
                try
                {
                    this.server.Stop();
                }
                catch (Exception ex)
                { 
                    this.setLog(ex.Message);
                }
            } 
        }

        private void btn_update_Click(object sender, RoutedEventArgs e)
        {
            this.sortLaps();
        }        

        private void chk_update_Click(object sender, RoutedEventArgs e)
        {
            this.manager.AutoRanking = this.chk_update.IsChecked ?? false;
        }
    }
}
