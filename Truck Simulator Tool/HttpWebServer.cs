using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Truck_Simulator_Tool
{
    class HttpWebServer
    {
        private HttpListener listener;
        private bool _HasEntries;
        private string _message;
        private string message
        {
            get
            {
                if (this._message != null)
                {
                    return _message;
                }
                else
                {
                    return "error... no data!";
                }
            }
            set
            {
                if (this._message != null)
                {
                    this.message = _message;
                }
                else
                {
                    this.message =  "error... no data!";
                }
            }
        }

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
        public void SetMessage(string json)
        {
            _message = json;
        }


        // Start and run server
        public void Start()
        {
            try
            {
                listener = new HttpListener();
                listener.Prefixes.Add(String.Format("http://+:{0}/", Port.iPort));
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
                    byte[] bytes = Encoding.UTF8.GetBytes(message);
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
                Process SetPortEntries = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = Application.ExecutablePath;
                startInfo.Verb = "runas";
                startInfo.Arguments = "-install";//String.Format("netsh http add urlacl url=http://+:{0}/ user=\"{1}\"; netsh advfirewall firewall add rule name=\"TruckSimulatorTool Server\" dir=in action=allow protocol=TCP localport={0}", Port.iPort, (object)new SecurityIdentifier("S-1-1-0").Translate(typeof(NTAccount)).ToString());

                SetPortEntries.StartInfo = startInfo;
                SetPortEntries.Start();
                SetPortEntries.WaitForExit();

                this.Start();
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
                Process SetPortEntries = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = Application.ExecutablePath;
                startInfo.Verb = "runas";
                startInfo.Arguments = "-uninstall";//String.Format("netsh http add urlacl url=http://+:{0}/ user=\"{1}\"; netsh advfirewall firewall add rule name=\"TruckSimulatorTool Server\" dir=in action=allow protocol=TCP localport={0}", Port.iPort, (object)new SecurityIdentifier("S-1-1-0").Translate(typeof(NTAccount)).ToString());

                SetPortEntries.StartInfo = startInfo;
                SetPortEntries.Start();
                SetPortEntries.WaitForExit();

                this.Stop();
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
