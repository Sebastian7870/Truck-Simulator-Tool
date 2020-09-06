using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace Truck_Simulator_Tool__WPF_.TruckSimulatorTool.StaticClasses
{
    public static class AntiKick
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);


        static Timer timer = new Timer();

        public static void Start()
        {
            timer.Interval = 150000;
            timer.Start();
            timer.Tick += Timer_Tick;
        }

        public static void Stop()
        {
            timer.Stop();
        }


        private static bool ProcessHasFocus(Process process)
        {
            var handle = GetForegroundWindow();
            if (handle == IntPtr.Zero)
                return false;

            var processId = process.Id;
            int activeProcessId;
            GetWindowThreadProcessId(handle, out activeProcessId);

            return activeProcessId == processId;
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            Process[] processETS = Process.GetProcessesByName("eurotrucks2");
            Process[] processATS = Process.GetProcessesByName("amtrucks");

            if (processETS.Length != 0)
            {
                if (ProcessHasFocus(processETS[0]))
                    SendKeys.Send("y/p{Enter}");
            }

            if (processATS.Length != 0)
            {
                if (ProcessHasFocus(processATS[0]))
                    SendKeys.Send("y/p{Enter}");
            }
        }
    }
}
