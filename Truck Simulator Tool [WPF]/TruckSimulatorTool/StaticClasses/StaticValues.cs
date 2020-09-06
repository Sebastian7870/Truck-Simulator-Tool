using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Truck_Simulator_Tool__WPF_.TruckSimulatorTool.StaticClasses
{
    public static class StaticValues
    {
        public static string SoftwarePath
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }

        public static string ExecutablePath
        {
            get { return Process.GetCurrentProcess().MainModule.FileName; }
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

        public static string FullIPAddress
        {
            get
            {
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                string ipAddress = "none";
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                        ipAddress = ip.ToString();
                }
                return ipAddress;
            }
        }
    }
}
