using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Truck_Simulator_Tool
{
    class HttpWebServer
    {
        private HttpListener listener;
        private bool _HasEntries;
        public int Port { get; set; }


        public bool IsRunning()
        {
            if (listener != null)
            {
                if (listener.IsListening == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public bool HasEntries()
        {
            if (_HasEntries == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Start and run server
        public void Start()
        {
            try
            {
                listener = new HttpListener();
                listener.Prefixes.Add(String.Format("http://+:{0}/", Port));
                listener.Start();
                _HasEntries = true;
                Run();
            }
            catch
            {
                _HasEntries = false;
            }
        }

        private void Run() => ThreadPool.QueueUserWorkItem((WaitCallback)(o =>
        {
            while (listener.IsListening == true)
            {
                try
                {
                    HttpListenerContext context = listener.GetContext();
                    string msg = "- - - Hello world - - -";
                    byte[] bytes = Encoding.UTF8.GetBytes(msg);
                    context.Response.ContentLength64 = (long)bytes.Length;
                    context.Response.ContentEncoding = Encoding.UTF8;
                    context.Response.ContentType = "application/json;charset=UTF-8";
                    context.Response.OutputStream.Write(bytes, 0, bytes.Length);
                }
                catch
                {
                    listener.Stop();
                }
            }
        }));


        // Stop server
        public void Stop()
        {
            if (listener.IsListening == true)
            {
                listener.Stop();
            }
        }


        // Set PowerShell port entries
        public void SetPowerShellEntries()
        {
            try
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo("netsh", String.Format("http add urlacl url=http://+:{0}/ user=\"{1}\"", Port, (object)new SecurityIdentifier("S-1-1-0").Translate(typeof(NTAccount)).ToString()));
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.Verb = "runas";
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
                _HasEntries = true;
            }
            catch
            {
                MessageBox.Show("Adminstratorrechte sind notwendig. Der Vorgang wurde abgebrochen.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _HasEntries = false;
            }
        }


        // Delete PowerShell port entries
        public void DeletePowerShellEntries()
        {
            try
            {
                this.Stop();
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo("netsh", String.Format("http delete urlacl url=http://+:{0}/", Port));
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.Verb = "runas";
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
                _HasEntries = false;
            }
            catch (Exception)
            {
                MessageBox.Show("Adminstratorrechte sind notwendig. Der Vorgang wurde abgebrochen.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _HasEntries = true;
            }
        }


    }
}
