using System;

namespace Truck_Simulator_Tool
{
    class Workshift
    {
        private int iCount;
        private DateTime dtWorkshift_start;
        private DateTime dtWorkshift_end;

        public Workshift(int pCount, DateTime pStart, DateTime pEnd)
        {
            iCount = pCount;
            dtWorkshift_start = pStart;
            dtWorkshift_end = pEnd;
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

        public DateTime EndDate
        {
            get => dtWorkshift_end;
            set => dtWorkshift_end = value;
        }

    }
}
