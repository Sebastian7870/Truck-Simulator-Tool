using System;
using System.Diagnostics;
using System.Linq;

namespace Truck_Simulator_Tool__WPF_.TruckSimulatorTool
{
    class ApplicationStartUp
    {
        [STAThread]
        public static void CanStartUp()
        {
            string[] args = Environment.GetCommandLineArgs() ?? null;
            Process[] process = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);

            if (args.Contains("-TSTinstall") || args.Contains("-TSTuninstall"))
            {//Has correct arguments
                if (args.Contains("-TSTinstall"))
                {
                    Process processSet = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.WindowStyle
                }
                if (args.Contains("-TSTuninstall"))
                {

                }
            }
            else if (process.Length > 1)
            {//Double opened
                Environment.Exit(0);
            }

        }

        private static ProcessStartInfo()
    }
}
