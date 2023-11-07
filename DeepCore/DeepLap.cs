using System;
using System.ComponentModel;
using UnitODB;

namespace DeepCore
{
    public class DeepLap : OdbEntity, INotifyPropertyChanged
    {
        public Team Team { get; set; }

        //public string Team { get; set; }

        public int Lap { get; set; }

        private long record;
        public long Record
        {
            get
            {
                return record;
            }

            set
            {
                record = value;
                OnPropertyChanged("Record");
            }
        } 

        public DateTime Date { get; set; }

        private bool invalid;
        public bool Invalid 
        { 
            get
            {
                return invalid;                    
            }

            set
            {
                this.invalid = value;
                OnPropertyChanged("Status");
            }
        }

        [Column(NotMapped = true)]
        public string Status
        {
            get
            {
                return Invalid ? "DNF" : ""; 
            }
        } 

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    } 
}
