using System.Windows.Media;

namespace Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Json
{
    public class TSTServerJson
    {
        public string connectionStatusText { get; set; }
        public Brush connectionStatusBrush { get; set; }
        public string contractStatusText { get; set; }
        public Brush contractStatusBrush { get; set; }
        public string shiftStatusText { get; set; }
        public Brush shiftStatusBrush { get; set; }
        public string currentArrival_dtText { get; set; }
        public string currentArrival_tsText { get; set; }
        public Brush currentArrivalBrush { get; set; }
        public string currentBestArrival_dtText { get; set; }
        public string currentBestArrival_tsText { get; set; }
        public string bestArrival_dtText { get; set; }
        public string bestArrival_tsText { get; set; }
        public string nextPauseTimeText { get; set; }
        public Brush nextPauseTimeBrush { get; set; }
        public string remainingTimeText { get; set; }
        public Brush remainingTimeBrush { get; set; }
        public string jobInfo_FreightText { get; set; }
        public string jobInfo_MassText { get; set; }
        public string jobInfo_IncomeText { get; set; }
        public string sourceText { get; set; }
        public string destinationText { get; set; }
        public string progressBarPercentage { get; set; }
        public string timebufferText { get; set; }
        public Brush timebufferBrush { get; set; }
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
