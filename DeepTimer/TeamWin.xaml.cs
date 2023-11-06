using System;
using System.Collections.Generic; 
using System.Windows; 

namespace DeepTimer
{
    /// <summary>
    /// Interaction logic for Team.xaml
    /// </summary>
    public partial class TeamWin : Window
    {
        public string TeamName
        {
            get
            {
                return this.tbName.Text;
            }

            set
            {
                this.tbName.Text = value;
            }
        }

        public TeamWin()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
