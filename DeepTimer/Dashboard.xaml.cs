using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls; 
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Diagnostics; 
using System.Windows.Media.Imaging;
using System.Windows.Media;
using DeepCore;
using System.Runtime.ConstrainedExecution;

namespace DeepTimer
{
    /// <summary>
    /// Interaction logic for Timer.xaml
    /// </summary>
    public partial class Dashboard : Window
    { 
        private WindowState oldstate;
        public DeepRacer Racer { get; set; }

        private Thread watchdog;

        private Stopwatch sw;
        private System.Timers.Timer timer;

        private TimeSpan elapse_time;

        private long last_elapse;

        private volatile bool is_end = true;
        private volatile bool is_start = false;
       
        public Dashboard()
        {
            InitializeComponent(); 

            this.sw = new Stopwatch();
            this.sw.Reset(); 


            this.timer = new System.Timers.Timer();

            this.timer.Interval = 100;
            this.timer.Elapsed += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //count down 
            this.Dispatcher.InvokeAsync(updateTimer);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Racer != null)
            {
                this.Racer.OnStart += Racer_OnStart;
                this.Racer.OnStop += Racer_OnStop;
                this.Racer.OnReset += Racer_OnReset;
                this.Racer.OnTouch += Racer_OnTouch;  

                this.Racer.Ready();
            }
        } 

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!this.is_end)
            {
                e.Cancel = true;

                return;
            }

            this.timer.Close();
            this.timer = null;

            if (this.Racer != null)
            {
                this.Racer.OnStart -= Racer_OnStart;
                this.Racer.OnStop -= Racer_OnStop;
                this.Racer.OnReset -= Racer_OnReset;
                this.Racer.OnTouch -= Racer_OnTouch; 

                this.Racer.Close();
            } 
        } 

        private void countdown_loop()
        {
            while (!this.is_end)
            {  
                if (this.is_start)
                {
                    this.elapse_time = this.sw.Elapsed - this.Racer.Last_Tick; 
                }

                this.Dispatcher.InvokeAsync(updateTimer);

                Thread.Sleep(10);
            }  

            this.Racer.Finish(DateTime.Now);
            
            if (this.sw.IsRunning)
            {
                this.sw.Stop();
            }

            this.timer.Stop();
        }

        private void updateTimer()
        { 
            this.lb_current.Text = elapse_time.ToString(@"mm\:ss\.ff");
        }

        private void Racer_OnStart(object sender, EventArgs e)
        {
            if (!this.is_end)
            {
                return;
            }

            this.is_end = false;
                       
            this.watchdog = new Thread(new ThreadStart(countdown_loop));

            this.watchdog.Start();  

            this.sw.Start();
            this.timer.Start();
        }

        private void Racer_OnStop(object sender, EventArgs e)
        {
            this.is_end = true; 

            if (this.watchdog.IsAlive)
            {
                this.watchdog.Join();
            }
 
            this.watchdog = null;
        }

        private void Racer_OnTouch(object sender, EventArgs e)
        {
            if (this.is_end) 
                return;

            //time elapsed now
            long ticks_now = this.sw.ElapsedTicks; 
            
            if (!this.is_start)
            {
                this.is_start = true;

                //start time ticks
                this.last_elapse = ticks_now;

                this.Racer.Last_Tick = TimeSpan.FromTicks(ticks_now);

                return;
            }

            //last time span 
            long lap_cur = ticks_now - last_elapse; 

            if (lap_cur < 20000000)
                return;

            this.Racer.Last_Tick = TimeSpan.FromTicks(ticks_now); 
           
            this.last_elapse = ticks_now; 
                
            this.Racer.Lap++;   

            this.Dispatcher.InvokeAsync(() =>
            {
                if (!Racer.TestMode)
                {
                    DeepLap car = new DeepLap() { Lap = this.Racer.Lap,  Team = this.Racer.Team, Record = lap_cur, Date = DateTime.Now, Invalid = false };
                     
                    this.Racer.Log(car);
                }

                this.lb_last.Text = lap_cur.ToTimespan();
                this.lb_lapnum.Text = this.Racer.Lap.ToString();
            });                
        }  

        private void Racer_OnReset(object sender, EventRacerArgs e)
        {
            this.sw.Reset();

            this.is_end = true;
            this.is_start = false;   
            
            this.last_elapse = 0;                    
            this.elapse_time = TimeSpan.FromSeconds(0);
            
            this.lb_current.Text = "00:00.00";       
            
            this.lb_lapnum.Text = this.Racer.Lap.ToString();
            this.lb_last.Text = this.Racer.LastLap.ToTimespan(); 
        } 
    }
}
