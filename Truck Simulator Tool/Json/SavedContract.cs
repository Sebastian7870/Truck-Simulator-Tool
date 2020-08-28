namespace Truck_Simulator_Tool
{
    class SavedContract
    {
        public string GameId { get; set; }
        public string SourceCity { get; set; }
        public string SourceCompany { get; set; }
        public string DestinationCity { get; set; }
        public string DestinationCompany { get; set; }
        public int Income { get; set; }
        public float TotalMass { get; set; }
        public string LastProfile { get; set; }
        public double SpeedSummary { get; set; }
        public int TimerCounter { get; set; }
        public double DrivenDistance { get; set; }
    }
}
