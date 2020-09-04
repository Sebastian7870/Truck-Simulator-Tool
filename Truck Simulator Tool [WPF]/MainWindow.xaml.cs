using Newtonsoft.Json;
using SCSSdkClient;
using SCSSdkClient.Object;
using System;
using System.Globalization;
using System.IO;
using System.Net.Cache;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Classes;
using Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Json;
using Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Methods;

namespace Truck_Simulator_Tool__WPF_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SpeedCalculations speedCalcs = new SpeedCalculations();
        public ShiftSchedule shiftSchedule = new ShiftSchedule();
        public SCSSdkTelemetry Telemetry;
        Rootobject_TFMdj tfmDJ_data = new Rootobject_TFMdj();
        Rootobject_TFMsong tfmSong_data = new Rootobject_TFMsong();
        bool tfmIsOnline = false;

        bool hasBestArrival = false;
        int timeScaleConstant = 19; // Todo: change to settings
        TimeSpan ts_bestArrival;
        DateTime dt_bestArrival = DateTime.Now;

        double navigationDistance;
        double plannedDistanceKM;
        double truckSpeed;
        double jobInfoMass;
        double fuelCurrent;
        double fuelCapacity;
        double fuelRange;
        string ingameTime;

        public MainWindow()
        {
            InitializeComponent();
            Telemetry = new SCSSdkTelemetry();

            Telemetry.Data += Telemetry_Data;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += Timer_Tick;
            timer.Start();
            dateTimePicker_shiftStart.Minimum = DateTime.Now;
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            label_dateTimeNowSeconds.Content = DateTime.Now.Second.ToString();
            label_dateTimeNowTime.Content = DateTime.Now.ToString("HH:mm");
            label_dateTimeNowDate.Content = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek)}\n{DateTime.Now.Date.ToShortDateString()}";
            await UpdateTFM();
            if (tfmIsOnline)
            {
                await SetTFMSongPicture();
                label_tfmSongTitle.Content = tfmSong_data.title;
                label_tfmSongAuthor.Content = tfmSong_data.artist;
                label_tfmDJName.Content = $"DJ {tfmDJ_data.result.dj.name}";
                TimeSpan ts = TimeSpan.FromSeconds(Convert.ToDouble(tfmDJ_data.result.slot.timeend) - Convert.ToDouble(tfmDJ_data.result.slot.timestart));
                label_tfmDuration.Content = $"{ts.TotalHours} Std.";
            }
            else
            {

            }
            //Todo: TST Server implementation
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

                double IntervalFactor = (double)Telemetry.UpdateInterval / 1000;
                if (!data.SdkActive)
                {
                    label_connectionStatus.Content = "Keine Verbindung zum Spiel";
                    label_connectionStatus.Background = new SolidColorBrush(Colors.Brown);
                }
                else
                {
                    switch (data.Game)
                    {
                        case SCSGame.Ets2:
                            {
                                SetETSTextUnits();
                                SetETSValueUnits(data);
                                break;
                            }
                        case SCSGame.Ats:
                            {
                                SetATSTextUntis();
                                SetATSValueUnits(data);
                                break;
                            }
                        default:
                            {
                                SetETSTextUnits();
                                SetETSValueUnits(data);
                                break;
                            }
                    }
                    label_ingameTime.Content = ingameTime;
                    TimeSpan ts_nextPauseTime = TimeSpan.FromSeconds(data.CommonValues.NextRestStop.Value * 60);

                    if (data.Paused)
                    {
                        label_connectionStatus.Content = "Spiel pausiert";
                        label_connectionStatus.Background = new SolidColorBrush(Colors.Goldenrod);
                    }
                    else
                    {// notPaused-only
                        label_connectionStatus.Content = "Verbunden";
                        label_connectionStatus.Background = new SolidColorBrush(Colors.LimeGreen);


                        if (data.TruckValues.CurrentValues.DashboardValues.Speed.Kph > 5)
                        {
                            speedCalcs.timerCounter += 1;
                            speedCalcs.SetSpeedSummary(truckSpeed, IntervalFactor);
                            label_averageSpeed.Content = Math.Round(speedCalcs.GetCurrentAverageSpeed(), 2);
                            if (data.Game == SCSGame.Ets2) { label_averageSpeed.Content += " km/h"; }
                            else { label_averageSpeed.Content += " mph"; }
                        }
                    }

                    if (data.NavigationValues.NavigationDistance != 0 && data.NavigationValues.NavigationTime != 0)
                    {// destination-only
                        speedCalcs.SetCurrentBestAverageSpeed(navigationDistance, data.NavigationValues.NavigationTime);

                        DateTime dt_currentBestArrival = DateTime.Now.AddSeconds(navigationDistance / speedCalcs.GetCurrentBestAverageSpeed() / timeScaleConstant * 3600);
                        TimeSpan ts_currentBestArrival = dt_currentBestArrival.Subtract(DateTime.Now);
                        label_dt_currentBestArrival.Content = $"{dt_currentBestArrival.ToString("HH:mm")} Uhr";
                        label_ts_currentBestArrival.Content = $"({ConverterMethods.ConvertTimespanToCustomString(ts_currentBestArrival)})";

                        if (!hasBestArrival)
                        {
                            ts_bestArrival = TimeSpan.FromSeconds((int)navigationDistance / speedCalcs.GetCurrentBestAverageSpeed() / timeScaleConstant * 3600);
                            dt_bestArrival = DateTime.Now.Add(ts_bestArrival);
                            label_dt_bestArrival.Content = $"{DateTime.Now.ToString("HH:mm")} Uhr - {dt_bestArrival.ToString("HH:mm")} Uhr";
                            hasBestArrival = true;
                        }
                        ts_bestArrival = dt_bestArrival - DateTime.Now;

                        if (ts_bestArrival.TotalSeconds > 0)
                        {
                            label_ts_bestArrival.Content = $"(-{ConverterMethods.ConvertTimespanToCustomString(ts_bestArrival)})";
                        }
                        else
                        {
                            label_ts_bestArrival.Content = $"(+{ConverterMethods.ConvertTimespanToCustomString(TimeSpan.FromSeconds(ts_bestArrival.TotalSeconds * (-1)))})";
                        }

                        DateTime dt_currentArrival;
                        TimeSpan ts_currentArrival;
                        if (speedCalcs.GetCurrentAverageSpeed() != 0)
                        {
                            dt_currentArrival = DateTime.Now.AddSeconds(navigationDistance / speedCalcs.GetCurrentAverageSpeed() / timeScaleConstant * 3600);
                            ts_currentArrival = dt_currentArrival.Subtract(DateTime.Now);

                            System.Windows.Controls.Label[] labels = { label_currentArrivalText, label_dt_currentArrival, label_ts_currentArrival };
                            if (ts_currentArrival.TotalMinutes - ts_currentBestArrival.TotalMinutes >= 60)
                            {
                                SetArrivalLabelColor(labels, Colors.Brown);
                            }
                            else if (ts_currentArrival.TotalMinutes - ts_currentBestArrival.TotalMinutes > 30 && ts_currentArrival.TotalMinutes - ts_currentBestArrival.TotalMinutes < 60)
                            {
                                SetArrivalLabelColor(labels, Colors.Goldenrod);
                            }
                            else
                            {
                                SetArrivalLabelColor(labels, Colors.LimeGreen);
                            }

                            label_dt_currentArrival.Content = $"{dt_currentArrival.ToString("HH:mm")} Uhr";
                            label_ts_currentArrival.Content = $"({ConverterMethods.ConvertTimespanToCustomString(ts_currentArrival)})";
                        }

                        if (!data.Paused)
                        {
                            if (data.TruckValues.CurrentValues.DashboardValues.Speed.Kph > 0.01)
                            {
                                //speedCalcs.SetDrivenDistance(, data.CommonValues.Scale, IntervalFactor); Look OneNote
                                speedCalcs.SetDistanceSummary(navigationDistance);

                                progressBar_distance.Value = Math.Round(100 * (speedCalcs.GetDrivenDistance() / speedCalcs.GetDistanceSummary()), 2);
                                label_progressBar_distanceText.Content = $"{Math.Round(speedCalcs.GetDrivenDistance(), 0)} {UDistance} / {Math.Round(speedCalcs.GetDistanceSummary(), 0)} {UDistance}";
                                label_drivenDistanceProgress.Content = (progressBar_distance.Value / 100).ToString("p2");
                                label_remainingDistance.Content = $"Noch {Math.Round(navigationDistance, 0)} {UDistance}";
                            }
                        }
                    }

                    if (data.SpecialEventsValues.OnJob)
                    {// contract-only
                        label_remainingDeliveryTime.Content = $"Restzeit: {TimeSpan.FromSeconds(data.JobValues.RemainingDeliveryTime.Value * 60)}";
                        label_jobInfoFreight.Content = data.JobValues.CargoValues.Name;
                        label_jobInfoMass.Content = $"{data.JobValues.CargoValues.Mass} {UMass}";
                        label_jobInfoIncome.Content = $"{data.JobValues.Income.ToString("c0", UCultureInfo)} ({Math.Round((double)data.JobValues.Income / plannedDistanceKM),2} {UMoneyDistance})";

                        TimeSpan ts_remainingTime = TimeSpan.FromSeconds(data.JobValues.RemainingDeliveryTime.Value * 60);
                        TimeSpan ts_estimatedTime = TimeSpan.FromSeconds(data.NavigationValues.NavigationTime);
                        TimeSpan ts_timebuffer = ts_remainingTime - ts_estimatedTime;
                        SetTimebufferLabel(data.Game, ts_nextPauseTime, ts_estimatedTime, ts_remainingTime, ts_timebuffer);
                        SetRemainingtimeLabel(data.Game, ts_remainingTime);
                    }

                    if (data.TruckValues.CurrentValues.LightsValues.Beacon)
                        label_beaconState.Content = "eingeschaltet";
                    else
                        label_beaconState.Content = "ausgeschaltet";
                    label_averageFuelConsumption.Content = GetAverageFuelConsumptionText(data);
                    progressBar_fuel.Value = fuelCurrent / fuelCapacity * 100;
                    if (data.TruckValues.CurrentValues.DashboardValues.WarningValues.FuelW)
                        progressBar_fuel.Foreground = new SolidColorBrush(Colors.Brown);
                    else
                        progressBar_fuel.Foreground = new SolidColorBrush(Colors.LimeGreen);
                    label_progressBar_fuelText.Content = $"{Math.Round(fuelCurrent, 0)} {UFluid}  /  {Math.Round(fuelCapacity, 0)} {UFluid}  ({Math.Round(fuelRange, 0)} {UDistance})";

                    label_nextRestStop.Content = $"Pause in: {ConverterMethods.ConvertTimespanToCustomString(ts_nextPauseTime)}";
                    if (ts_nextPauseTime.TotalSeconds > 0)
                    {
                        if (ts_nextPauseTime.TotalHours < 3)
                            label_nextRestStop.Foreground = new SolidColorBrush(Colors.Goldenrod);
                        else
                            label_nextRestStop.Foreground = new SolidColorBrush(Colors.LimeGreen);
                    }
                    else
                    {
                        label_nextRestStop.Foreground = new SolidColorBrush(Colors.Brown);
                    }
                    progressBar_damage.Value = data.JobValues.CargoValues.CargoDamage;
                    label_progressBar_damageText.Content = data.JobValues.CargoValues.CargoDamage.ToString("p0");
                }
            }
            catch
            {
            }
        }

        private static void SetArrivalLabelColor(System.Windows.Controls.Label[] labels, Color color)
        {
            foreach (System.Windows.Controls.Label label in labels)
                label.Background = new SolidColorBrush(color);
        }
        private static string GetAverageFuelConsumptionText(SCSTelemetry data)
        {
            if (data.Game == SCSGame.Ets2)
                return $"{(data.TruckValues.CurrentValues.DashboardValues.FuelValue.AverageConsumption * 100).ToString("n2")} {UAverageFuelConsumption}";
            else
                return $"{ConverterMethods.ConvertEUAverageFueltoAMAverageFuel((data.TruckValues.CurrentValues.DashboardValues.FuelValue.AverageConsumption * 100)).ToString("n2")} {UAverageFuelConsumption}";
        }

        private static string UDistance = "km";
        private static string UCurrency = "€";
        private static string UMass = "t";
        private static string UFluid = "l";
        private static string UMoneyDistance = "€/km";
        private static string UAverageFuelConsumption = "l/100km";
        private static CultureInfo UCultureInfo = new CultureInfo("de-DE");

        private static void SetETSTextUnits()
        {
            UDistance = "km";
            UCurrency = "€";
            UMass = "t";
            UFluid = "l";
            UMoneyDistance = "€/km";
            UAverageFuelConsumption = "l/100km";
            UCultureInfo = new CultureInfo("de-DE");
        }
        private void SetETSValueUnits(SCSTelemetry data)
        {
            navigationDistance = data.NavigationValues.NavigationDistance / 1000;
            plannedDistanceKM = data.JobValues.PlannedDistanceKm;
            truckSpeed = data.TruckValues.CurrentValues.DashboardValues.Speed.Kph;
            jobInfoMass = data.JobValues.CargoValues.Mass;
            fuelCurrent = data.TruckValues.CurrentValues.DashboardValues.FuelValue.Amount;
            fuelCapacity = data.TruckValues.ConstantsValues.CapacityValues.Fuel;
            fuelRange = data.TruckValues.CurrentValues.DashboardValues.FuelValue.Range;
            ingameTime = $"{data.CommonValues.GameTime.Date.ToString("ddd H:mm", UCultureInfo)}";
        }

        private static void SetATSTextUntis()
        {
            UDistance = "mi";
            UCurrency = "$";
            UMass = "lb";
            UFluid = "gal";
            UMoneyDistance = "$/mi";
            UAverageFuelConsumption = "mpg";
            UCultureInfo = new CultureInfo("en-US");
        }
        private void SetATSValueUnits(SCSTelemetry data)
        {
            navigationDistance = ConverterMethods.ConvertKMtoMI(data.NavigationValues.NavigationDistance / 1000);
            plannedDistanceKM = ConverterMethods.ConvertKMtoMI(data.JobValues.PlannedDistanceKm);
            truckSpeed = data.TruckValues.CurrentValues.DashboardValues.Speed.Mph;
            jobInfoMass = ConverterMethods.ConvertTtoLB(data.JobValues.CargoValues.Mass);
            fuelCurrent = ConverterMethods.ConvertLtoGAL(data.TruckValues.CurrentValues.DashboardValues.FuelValue.Amount);
            fuelCapacity = ConverterMethods.ConvertLtoGAL(data.TruckValues.ConstantsValues.CapacityValues.Fuel);
            fuelRange = ConverterMethods.ConvertLtoGAL(data.TruckValues.CurrentValues.DashboardValues.FuelValue.Range);
            ingameTime = $"{data.CommonValues.GameTime.Date.ToString("ddd h:mm", UCultureInfo)}";
        }

        private void SetTimebufferLabel(SCSGame scsGame, TimeSpan nextPauseTime, TimeSpan estimatedTime, TimeSpan remainingTime, TimeSpan timebuffer)
        {
            if (nextPauseTime < estimatedTime)
            {
                if (scsGame == SCSGame.Ets2)
                {
                    double d = Math.Ceiling((estimatedTime.TotalSeconds / nextPauseTime.TotalSeconds) / (11 * 3600));
                    timebuffer = remainingTime - (estimatedTime.Add(TimeSpan.FromHours(d * 9)));
                }
                else
                {
                    double d = Math.Ceiling((estimatedTime.TotalSeconds / nextPauseTime.TotalSeconds) / (14 * 3600));
                    timebuffer = remainingTime - (estimatedTime.Add(TimeSpan.FromHours(d * 10)));
                }
            }
            if (timebuffer.TotalSeconds < 0)
            {
                if (remainingTime.TotalHours < 5000)
                {
                    label_timebuffer.Background = new SolidColorBrush(Colors.CornflowerBlue);
                    label_timebuffer.Content = "Zeitpuffer: WoT";
                }
                else
                {
                    label_timebuffer.Background = new SolidColorBrush(Colors.Brown);
                    label_timebuffer.Content = "Zeitpuffer: 0 Min.";
                }
            }
            else
            {
                if (timebuffer.TotalHours <= 5)
                    label_timebuffer.Background = new SolidColorBrush(Colors.Goldenrod);
                else
                    label_timebuffer.Background = new SolidColorBrush(Colors.LimeGreen);

                label_timebuffer.Content = $"Zeitpuffer: {ConverterMethods.ConvertTimespanToCustomString(timebuffer)}";
            }
        }
        
        private void SetRemainingtimeLabel(SCSGame scsGame, TimeSpan remainingTime)
        {
            if (remainingTime.TotalSeconds < 0)
            {
                if (remainingTime.TotalHours < 5000)
                {
                    label_remainingDeliveryTime.Foreground = new SolidColorBrush(Colors.CornflowerBlue);
                    label_remainingDeliveryTime.Content = "Restzeit: WoT";
                }
                else
                {
                    label_remainingDeliveryTime.Foreground = new SolidColorBrush(Colors.Brown);
                    label_remainingDeliveryTime.Content = "Restzeit: 0 Min.";
                }
            }
            else
            {
                if (remainingTime.TotalHours < 3)
                {
                    label_remainingDeliveryTime.Foreground = new SolidColorBrush(Colors.Goldenrod);
                }
                else
                {
                    label_remainingDeliveryTime.Foreground = new SolidColorBrush(Colors.LimeGreen);
                }
                label_remainingDeliveryTime.Content = $"Restzeit: {ConverterMethods.ConvertTimespanToCustomString(remainingTime)}";
            }
        }

        private async Task UpdateTFM()
        {
            try
            {
                HttpClient client = new HttpClient();
                Stream stream = await client.GetStreamAsync("https://panel.truckers.fm/api/current");
                Stream stream1 = await client.GetStreamAsync("https://panel.truckers.fm/api/song/current");

                StreamReader sr = new StreamReader(stream);
                string jsonDJ = sr.ReadToEnd();
                sr.Close();
                StreamReader sr1 = new StreamReader(stream1);
                string jsonSong = sr1.ReadToEnd();
                sr1.Close();

                tfmDJ_data = JsonConvert.DeserializeObject<Rootobject_TFMdj>(jsonDJ);
                tfmSong_data = JsonConvert.DeserializeObject<Rootobject_TFMsong>(jsonSong);
                tfmIsOnline = true;
            }
            catch
            {
                tfmIsOnline = false;
            }
        }
        private async Task SetTFMSongPicture()
        {
            try
            {
                HttpClient client = new HttpClient();
                Stream stream = await client.GetStreamAsync(tfmSong_data.art.ToString());
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;
                ImageBrush imageBrush = new ImageBrush(image);
                imageBrush.Stretch = Stretch.UniformToFill;
                canvas_tfmSongPicture.Background = imageBrush;
                image.EndInit();
            }
            catch
            {

            }
        }
    }
}
