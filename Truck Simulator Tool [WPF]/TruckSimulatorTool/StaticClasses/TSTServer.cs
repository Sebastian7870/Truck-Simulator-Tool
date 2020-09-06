using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;

namespace Truck_Simulator_Tool__WPF_.TruckSimulatorTool.StaticClasses
{
    public static class TSTServer
    {
        private static HttpListener listener;
        private static string message;
        public static string Message
        {
            get
            {
                if (message != null)
                    return message;
                else
                    return "error . . . no data!";
            }
            set
            {
                if (value != null)
                    message = value;
                else
                    message = "error . . . no data!";
            }
        }

        public static bool IsOnline
        {
            get
            {
                if (listener != null && listener.IsListening)
                    return true;
                else
                    return false;
            }
        }

        private static bool hasEntries = false;
        public static bool HasEntries
        {
            get { return hasEntries; }
        }

        public static void TryStart(bool reSetIfnotAvailable)
        {
            try
            {
                listener = new HttpListener();
                listener.Prefixes.Add($"http://+:{StaticValues.Port}/");
                listener.Start();
                hasEntries = true;
                Run();
            }
            catch
            {
                hasEntries = false;
                if (reSetIfnotAvailable)
                {
                    if (MessageBox.Show("Der TST-Server hat fehlende Firewall und Port Einträge und kann ohne diese nicht gestartet werden. Möchten Sie dieses Problem jetzt beheben? Sie können es jederzeit im Servermenü nachholen unter \"TST-Server installieren\".", "Fehlende Einträge des TST-Servers gefunden!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        ReSetPowerShellEntries();
                    }
                }
            }
        }

        private static void Run() => ThreadPool.QueueUserWorkItem(o =>
        {
            while (listener.IsListening)
            {
                try
                {
                    HttpListenerContext context = listener.GetContext();
                    byte[] bytes = Encoding.UTF8.GetBytes(Message);
                    context.Response.ContentLength64 = (long)bytes.Length;
                    context.Response.ContentEncoding = Encoding.UTF8;
                    context.Response.ContentType = "application/json;charset=UTF-8";
                    context.Response.OutputStream.Write(bytes, 0, bytes.Length);
                }
                catch
                {
                    //Todo: LogEntry
                }
            }
        });

        public static void Stop()
        {
            if (listener != null && listener.IsListening)
                listener.Stop();
        }

        // Please use "ReSetPowerShellEntries" to prevent multiple entries.
        /*public static void SetPowerShellEntries()
        {
            try
            {
                netshProcess(StaticValues.SetEntriesArgs);
                hasEntries = true;
                TryStart();
            }
            catch
            {
                //Admin rights not given
                //Todo: LogEntry
            }
        }*/

        public static void DeletePowerShellEntries()
        {
            try
            {
                netshProcess(StaticValues.DeleteEntriesArgs);
                hasEntries = false;
                Stop();
            }
            catch
            {
                //Admin rights not given
                //Todo: LogEntry
            }
        }

        public static void ReSetPowerShellEntries()
        {
            try
            {
                Stop();
                netshProcess(StaticValues.DeleteEntriesArgs, StaticValues.SetEntriesArgs);
                hasEntries = true;
                TryStart(false);
            }
            catch
            {
                //Admin rights not given
                //Todo: LogEntry
            }
        }

        private static void netshProcess(params string[] args)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = StaticValues.ExecutablePath;
            startInfo.Verb = "runas";
            foreach (string Item in args)
            {
                startInfo.Arguments += Item;
                startInfo.Arguments += " ";
            }

            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

    }
}
