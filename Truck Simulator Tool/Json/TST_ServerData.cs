using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Truck_Simulator_Tool.Json
{
    public class TST_ServerData
    {
        public string connectionStatusText { get; set; }
        public int[] connectionStatusArgb { get; set; }
        public string contractStatusText { get; set; }
        public int[] contractStatusArgb { get; set; }
        public string shiftStatusText { get; set; }
        public int[] shiftStatusArgb { get; set; }
        public string currentArrival_dtText { get; set; }
        public string currentArrival_tsText { get; set; }
        public int[] currentArrivalArgb { get; set; }
        public string currentBestArrival_dtText { get; set; }
        public string currentBestArrival_tsText { get; set; }
        public string bestArrival_dtText { get; set; }
        public string bestArrival_tsText { get; set; }
        public string nextPauseTimeText { get; set; }
        public int[] nextPauseTimeArgb { get; set; }
        public string remainingTimeText { get; set; }
        public int[] remainingTimeArgb { get; set; }
        public string jobInfo_FreightText { get; set; }
        public string jobInfo_MassText { get; set; }
        public string jobInfo_IncomeText { get; set; }
        public string sourceText { get; set; }
        public string destinationText { get; set; }
        public string progressBarPercentage { get; set; }
        public string timebufferText { get; set; }
        public string remainingDistanceText { get; set; }
        public string timescaleText { get; set; }
        public double pb_distanceProgress { get; set; }
        public string pb_distanceText { get; set; }
        public double pb_damageProgress { get; set; }
        public string pb_damageText { get; set; }
        
        public bool hasShift { get; set; }
        public string nextShiftEvent { get; set; }
        public string nextShiftPause { get; set; }
        public string shiftTimeLeft { get; set; }
    }
}
