using SCSSdkClient;

namespace Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Json
{
    public class ContractJson
    {
        public string Game { get; set; }
        public string CitySource { get; set; }
        public string CityDestination { get; set; }
        public ulong Income { get; set; }
        public float Mass { get; set; }
        public int OdometerStartValue { get; set; }
    }
}
