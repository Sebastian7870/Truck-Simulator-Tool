using System;

namespace Truck_Simulator_Tool
{
    class AlldatesTimeSpan
    {
        private int iIndex;
        private TimeSpan ts_timespan;
        private string sType;

        public AlldatesTimeSpan(int pIndex, TimeSpan pTimeSpan, string pType)
        {
            iIndex = pIndex;
            ts_timespan = pTimeSpan;
            sType = pType;
        }

        public int Index
        {
            get => iIndex;
            set => iIndex = value;
        }


        public TimeSpan TimeSpan
        {
            get => ts_timespan;
            set => ts_timespan = value;
        }
        public string Type
        {
            get => sType;
            set => sType = value;
        }


    }
}
