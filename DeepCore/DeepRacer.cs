using System; 

namespace DeepCore
{ 
    public class DeepRacer
    {
        public event EventHandler<EventArgs> OnLoad;
        public event EventHandler<EventArgs> OnStart;
        public event EventHandler<EventArgs> OnStop;
        public event EventHandler<EventRacerArgs> OnReset;
        public event EventHandler<EventArgs> OnTouch;
        public event EventHandler<EventRecordArgs> OnRecord;
        public event EventHandler<EventFinArgs> OnFinish;
        public event EventHandler<EventArgs> OnClose;
        public event EventHandler<EventActivityArgs> OnObserve;
        public event EventHandler<EventReviseArgs> OnChanged;
        public event EventHandler<EventActivityArgs> OnDraw;

        public bool IsRunning { get; set; }
        public Team Team { get; set; }
        public int Lap { get; set; }
        public long BestLap { get; set; }
        public long LastLap { get; set; }
        public bool IsDuty { get; set; }

        public TimeSpan Last_Tick { get; set; }

        public bool TestMode
        {
            get { return this.testMode; }
        }
        private bool testMode { get; set; }

        public DeepRacer()
        {
            testMode = false;
            IsDuty = false;
        }

        public void Ready()
        {
            this.IsDuty = true;

            if (OnLoad != null)
            {
                OnLoad(this, new EventArgs());
            }
        } 
        public void Start()
        {
            this.IsRunning = true;

            if (OnStart != null)
            {
                OnStart(this, new EventArgs());
            }
        }

        public void Stop()
        {
            this.IsRunning = false;

            if (OnStop != null)
            {
                OnStop(this, new EventArgs());
            }
        }

        public void Reset(int duration)
        {  
            this.Last_Tick = TimeSpan.FromSeconds(0);
                
            if (OnReset != null)
            {
                OnReset(this, new EventRacerArgs(duration));
            }   
        }

        public void Tap()
        {
            if (OnTouch != null)
            {
                OnTouch(this, new EventArgs());
            }
        }

        public void Log(DeepLap p)
        {
            if (OnRecord != null)
            {
                OnRecord(this, new EventRecordArgs(p));
            }
        }

        public void Finish(DateTime dt)
        {
            this.IsRunning = false;

            if (OnFinish != null)
            {
                OnFinish(this, new EventFinArgs(dt));
            } 
        }

        public void Activity(string data)
        {
            if (OnObserve != null)
            {
                OnObserve(this, new EventActivityArgs(data));
            } 
        }

        public void Revise(long t)
        {
            this.BestLap = t;

            if (OnChanged != null)
            {
                OnChanged(this, new EventReviseArgs(t));
            }
        }

        public void DrawImage(string file)
        {
            if (OnDraw != null)
            {
                OnDraw(this, new EventActivityArgs(file));
            }
        }

        public void Close()
        {
            if (IsRunning)
            {
                this.IsRunning = false;
            }

            if (OnClose != null)
            {
                OnClose(this, new EventArgs());
            }

            this.IsDuty = false;
        }

        public void SetMode(bool isTest)
        {
            this.testMode = isTest;
        }
    }
}
