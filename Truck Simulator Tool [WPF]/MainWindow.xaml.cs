using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using SCSSdkClient;
using SCSSdkClient.Object;

namespace Truck_Simulator_Tool__WPF_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SCSSdkTelemetry Telemetry;
        public bool SdkOnline = false;
        private double drivenDistance;


        public MainWindow()
        {
            InitializeComponent();
            Telemetry = new SCSSdkTelemetry();

            Telemetry.Data += Telemetry_Data;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            label_dateTimeNowSeconds.Content = DateTime.Now.Second.ToString();
            label_dateTimeNowTime.Content = DateTime.Now.ToString("HH:mm");
            label_dateTimeNowDate.Content = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek)}\n{DateTime.Now.Date.ToShortDateString()}";
            //Todo: UpdateTFM():    
            //      TST Server implementation
            //      DateTimePicker set min. date
        }


        private void Telemetry_Data(SCSTelemetry data, bool updated)
        {
            if (!updated) return;
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(new TelemetryData(Telemetry_Data), data, updated);
                    return;
                }

                if (!data.SdkActive)
                {
                    label_connectionStatus.Content = "Keine Verbindung zum Spiel";
                    label_connectionStatus.Background = new SolidColorBrush(Colors.Brown);
                }
                else
                {
                    if (data.Paused)
                    {
                        label_connectionStatus.Content = "Spiel pausiert";
                        label_connectionStatus.Background = new SolidColorBrush(Colors.Goldenrod);
                    }
                    else
                    {
                        label_connectionStatus.Content = "Verbunden";
                        label_connectionStatus.Background = new SolidColorBrush(Colors.LimeGreen);


                        // LINEAR IS THE CORRECT VELOCITY-VALUE!
                        label_nextRestStop.Content = $"LINEAR: {GetKmHFromFVector(data.TruckValues.CurrentValues.AccelerationValues.LinearVelocity)}";
                    }
                }
            }
            catch
            {

            }
        }

        private double GetKmHFromFVector(SCSTelemetry.FVector fVector)
        {
            return (Math.Sqrt((Math.Pow(fVector.X, 2) + Math.Pow(fVector.Y, 2) + Math.Pow(fVector.Z, 2))) / 1000) * 3600;
        }
    }
}
