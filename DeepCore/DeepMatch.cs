using System;

namespace DeepCore
{
    public class DeepMatch
    {
        public int No { get; set; }
        public DeepLap Lap { get; set; }

        public string Result
        {
            get
            {
                if (Lap.Invalid)
                {
                    return "DNF";
                }

                return Lap.Record.ToTimespan();
            }
        }

        public int Flag
        {
            get
            {
                if (Lap.Record == 0 || Lap.Invalid)
                {
                    return 0;
                }

                switch (No)
                {
                    case 1:
                        return 1;
                    case 2:
                        return 2;
                    case 3:
                        return 3;

                    default:
                        return 0;
                }
            }
        }
    }
}
