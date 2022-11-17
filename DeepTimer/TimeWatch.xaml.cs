using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Diagnostics;
using DeepCore;

namespace DeepTimer
{
    /// <summary>
    /// Interaction logic for TimeWatch.xaml
    /// </summary>
    public partial class TimeWatch : Window
    {
        public DeepRacer Racer { get; set; } 

        private Thread runable;
        private Stopwatch sw;

        private TimeSpan progress;
        private TimeSpan reminding; 
 
        private WindowState oldstate; 
 
        private volatile bool is_end = true;
 
        public TimeWatch()
        {
            InitializeComponent(); 

            this.progress = TimeSpan.FromSeconds(3 * 60);
            this.reminding = TimeSpan.FromSeconds(3 * 60);

            this.sw = new Stopwatch(); 
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Racer != null)
            {
                this.Racer.OnStart += Racer_OnStart;
                this.Racer.OnStop += Racer_OnStop;
                this.Racer.OnReset += Racer_OnReset;

                this.Racer.Ready();
            }

            if (!String.IsNullOrEmpty(DeepManager.Instance.Wallpaper))
            {
                ImageBrush myBrush = new ImageBrush();

                Image image = new Image();
                image.Source = new BitmapImage(new Uri(DeepManager.Instance.Wallpaper));

                myBrush.ImageSource = image.Source;

                this.board.Background = myBrush;
            }

            this.updateTimer();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!this.is_end)
            {
                e.Cancel = true;

                return;
            }

            if (this.Racer != null)
            {
                this.Racer.OnStart -= Racer_OnStart;
                this.Racer.OnStop -= Racer_OnStop;
                this.Racer.OnReset -= Racer_OnReset;

                this.Racer.Close();
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.F11)
            {
                this.oldstate = WindowState;

                WindowState = WindowState.Maximized;
                Visibility = Visibility.Collapsed;
                WindowStyle = WindowStyle.None;
                ResizeMode = ResizeMode.NoResize;
                Visibility = Visibility.Visible;
            }

            if (e.Key == System.Windows.Input.Key.Escape)
            {
                WindowState = oldstate;
                WindowStyle = WindowStyle.SingleBorderWindow;
                ResizeMode = ResizeMode.CanResize;
            }
        }

        private void updateTimer()
        {
            //count down 
            this.lb_time.Text = this.reminding.ToString(@"mm\:ss");
        }

        private void timer_loop()
        {
            //start stopwatch
            this.sw.Start();

            while (!this.is_end)
            { 
                if (sw.Elapsed.TotalMilliseconds >= progress.TotalMilliseconds)
                {
                    this.is_end = true; 

                    this.sw.Stop();
                    
                    this.Racer.Finish(DateTime.Now);
                } else
                {
                    this.reminding = progress - sw.Elapsed;
                }

                this.Dispatcher.InvokeAsync(this.updateTimer);

                Thread.Sleep(100);
            }

            if (this.sw.IsRunning)
            {
                this.sw.Stop();
            }
        }

        private void Racer_OnStart(object sender, EventArgs e)
        {
            if (!this.is_end)
            {
                return;
            }

            this.is_end = false;

            this.runable = new Thread(new ThreadStart(timer_loop));

            this.runable.Start(); 
        }

        private void Racer_OnStop(object sender, EventArgs e)
        {  
            this.is_end = true; 

            if (this.runable.IsAlive)
            {
                this.runable.Join();
            }

            this.runable = null;
        }

        private void Racer_OnReset(object sender, EventRacerArgs e)
        {
            this.sw.Reset();

            this.is_end = true;  

            this.progress = TimeSpan.FromSeconds(e.Duration);
            this.reminding = TimeSpan.FromSeconds(e.Duration);

            this.updateTimer();
        } 
    }
}
