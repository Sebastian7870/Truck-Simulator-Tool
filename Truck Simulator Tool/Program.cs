using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Windows.Forms;

namespace Truck_Simulator_Tool
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 0)
            {
                string[] strArray = new string[args.Length];

                int i = 0;
                foreach (string Argument in args)
                {
                    strArray[i] = Argument;
                    i++;
                }

                if (strArray.Contains<string>("-install") || strArray.Contains<string>("-uninstall"))
                {
                    if (strArray.Contains<string>("-install"))
                    {
                        Process SetPortEntry = new Process();
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo.FileName = "netsh";
                        startInfo.Verb = "runas";
                        startInfo.Arguments = String.Format("http add urlacl url=http://+:{0}/ user=\"{1}\"", Port.iPort, (object)new SecurityIdentifier("S-1-1-0").Translate(typeof(NTAccount)).ToString());
                        SetPortEntry.StartInfo = startInfo;
                        SetPortEntry.Start();
                        SetPortEntry.WaitForExit();

                        Process SetFirewallEntry = new Process();
                        ProcessStartInfo startInfo1 = new ProcessStartInfo();
                        startInfo1.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo1.FileName = "netsh";
                        startInfo1.Verb = "runas";
                        startInfo1.Arguments = String.Format("advfirewall firewall add rule name=\"TruckSimulatorTool Server\" dir=in action=allow protocol=TCP localport={0}", Port.iPort);
                        SetFirewallEntry.StartInfo = startInfo1;
                        SetFirewallEntry.Start();
                        SetFirewallEntry.WaitForExit();
                    }
                    if (strArray.Contains<string>("-uninstall"))
                    {
                        Process DeletePortEntry = new Process();
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo.FileName = "netsh";
                        startInfo.Verb = "runas";
                        startInfo.Arguments = String.Format("http delete urlacl url=http://+:{0}/", Port.iPort);
                        DeletePortEntry.StartInfo = startInfo;
                        DeletePortEntry.Start();
                        DeletePortEntry.WaitForExit();

                        Process DeleteFirewallEntry = new Process();
                        ProcessStartInfo startInfo1 = new ProcessStartInfo();
                        startInfo1.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo1.FileName = "netsh";
                        startInfo1.Verb = "runas";
                        startInfo1.Arguments = String.Format("advfirewall firewall delete rule name=all dir=in protocol=TCP localport={0}", Port.iPort);
                        DeleteFirewallEntry.StartInfo = startInfo1;
                        DeleteFirewallEntry.Start();
                        DeleteFirewallEntry.WaitForExit();
                    }
                    Application.Exit();
                }
                else
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MainForm());
                }
            }

        }
    }
}
