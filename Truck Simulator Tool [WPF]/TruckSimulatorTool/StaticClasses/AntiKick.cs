using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
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
            timer.Interval = 5000; //150000
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

        private static double NoFocusCounter = 0;
        private static bool messageBoxShowed = false;
        private static void Timer_Tick(object sender, EventArgs e)
        {
            Process[] processETS = Process.GetProcessesByName("eurotrucks2");
            Process[] processATS = Process.GetProcessesByName("amtrucks");

            if (processETS.Length != 0)
            {
                if (ProcessHasFocus(processETS[0]))
                {
                    SendKeys.SendWait("y/p{Enter}");
                    NoFocusCounter = 0;
                    messageBoxShowed = false;
                }
                else
                {
                    ShowMessageBoxNoFocus();
                }
            }

            if (processATS.Length != 0)
            {
                if (ProcessHasFocus(processETS[0]))
                {
                    SendKeys.SendWait("y/p{Enter}");
                    NoFocusCounter = 0;
                    messageBoxShowed = false;
                }
                else
                {
                    ShowMessageBoxNoFocus();
                }
            }
        }


        private static void ShowMessageBoxNoFocus()
        {
            if (SettingsHelper.SettingsJson.AntiKickMessage)
            {
                if (10 - (NoFocusCounter + 2.5) <= 0)
                {
                    if (!messageBoxShowed)
                    {
                        messageBoxShowed = true;
                        MessageBox.Show("Weil Sie zu lange nicht im Spiel waren, wurden Sie wahrscheinlich vom Server gekickt.", "Sie wurden wahrscheinlich gekickt!", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    NoFocusCounter += 2.5;
                    MessageBox.Show("ETS2 befindet sich derzeitig nicht im Vordergrund. AntiKick kann das Kicken vom Server dadurch nicht aufhalten.", $"ETS2 nicht im Vordergrund! (Noch ~{10 - NoFocusCounter} Min.)", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}
