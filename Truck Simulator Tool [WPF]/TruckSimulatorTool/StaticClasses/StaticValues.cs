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

        public static string SetEntriesArgs
        {
            get { return "-TSTinstall"; }
        }

        public static string DeleteEntriesArgs
        {
            get { return "-TSTuninstall"; }
        }
    }
}
