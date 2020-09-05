using System;

namespace Truck_Simulator_Tool__WPF_.TruckSimulatorTool.StaticClasses
{
    public static class StaticValues
    {
        public static string SoftwarePath
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }

        public static int Port
        {
            get { return 25558; }
        }
    }
}
