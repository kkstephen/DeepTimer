using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepCore
{
    public class EventRacerArgs : EventArgs
    {
        public int Duration { get; set; }    
        public EventRacerArgs(int duration)
        {
            Duration = duration;          
        }
    }

    public class EventRecordArgs : EventArgs
    {
        public DeepLap Lap { get; set; }

        public EventRecordArgs(DeepLap p)
        {
            Lap = p;
        }
    }

    public class EventFinArgs : EventArgs
    {
        public DateTime Date { get; set; }

        public EventFinArgs(DateTime date)
        {
            Date = date;
        }
    }

    public class EventActivityArgs : EventArgs
    {
        public string Data { get; set; }

        public EventActivityArgs(string str)
        {
            Data = str;
        }
    }

    public class EventReviseArgs : EventArgs
    {
        public long Tick { get; set; }

        public EventReviseArgs(long t)
        {
            Tick = t;
        }
    }
}
