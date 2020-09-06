using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Truck_Simulator_Tool__WPF_.TruckSimulatorTool.StaticClasses
{
    public static class TSTServer
    {
        private static HttpListener listener = new HttpListener();
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

        public static void TryStart()
        {
            try
            {
                listener.Prefixes.Add($"http://+:{StaticValues.Port}/");
                listener.Start();
                hasEntries = true;
                Run();
            }
            catch
            {
                hasEntries = false;
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


        public static void SetPowerShellEntries()
        {
            try
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = Application.ExecutablePath;
                startInfo.Verb = "runas";
                startInfo.Arguments = "-TSTinstall";

                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();

                hasEntries = true;
                TryStart();
            }
            catch
            {
                //Admin rights not given
                //Todo: LogEntry
            }
        }

        public static void DeletePowerShellEntries()
        {
            try
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = Application.ExecutablePath;
                startInfo.Verb = "runas";
                startInfo.Arguments = "-TSTuninstall";

                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();

                hasEntries = false;
                Stop();
            }
            catch
            {
                //Admin rights not given
                //Todo: LogEntry
            }
        }

    }
}
