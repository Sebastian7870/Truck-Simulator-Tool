namespace Truck_Simulator_Tool
{
    class Contract
    {
        public string SourceCity { get; set; }
        public string SourceCompany { get; set; }
        public string DestinationCity { get; set; }
        public string DestinationCompany { get; set; }

        public double SpeedSummary { get; set; }
        public int TimerCounter { get; set; }
        public double DrivenDistance { get; set; }
    }
}
