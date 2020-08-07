using System;

namespace Truck_Simulator_Tool
{
    class Workshift
    {
        private int iCount;
        private DateTime dtWorkshift_start;
        private DateTime dtWorkshift_end;
        private DateTime dtWorkshift_startpause;
        private DateTime dtWorkshift_endpause;

        public Workshift(int pCount, DateTime pStart, DateTime pEnd, DateTime pPauseStart, DateTime pPauseEnd)
        {
            iCount = pCount;
            dtWorkshift_start = pStart;
            dtWorkshift_end = pEnd;
            dtWorkshift_startpause = pPauseStart; // Shift pause!!! (between pStart and pEnd)
            dtWorkshift_endpause = pPauseEnd; // Shift pause!!! (between pStart and pEnd)
        }

        public int Count
        {
            get => iCount;
            set => iCount = value;
        }

        public DateTime StartDate
        {
            get => dtWorkshift_start;
            set => dtWorkshift_start = value;
        }

        public DateTime StartPause
        {
            get => dtWorkshift_startpause;
            set => dtWorkshift_startpause = value;
        }

        public DateTime EndPause
        {
            get => dtWorkshift_endpause;
            set => dtWorkshift_endpause = value;
        }

        public DateTime EndDate
        {
            get => dtWorkshift_end;
            set => dtWorkshift_end = value;
        }

    }
}
