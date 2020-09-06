using System.Diagnostics;

namespace Truck_Simulator_Tool__WPF_.TruckSimulatorTool.StaticClasses
{
    public static class GeneralHelpers
    {
        public static bool SDKGameIsRunning
        {
            get
            {
                Process[] processETS = Process.GetProcessesByName("eurotrucks2");
                Process[] processATS = Process.GetProcessesByName("amtrucks");

                if (processETS.Length != 0 || processATS.Length != 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
