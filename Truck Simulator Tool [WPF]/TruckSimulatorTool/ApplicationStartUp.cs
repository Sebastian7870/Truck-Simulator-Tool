using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using Truck_Simulator_Tool__WPF_.TruckSimulatorTool.StaticClasses;

namespace Truck_Simulator_Tool__WPF_.TruckSimulatorTool
{
    class ApplicationStartUp
    {
        [STAThread]
        public static void CanStartUp()
        {
            string[] args = Environment.GetCommandLineArgs() ?? null;
            Process[] process = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);

            if (args.Contains(StaticValues.SetEntriesArgs) || args.Contains(StaticValues.DeleteEntriesArgs))
            {//Has correct arguments
                if (args.Contains(StaticValues.DeleteEntriesArgs))
                {// if both args given: this (DeleteEntries) will firtsly be executed then SetEntries.
                    netshProcess($"http delete urlacl url=http://+:{StaticValues.Port}/");
                    netshProcess($"advfirewall firewall delete rule name=all dir=in protocol=TCP localport={StaticValues.Port}");
                }
                if (args.Contains(StaticValues.SetEntriesArgs))
                {
                    netshProcess($"http add urlacl url = http://+:{StaticValues.Port}/ user=\"{(object)new SecurityIdentifier("S-1-1-0").Translate(typeof(NTAccount)).ToString()}\"");
                    netshProcess($"advfirewall firewall add rule name=\"TruckSimulatorTool Server\" dir=in action=allow protocol=TCP localport={StaticValues.Port}");
                }
                Environment.Exit(0);
            }
            else if (process.Length > 1)
            {//Application opened twice
                Environment.Exit(0);
            }

        }

        private static void netshProcess(string args)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "netsh";
            startInfo.Verb = "runas";
            startInfo.Arguments = args;

            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }
    }
}
