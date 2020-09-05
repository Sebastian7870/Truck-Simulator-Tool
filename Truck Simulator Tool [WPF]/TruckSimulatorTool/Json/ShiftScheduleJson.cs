using System;

namespace Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Json
{
    public class ShiftScheduleJson
    {
        public int Count { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartPause { get; set; }
        public DateTime EndPause { get; set; }
    }
}
