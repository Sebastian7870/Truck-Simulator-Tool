using SCSSdkClient;
using SCSSdkClient.Object;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Classes;
using Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Methods;
using Xceed.Wpf.Toolkit.Panels;

namespace Truck_Simulator_Tool__WPF_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SpeedCalculations speedCalcs = new SpeedCalculations();
        public SCSSdkTelemetry Telemetry;
        public bool SdkOnline = false;
        
        bool hasBestArrival = false;
        int timeScaleConstant = 19; // Todo: change to settings
        double navigationDistance;
        double plannedDistanceKM;
        double truckSpeed;
        double jobInfoMass;
        double fuelCurrent;
        double fuelCapacity;
        double fuelRange;

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

                double updateIntervalSeconds = (double)Telemetry.UpdateInterval / 1000;
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

                    if (data.Paused)
                    {
                        label_connectionStatus.Content = "Spiel pausiert";
                        label_connectionStatus.Background = new SolidColorBrush(Colors.Goldenrod);
                    }
                    else
                    {// notPaused-only
                        label_connectionStatus.Content = "Verbunden";
                        label_connectionStatus.Background = new SolidColorBrush(Colors.LimeGreen);


                        if (ConverterMethods.GetKmHFromFVector(data.TruckValues.CurrentValues.AccelerationValues.LinearVelocity) > 5)
                        {
                            speedCalcs.timerCounter += updateIntervalSeconds;
                            speedCalcs.SetSpeedSummary(truckSpeed, updateIntervalSeconds);
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

                        TimeSpan ts_bestArrival;
                        DateTime dt_bestArrival = DateTime.Now;
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
                            if (ConverterMethods.GetKmHFromFVector(data.TruckValues.CurrentValues.AccelerationValues.LinearVelocity) > 0.01)
                            {
                                speedCalcs.SetDrivenDistance(truckSpeed, updateIntervalSeconds, data.CommonValues.Scale);
                                speedCalcs.SetDistanceSummary(navigationDistance);

                                progressBar_distance.Value = Math.Round(100 * (speedCalcs.GetDrivenDistance() / speedCalcs.GetDistanceSummary()), 2);
                                label_progressBar_distanceText.Content = $"{Math.Round(speedCalcs.GetDrivenDistance(), 1)} {UDistance} / {Math.Round(speedCalcs.GetDistanceSummary(), 1)} {UDistance}";
                                label_drivenDistanceProgress.Content = (progressBar_distance.Value / 100).ToString("p2");
                                label_remainingDistance.Content = $"Noch {navigationDistance} {UDistance}";
                            }
                        }
                    }

                    if (data.SpecialEventsValues.OnJob)
                    {// contract-only
                        label_remainingDeliveryTime.Content = $"Restzeit: {TimeSpan.FromSeconds(data.JobValues.RemainingDeliveryTime.Value * 60)}";
                        label_jobInfoFreight.Content = data.JobValues.CargoValues.Name;
                        label_jobInfoMass.Content = jobInfoMass;
                        label_jobInfoIncome.Content = $"{data.JobValues.Income.ToString("c0", UCultureInfo)} ({Math.Round(data.JobValues.Income / plannedDistanceKM), 2} {UMoneyDistance})";
                    }

                    if (data.TruckValues.CurrentValues.LightsValues.Beacon)
                        label_beaconState.Content = "eingeschaltet";
                    else
                        label_beaconState.Content = "ausgeschaltet";
                    label_averageFuelConsumption.Content = GetAverageFuelConsumptionText(data);
                    progressBar_fuel.Value = fuelCapacity / fuelCurrent * 100;
                    if (data.TruckValues.CurrentValues.DashboardValues.WarningValues.FuelW)
                        progressBar_fuel.Foreground = new SolidColorBrush(Colors.LimeGreen);
                    else
                        progressBar_fuel.Foreground = new SolidColorBrush(Colors.Brown);
                    label_progressBar_fuelText.Content = $"{fuelCurrent} {UFluid}  /  {fuelCapacity} {UFluid}  ({fuelRange} {UDistance})";

                    label_nextRestStop.Content = $"Pause in: {ConverterMethods.ConvertTimespanToCustomString(TimeSpan.FromSeconds(data.CommonValues.NextRestStop.Value * 60))}";
                    progressBar_damage.Value = data.JobValues.CargoValues.CargoDamage;
                    label_progressBar_damageText.Content =  data.JobValues.CargoValues.CargoDamage.ToString("p0");
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
                return $"{data.TruckValues.CurrentValues.DashboardValues.FuelValue.AverageConsumption.ToString("n2")} {UAverageFuelConsumption}";
            else
                return $"{ConverterMethods.ConvertEUAverageFueltoAMAverageFuel(data.TruckValues.CurrentValues.DashboardValues.FuelValue.AverageConsumption).ToString("n2")} {UAverageFuelConsumption}";
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
            truckSpeed = ConverterMethods.GetKmHFromFVector(data.TruckValues.CurrentValues.AccelerationValues.LinearVelocity);
            jobInfoMass = data.JobValues.CargoValues.Mass;
            fuelCurrent = data.TruckValues.CurrentValues.DashboardValues.FuelValue.Amount;
            fuelCapacity = data.TruckValues.ConstantsValues.CapacityValues.Fuel;
            fuelRange = data.TruckValues.CurrentValues.DashboardValues.FuelValue.Range;
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
            truckSpeed = ConverterMethods.ConvertKMtoMI(ConverterMethods.GetKmHFromFVector(data.TruckValues.CurrentValues.AccelerationValues.LinearVelocity));
            jobInfoMass = ConverterMethods.ConvertTtoLB(data.JobValues.CargoValues.Mass);
            fuelCurrent = ConverterMethods.ConvertLtoGAL(data.TruckValues.CurrentValues.DashboardValues.FuelValue.Amount);
            fuelCapacity = ConverterMethods.ConvertLtoGAL(data.TruckValues.ConstantsValues.CapacityValues.Fuel);
            fuelRange = ConverterMethods.ConvertLtoGAL(data.TruckValues.CurrentValues.DashboardValues.FuelValue.Range);
        }
    }
}
