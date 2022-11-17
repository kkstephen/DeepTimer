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

namespace DeepLive
{
    /// <summary>
    /// Interaction logic for Display.xaml
    /// </summary>
    public partial class Display : Window
    {
        private WindowState oldstate; 
        public DeepZoom Broadcast { get; set; }
        
        public Display()
        {
            InitializeComponent(); 
        }
 
        private void live_OnUpdate(object sender, EventArgs e)
        {  
            this.Dispatcher.InvokeAsync(() => { 
                this.rankView.Items.Refresh();
            }); 
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Broadcast != null)
            { 
                this.rankView.ItemsSource = this.Broadcast.Teams;
                
                this.Broadcast.OnUpdate += live_OnUpdate;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.Broadcast != null)
            {
                this.Broadcast.OnUpdate -= live_OnUpdate; 
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11)
            {
                this.oldstate = WindowState;

                WindowState = WindowState.Maximized;
                Visibility = Visibility.Collapsed;
                WindowStyle = WindowStyle.None;
                ResizeMode = ResizeMode.NoResize;
                Visibility = Visibility.Visible;
            }

            if (e.Key == Key.Escape)
            {
                WindowState = oldstate;
                WindowStyle = WindowStyle.SingleBorderWindow;
                ResizeMode = ResizeMode.CanResize;
            }
        }
    }
}
