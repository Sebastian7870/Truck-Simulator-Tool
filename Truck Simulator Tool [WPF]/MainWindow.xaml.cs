using Microsoft.Win32;
using Newtonsoft.Json;
using SCSSdkClient;
using SCSSdkClient.Object;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Truck_Simulator_Tool__WPF_.TruckSimulatorTool;
using Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Classes;
using Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Json;
using Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Methods;
using Truck_Simulator_Tool__WPF_.TruckSimulatorTool.StaticClasses;

namespace Truck_Simulator_Tool__WPF_
{
    public partial class MainWindow : Window
    {
        public bool MainWindowIsInitialized = false;
        public SCSSdkTelemetry Telemetry;
        Rootobject_TFMdj tfmDJ_data = new Rootobject_TFMdj();
        Rootobject_TFMsong tfmSong_data = new Rootobject_TFMsong();
        bool tfmIsOnline = false;

        string status = string.Empty;
        int timeScaleConstant = 19;

        public MainWindow()
        {
            ApplicationStartUp.CanStartUp();
            TSTServer.TryStart(false); //start to check if entries are available
            TSTServer.Stop();

            if (Directory.Exists($"{StaticValues.SoftwarePath}{StaticValues.ContractsPath}"))
            {
                string[] filesOlderThanOneMonth = Directory.GetFiles($"{StaticValues.SoftwarePath}{StaticValues.ContractsPath}");
                foreach (string file in filesOlderThanOneMonth)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.LastWriteTime < DateTime.Now.AddMonths(-1))
                        fileInfo.Delete();
                }
            }
            else
            {
                Directory.CreateDirectory($"{StaticValues.SoftwarePath}{StaticValues.ContractsPath}");
                //Todo : Add LogEntry
            }
            if (!Directory.Exists($"{StaticValues.SoftwarePath}{StaticValues.ShiftSchedulesPath}"))
            {
                Directory.CreateDirectory($"{StaticValues.SoftwarePath}{StaticValues.ShiftSchedulesPath}");
                //Todo : Add LogEntry
            }


            InitializeComponent();
            MainWindowIsInitialized = true;

            CalcData.HasBestArrival = false;
            Telemetry = new SCSSdkTelemetry();
            SettingsHelper.LoadCreateSettings();
            GetSettings();
            ContractHelper.StartBackupper();

            Telemetry.JobStarted += Telemetry_JobStarted;
            Telemetry.JobDelivered += Telemetry_JobDelivered;
            Telemetry.JobCancelled += Telemetry_JobCancelled;
            Telemetry.Data += Telemetry_Data;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
            timer.Start();

            dateTimePicker_shiftStart.Minimum = DateTime.Now.AddDays(-10);
            dateTimePicker_shiftStart.Value = DateTime.Now.AddDays(1);
        }

        #region "GetSettings"
        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {//SettingsWindows Button_Save
            timeScaleConstant = SettingsHelper.SettingsJson.TimeScaleValue;
            SetBackground();
        }

        private void SetBackground()
        {
            if (SettingsHelper.SettingsJson.BackgroundPath != string.Empty)
                this.Background = new ImageBrush(new BitmapImage(new Uri(SettingsHelper.SettingsJson.BackgroundPath)));
            else
                this.Background = new SolidColorBrush(Colors.LightGray);
        }

        private void GetSettings()
        {
            menuItem_antiKick.IsChecked = SettingsHelper.SettingsJson.AntiKickAutoStart;
            if (menuItem_antiKick.IsChecked)
                AntiKick.Start();
            else
                AntiKick.Stop();
            if (SettingsHelper.SettingsJson.TSTServerAutoStart)
                TSTServer.TryStart(true);
            timeScaleConstant = SettingsHelper.SettingsJson.TimeScaleValue;
            SetBackground();
        }
        #endregion

        #region "Telemetry_Data"
        private void Telemetry_JobStarted(object sender, EventArgs e)
        {
            CalcData.ResetValues(true);
            ContractHelper.ResetValues();
            ContractHelper.contractOnStartLoaded = false;
            ContractHelper.ContractJson.OdometerStartValue = CalcData.currentOdometer;
            //ContractHelper.jobStateHasChanged = true; //let it on the bottom!
        }
        private void Telemetry_JobDelivered(object sender, EventArgs e)
        {
            ContractHelper.AutoDelete();
            CalcData.ResetValues(true);
            ContractHelper.ResetValues();
            ContractHelper.contractOnStartLoaded = false;
            ContractHelper.ContractJson.OdometerStartValue = CalcData.currentOdometer;
            //ContractHelper.jobStateHasChanged = true; //let it on the bottom!
        }
        private void Telemetry_JobCancelled(object sender, EventArgs e)
        {
            ContractHelper.AutoDelete();
            CalcData.ResetValues(true);
            ContractHelper.ResetValues();
            ContractHelper.contractOnStartLoaded = false;
            ContractHelper.ContractJson.OdometerStartValue = CalcData.currentOdometer;
            //ContractHelper.jobStateHasChanged = true; //let it on the bottom!
        }

        private void Telemetry_Data(SCSTelemetry data, bool updated)
        {
            //if (!updated) return;
            try
            {
                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke(new TelemetryData(Telemetry_Data), data, updated);
                    return;
                }

                if (data.SdkActive && GeneralHelpers.SDKGameIsRunning)
                {
                    CalcData.timerInvervalFactor = (double)Telemetry.UpdateInterval / 1000;
                    CalcData.SetGameValues(data);
                    label_ingameTime.Content = CalcData.ingameTime;

                    /*if (data.SpecialEventsValues.OnJob)
                    {//OnJob
                        if (status != "OnJob")
                        {
                            CalcData.ResetValues(true);
                            ContractHelper.ResetValues();
                            ContractHelper.contractOnStartLoaded = false;
                            ContractHelper.ContractJson.OdometerStartValue = data.TruckValues.CurrentValues.DashboardValues.Odometer;
                            status = "OnJob";
                        }
                    }
                    else
                    {
                        try
                        {
                            ContractHelper.AutoDelete();
                        }
                        catch
                        {
                        }
                        if (data.NavigationValues.NavigationDistance != 0)
                        {
                            if (status != "Destination")
                            {//Destination
                                CalcData.ResetValues(true);
                                ContractHelper.ResetValues();
                                ContractHelper.contractOnStartLoaded = false;
                                ContractHelper.ContractJson.OdometerStartValue = data.TruckValues.CurrentValues.DashboardValues.Odometer;
                                status = "Destination";
                            }
                        }
                        else
                        {
                            {//FreeDrive
                                if (status != "Free")
                                {
                                    CalcData.ResetValues(true);
                                    ContractHelper.ResetValues();
                                    ContractHelper.contractOnStartLoaded = false;
                                    ContractHelper.ContractJson.OdometerStartValue = data.TruckValues.CurrentValues.DashboardValues.Odometer;
                                    status = "Free";
                                }
                            }
                        }
                    }*/

                    if (data.Paused)
                    {
                        accessText_connectionStatus.Text = "Spiel pausiert";
                        label_connectionStatus.Background = new SolidColorBrush(Colors.Goldenrod);
                    }
                    else
                    {
                        accessText_connectionStatus.Text = "Verbunden";
                        label_connectionStatus.Background = new SolidColorBrush(Colors.LimeGreen);

                        if (Math.Abs(data.TruckValues.CurrentValues.DashboardValues.Speed.Kph) > 5)
                        {
                            CalcData.timerCounter += 1;
                            label_averageSpeed.Content = $"{Math.Round(CalcData.SpeedCurrentAverage, 2)} {Unit.USpeed}";
                        }
                    }

                    if (data.NavigationValues.NavigationDistance != 0)
                    {//  destination-only
                        label_dt_currentBestArrival.Content = $"{CalcData.dt_CurrentBestArrival.ToString("HH:mm")} Uhr";
                        label_ts_currentBestArrival.Content = $"{ConverterHelper.ConvertTimespanToCustomString(CalcData.ts_CurrentBestArrival)}";

                        if (!CalcData.HasBestArrival)
                        {
                            label_dt_bestArrival.Content = $"{DateTime.Now.ToString("HH:mm")} Uhr - {CalcData.dt_BestArrival.ToString("HH:mm")} Uhr";
                            CalcData.HasBestArrival = true;
                        }

                        if (CalcData.ts_BestArrival.TotalSeconds > 0)
                            label_ts_bestArrival.Content = $"-{ConverterHelper.ConvertTimespanToCustomString(CalcData.ts_BestArrival)}";
                        else
                            label_ts_bestArrival.Content = $"+{ConverterHelper.ConvertTimespanToCustomString(TimeSpan.FromSeconds(CalcData.ts_BestArrival.TotalSeconds * (-1)))}";

                        if (!data.Paused)
                        {// destination-only + notPaused-only
                            if (CalcData.SpeedCurrentAverage != 0)
                            {
                                System.Windows.Controls.Label[] labels = { label_currentArrivalText, label_dt_currentArrival, label_ts_currentArrival };
                                if (CalcData.ts_CurrentArrival.TotalMinutes - CalcData.ts_CurrentBestArrival.TotalMinutes >= 60)
                                    SetCurrentArrivalLabelColor(labels, Colors.Brown);
                                else if (CalcData.ts_CurrentArrival.TotalMinutes - CalcData.ts_CurrentBestArrival.TotalMinutes > 30 && CalcData.ts_CurrentArrival.TotalMinutes - CalcData.ts_CurrentBestArrival.TotalMinutes < 60)
                                    SetCurrentArrivalLabelColor(labels, Colors.Goldenrod);
                                else
                                    SetCurrentArrivalLabelColor(labels, Colors.LimeGreen);

                                label_dt_currentArrival.Content = $"{CalcData.dt_CurrentArrival.ToString("HH:mm")} Uhr";
                                label_ts_currentArrival.Content = $"{ConverterHelper.ConvertTimespanToCustomString(CalcData.ts_CurrentArrival)}";
                            }

                            if (Math.Abs(data.TruckValues.CurrentValues.DashboardValues.Speed.Kph) > 0.1)
                            {
                                progressBar_distance.Value = Math.Round(100 * (CalcData.DistanceDriven / CalcData.DistanceSummary), 2);
                                label_progressBar_distanceText.Content = $"{Math.Round(CalcData.DistanceDriven, 1)} {Unit.UDistance} / {Math.Round(CalcData.DistanceSummary, 1)}";
                               label_drivenDistanceProgress.Content = (progressBar_distance.Value / 100).ToString("p2");
                                label_remainingDistance.Content = $"Noch {Math.Round(CalcData.navigationDistanceC, 0)} {Unit.UDistance}";
                            }
                        }
                    }
                    if (data.SpecialEventsValues.OnJob)
                    {// onJob-only
                        if (CalcData.ts_RemainingTime.TotalSeconds < 0)
                        {
                            if (CalcData.ts_RemainingTime.TotalHours < 500)
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
                            if (CalcData.ts_RemainingTime.TotalHours < 3)
                                label_remainingDeliveryTime.Foreground = new SolidColorBrush(Colors.Goldenrod);
                            else
                                label_remainingDeliveryTime.Foreground = new SolidColorBrush(Colors.LimeGreen);
                            
                            label_remainingDeliveryTime.Content = $"Restzeit: {ConverterHelper.ConvertTimespanToCustomString(CalcData.ts_RemainingTime)}";
                        }

                        if (CalcData.ts_Timebuffer.TotalSeconds <= 0)
                            label_timebuffer.Background = new SolidColorBrush(Colors.Brown);
                        else if (CalcData.ts_Timebuffer.TotalHours <= 5 && CalcData.ts_Timebuffer.TotalHours > 0)
                            label_timebuffer.Background = new SolidColorBrush(Colors.Goldenrod);
                        else
                            label_timebuffer.Background = new SolidColorBrush(Colors.LimeGreen);

                        if (data.NavigationValues.NavigationTime < 100000000)
                        {
                            label_timebuffer.Content = $"Zeitpuffer: {ConverterHelper.ConvertTimespanToCustomString(CalcData.ts_Timebuffer)}";
                        }
                        else
                        {
                            label_timebuffer.Background = new SolidColorBrush(Colors.CornflowerBlue);
                            label_timebuffer.Content = "Zeitpuffer: WoT";
                        }
                    }

                    //label beacon
                    if (data.TruckValues.CurrentValues.LightsValues.Beacon)
                        label_beaconState.Content = "eingeschaltet";
                    else
                        label_beaconState.Content = "ausgeschaltet";
                    //label fuel
                    label_averageFuelConsumption.Content = $"{CalcData.fuelAverageConsumptionC.ToString("n2")} {Unit.UAverageFuelConsumption}";
                    progressBar_fuel.Value = CalcData.fuelCurrent / CalcData.fuelCapacity * 100;
                    label_progressBar_fuelText.Content = $"{Math.Round(CalcData.fuelCurrent, 0)} {Unit.UFluid}  /  {Math.Round(CalcData.fuelCapacity),0} {Unit.UFluid}  ({Math.Round(CalcData.fuelRange, 0)} {Unit.UDistance})";
                    if (data.TruckValues.CurrentValues.DashboardValues.WarningValues.FuelW)
                        progressBar_fuel.Foreground = new SolidColorBrush(Colors.Brown);
                    else
                        progressBar_fuel.Foreground = new SolidColorBrush(Colors.LimeGreen);
                    //label nextRestStop
                    TimeSpan ts_nextPauseTime = TimeSpan.FromSeconds(Math.Abs(data.CommonValues.NextRestStop.Value) * 60);
                    label_nextRestStop.Content = $"Pause in: {ConverterHelper.ConvertTimespanToCustomString(ts_nextPauseTime)}";
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
                    //progressBar damage
                    progressBar_damage.Value = data.JobValues.CargoValues.CargoDamage;
                    label_progressBar_damageText.Content = data.JobValues.CargoValues.CargoDamage.ToString("p0");

                    //DataSetter (example: calculatedData)
                    ContractHelper.sdkActive = true;
                    ContractHelper.onJob = data.SpecialEventsValues.OnJob;
                    /*if (ContractHelper.jobStateHasChanged)
                    {
                        ContractHelper.ContractJson.OdometerStartValue = data.TruckValues.CurrentValues.DashboardValues.Odometer;
                        CalcData.ResetValues(true);
                        ContractHelper.ResetValues();
                        ContractHelper.jobStateHasChanged = false;
                        ContractHelper.contractOnStartLoaded = false;
                    }*/

                    ContractJson contractJson = new ContractJson();
                    contractJson.Game = data.Game.ToString();
                    contractJson.CitySource = data.JobValues.CitySource;
                    contractJson.CityDestination = data.JobValues.CityDestination;
                    contractJson.Income = data.JobValues.Income;
                    contractJson.Mass = data.JobValues.CargoValues.Mass;
                    contractJson.OdometerStartValue = ContractHelper.ContractJson.OdometerStartValue;
                    contractJson.timerCounter = CalcData.timerCounter;
                    contractJson.speedSummary = CalcData.SpeedSummary;
                    contractJson.distanceDriven = CalcData.DistanceDriven;
                    contractJson.distanceSummary = CalcData.DistanceSummary;
                    ContractHelper.ContractJson = contractJson;

                    ContractHelper.AutoLoadIfStartup();
                }
                else
                {
                    ResetSDK();
                }
            }
            catch
            {
                // Todo: Add Log Entries
            }
        }

        private void ResetSDK()
        {
            accessText_connectionStatus.Text = "Keine Verbindung zum Spiel";
            label_connectionStatus.Background = new SolidColorBrush(Colors.Brown);
            ContractHelper.sdkActive = false;
            ContractHelper.onJob = false;
            ContractHelper.contractOnStartLoaded = false;
            ContractHelper.ResetValues();
            CalcData.ResetValues(true);
        }

        private void SetCurrentArrivalLabelColor(System.Windows.Controls.Label[] labels, Color color)
        {
            SolidColorBrush colBrush = new SolidColorBrush(color);
            label_currentArrivalText.Background = colBrush;
            label_dt_currentArrival.Background = colBrush;
            label_ts_currentArrival.Background = colBrush;
        }
        #endregion

        #region "Timer_Tick"
        private bool tstServerWasConnected = true;
        private string lastTFMPicturePath = null;
        private async void Timer_Tick(object sender, EventArgs e)
        {
            label_dateTimeNowSeconds.Content = DateTime.Now.Second.ToString();
            label_dateTimeNowTime.Content = DateTime.Now.ToString("HH:mm");
            label_dateTimeNowDate.Content = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(DateTime.Now.DayOfWeek)}\n{DateTime.Now.Date.ToShortDateString()}";
            if (radioButton_distanceCalculatorDefault.IsChecked ?? false)
                label_distanceCalculatorTimeScale.Content = $"Zeitskalierung: {timeScaleConstant}";

            await UpdateTFM();
            if (tfmIsOnline)
            {
                if (tfmSong_data.art.ToString() != lastTFMPicturePath)
                    await SetTFMSongPicture();
                label_tfmSongTitle.Content = tfmSong_data.title;
                label_tfmSongAuthor.Content = tfmSong_data.artist;
                label_tfmDJName.Content = $"DJ {tfmDJ_data.result.dj.name}";
                TimeSpan ts = TimeSpan.FromSeconds(Convert.ToDouble(tfmDJ_data.result.slot.timeend) - Convert.ToDouble(tfmDJ_data.result.slot.timestart));
                label_tfmDuration.Content = $"{ts.TotalHours} Std.";
                lastTFMPicturePath = tfmSong_data.art.ToString();
            }
            else
            {
                canvas_tfmSongPicture.Background = new SolidColorBrush(Colors.Transparent);
                label_tfmSongTitle.Content = "";
                label_tfmSongAuthor.Content = "";
                label_tfmDJName.Content = "";
                label_tfmDuration.Content = "";
            }

            if (ShiftSchedule.HasShift)
            {
                ShiftSchedule.Update();

                label_shiftCount.Visibility = Visibility.Visible;
                label_nextShiftEvent.Visibility = Visibility.Visible;
                label_shiftTimeLeft.Visibility = Visibility.Visible;
                label_nextShiftPause.Visibility = Visibility.Visible;
                label_currentShift.Visibility = Visibility.Visible;
                menuItem_shiftScheduleSave.IsEnabled = true;

                if (ShiftSchedule.CurrentShiftIsActive)
                {
                    if (ShiftSchedule.ShiftPaused)
                    {
                        accessText_shiftStatus.Text = "Schichtpause";
                        label_shiftStatus.Background = new SolidColorBrush(Colors.CornflowerBlue);
                    }
                    else
                    {
                        accessText_shiftStatus.Text = "Schicht aktiv";
                        label_shiftStatus.Background = new SolidColorBrush(Colors.LimeGreen);
                    }
                }
                else
                {
                    accessText_shiftStatus.Text = "Schicht nicht aktiv";
                    label_shiftStatus.Background = new SolidColorBrush(Colors.Goldenrod);
                }
                button_shiftLoadDelete.Content = "Schichtplan löschen";
                button_shiftLoadDelete.Background = new SolidColorBrush(Colors.Brown);

                label_shiftCount.Content = $"Schicht: {ShiftSchedule.ShiftCount}";
                label_nextShiftEvent.Content = $"Nächstes Schichtereignis: {ReturnNextShiftEventString(ShiftSchedule.NextShiftEvent)}";
                if (ShiftSchedule.CurrentShiftIsActive)
                {
                    label_shiftTimeLeft.Content = $"Übrige Schichtlänge: {ConverterHelper.ConvertTimespanToCustomString(ShiftSchedule.NextShiftEnd - DateTime.Now)}";
                    label_currentShift.Content = $"Derzeitige Schicht: {ShiftSchedule.CurrentShiftStartEnd[0].ToString("HH:mm")} Uhr,  {ShiftSchedule.CurrentShiftStartEnd[0].ToShortDateString()}   -   {ShiftSchedule.CurrentShiftStartEnd[1].ToString("HH:mm")} Uhr,  {ShiftSchedule.CurrentShiftStartEnd[1].ToShortDateString()}";

                    if (ShiftSchedule.CurrentShiftHasPause)
                    {
                        if (ShiftSchedule.ShiftPaused)
                            label_nextShiftPause.Content = $"Pausenende in: {ConverterHelper.ConvertTimespanToCustomString(ShiftSchedule.NextShiftPauseEnd - DateTime.Now)}";
                        else
                            label_nextShiftPause.Content = $"Nächste Pause in: {ConverterHelper.ConvertTimespanToCustomString(ShiftSchedule.NextShiftPauseStart - DateTime.Now)}";
                    }
                    else
                    {
                        label_nextShiftPause.Content = "Nächste Pause in: ---";
                    }
                }
                else
                {
                    label_shiftTimeLeft.Content = "Übrige Schichtlänge: ---";
                    label_nextShiftPause.Content = "Nächste Pause in: ---";
                    label_currentShift.Content = "Derzeitige Schicht: ---";
                }
            }
            else
            {
                ResetShiftScheduleLabels();
                listView_shiftScheduleText.Items.Clear();
                button_shiftLoadDelete.Content = "Schichtplan laden";
                button_shiftLoadDelete.Background = new SolidColorBrush(Colors.LightSteelBlue);
                accessText_shiftStatus.Text = "Keine Schicht geladen";
                label_shiftStatus.Background = new SolidColorBrush(Colors.Brown);
                label_shiftCount.Visibility = Visibility.Hidden;
                label_nextShiftEvent.Visibility = Visibility.Hidden;
                label_shiftTimeLeft.Visibility = Visibility.Hidden;
                label_nextShiftPause.Visibility = Visibility.Hidden;
                label_currentShift.Visibility = Visibility.Hidden;
                menuItem_shiftScheduleSave.IsEnabled = false;
            }

            if (tstServerWasConnected && StaticValues.FullIPAddress.Contains("127.0.0.1") || StaticValues.FullIPAddress.Contains("none"))
            {// = no internet connection
                TSTServer.Stop();
                tstServerWasConnected = false;
                MessageBox.Show("Es konnte keine IP Adresse gefunden werden. Überprüfen Sie bitte die Internetverbindung.", "Keine Verbindung zum Internet!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (TSTServer.IsOnline)
            {
                menuItem_serverStatus.Background = new SolidColorBrush(Colors.LimeGreen);
                menuItem_serverStart.Header = "Server stoppen";
                menuItem_serverIP.IsEnabled = true;
                tstServerWasConnected = true;
            }
            else
            {
                menuItem_serverStatus.Background = new SolidColorBrush(Colors.Brown);
                menuItem_serverStart.Header = "Server starten";
                menuItem_serverIP.IsEnabled = false;
                tstServerWasConnected = false;
            }

            if (TSTServer.HasEntries)
            {
                menuItem_serverStart.IsEnabled = true;
                menuItem_serverInstall.Header = "Server reinstallieren";
            }
            else
            {
                menuItem_serverStart.IsEnabled = false;
                menuItem_serverInstall.Header = "Server installieren";
            }
        }
        #endregion

        #region "TruckersFM"
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
        #endregion

        #region "Shift Schedule"
        private void button_shiftCreate_Click(object sender, RoutedEventArgs e)
        {
            if (!ShiftSchedule.HasShift)
            {
                CreateShiftSchedule();
            }
            else
            {
                if (MessageBox.Show("Sie haben zurzeit eine Schicht geladen, möchten Sie diese ersetzen?", "Schicht schon aktiv!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    CreateShiftSchedule();
                }
            }
        }
        private void CreateShiftSchedule()
        {
            if (dateTimePicker_shiftStart.Value.HasValue && doubleUpDown_driveTimeHours.Value != null && integerUpDown_durationDays.Value != null && doubleUpDown_pauseTimeHours.Value != null)
            {
                ShiftSchedule.CreateShift((DateTime)dateTimePicker_shiftStart.Value, (int)integerUpDown_durationDays.Value, (double)doubleUpDown_driveTimeHours.Value, (double)doubleUpDown_pauseTimeHours.Value);
                SetShiftScheduleTextView();
            }
            else
            {
                MessageBox.Show("Es konnte kein Schichtplan erstellt werden, da Werte fehlen.", "Fehlende Werte!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void button_shiftScheduleLoadDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!ShiftSchedule.HasShift)
            {
                LoadShiftSchedule();
            }
            else
            {
                DeleteShiftSchedule();
            }
        }

        private void SaveShiftSchedule()
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "json|*.json";
            fileDialog.InitialDirectory = $@"{StaticValues.SoftwarePath}{StaticValues.ShiftSchedulesPath}";
            if (!fileDialog.CheckPathExists)
                fileDialog.InitialDirectory = null;
            if (fileDialog.ShowDialog() == true)
            {
                string json = JsonConvert.SerializeObject(ShiftSchedule.Getlist_ShiftScheduleJson);
                File.WriteAllText(fileDialog.FileName, json);
            }
        }

        private void LoadShiftSchedule()
        {
            if (ShiftSchedule.HasShift)
                if (MessageBox.Show("Sie haben bereits einen Schichtplan geladen. Wenn Sie den Schichtplan nicht gespeichert haben, wird er dauerhaft gelöscht.", "Soll der derzeitige Schichtplan gelöscht werden?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    return;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "json|*.json";
            fileDialog.InitialDirectory = $@"{StaticValues.SoftwarePath}{StaticValues.ShiftSchedulesPath}";
            if (!fileDialog.CheckPathExists)
                fileDialog.InitialDirectory = null;
            if (fileDialog.ShowDialog() == true)
            {
                ShiftSchedule.LoadShift(fileDialog.FileName);
                SetShiftScheduleTextView();
            }
        }

        private void DeleteShiftSchedule()
        {
            ShiftSchedule.DeleteShift();
            ResetShiftScheduleLabels();
            listView_shiftScheduleText.Items.Clear();
        }

        private void ResetShiftScheduleLabels()
        {
            label_shiftCount.Content = "Schicht: ---";
            label_nextShiftEvent.Content = "Nächstes Schichtereignis: ---";
            label_shiftTimeLeft.Content = "Übrige Schichtlänge: ---";
            label_nextShiftPause.Content = "Nächste Pause in: ---";
            label_currentShift.Content = "Derzeitige Schicht: ---";
        }

        private void SetShiftScheduleTextView()
        {
            if (ShiftSchedule.HasShift)
            {
                listView_shiftScheduleText.Items.Clear();
                foreach (ShiftScheduleJson Item in ShiftSchedule.Getlist_ShiftScheduleJson)
                {
                    ListBoxItem count = new ListBoxItem();
                    count.Content = $"Schicht: {Item.Count}";
                    count.Foreground = new SolidColorBrush(Colors.CornflowerBlue);

                    listView_shiftScheduleText.Items.Add(count);//$"Schich: {Item.Count}");
                    listView_shiftScheduleText.Items.Add(new Separator());
                    listView_shiftScheduleText.Items.Add($"Schichtbeginn_______:    {Item.StartDate}          [{DateTimeFormatInfo.CurrentInfo.GetDayName(Item.StartDate.DayOfWeek)}]");
                    listView_shiftScheduleText.Items.Add($"Schichtpausenbeginn_:    {Item.StartPause}          [{DateTimeFormatInfo.CurrentInfo.GetDayName(Item.StartPause.DayOfWeek)}]");
                    listView_shiftScheduleText.Items.Add(new Separator());
                    listView_shiftScheduleText.Items.Add($"Schichtpausenende___:    {Item.EndPause}          [{DateTimeFormatInfo.CurrentInfo.GetDayName(Item.EndPause.DayOfWeek)}]");
                    listView_shiftScheduleText.Items.Add($"Schichtende_________:    {Item.EndDate}          [{DateTimeFormatInfo.CurrentInfo.GetDayName(Item.EndDate.DayOfWeek)}]");
                    listView_shiftScheduleText.Items.Add("\n");
                }
            }
        }

        private string ReturnNextShiftEventString(Tuple<DateTime, int, ShiftSchedule.IndexType> tuple)
        {
            switch (tuple.Item3)
            {
                case ShiftSchedule.IndexType.startDate:
                    return $"[Schichtbeginn]  {tuple.Item1.ToString("HH:mm")} Uhr, {tuple.Item1.ToShortDateString()}";
                case ShiftSchedule.IndexType.endDate:
                    return $"[Schichtende]  {tuple.Item1.ToString("HH:mm")} Uhr, {tuple.Item1.ToShortDateString()}";
                case ShiftSchedule.IndexType.startPause:
                    return $"[Schichtpausenbeginn]  {tuple.Item1.ToString("HH:mm")} Uhr, {tuple.Item1.ToShortDateString()}";
                case ShiftSchedule.IndexType.endPause:
                    return $"[Schichtpausenende]  {tuple.Item1.ToString("HH:mm")} Uhr, {tuple.Item1.ToShortDateString()}";
                default:
                    return "---";
            }
        }
        #endregion

        #region "Distance Calculator"
        public enum calc_LastChangedType
        {
            Time,
            Speed,
            Distance,
        };
        private bool calc_IsDefault
        {
            get
            {
                if ((bool)radioButton_distanceCalculatorDefault.IsChecked)
                    return true;
                else
                    return false;
            }
        }
        private bool calc_distanceWasLastChanged = false;
        private void doubleUpDown_distanceCalculatorTime1_GotFocus(object sender, RoutedEventArgs e)
        {
            calc_distanceWasLastChanged = false;
        }
        private void doubleUpDown_distanceCalculatorTime2_GotFocus(object sender, RoutedEventArgs e)
        {
            calc_distanceWasLastChanged = false;
        }
        private void doubleUpDown_distanceCalculatorTime3_GotFocus(object sender, RoutedEventArgs e)
        {
            calc_distanceWasLastChanged = false;
        }
        private void integerUpDown_distanceCalculatorDistance_GotFocus(object sender, RoutedEventArgs e)
        {
            calc_distanceWasLastChanged = true;
        }

        private void doubleUpDown_distanceCalculatorTime1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!calc_distanceWasLastChanged)
                UpdateDistanceCalculator(calc_LastChangedType.Time);
        }
        private void doubleUpDown_distanceCalculatorTime2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!calc_distanceWasLastChanged)
                UpdateDistanceCalculator(calc_LastChangedType.Time);
        }
        private void doubleUpDown_distanceCalculatorTime3_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!calc_distanceWasLastChanged)
                UpdateDistanceCalculator(calc_LastChangedType.Time);
        }
        private void integerUpDown_distanceCalculatorAverageSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            UpdateDistanceCalculator(calc_LastChangedType.Speed);
        }
        private void integerUpDown_distanceCalculatorDistance_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (calc_distanceWasLastChanged)
                UpdateDistanceCalculator(calc_LastChangedType.Distance);
        }

        private void radioButton_distanceCalculatorDefault_Click(object sender, RoutedEventArgs e)
        {
            label_distanceCalculatorText1_1.Content = "Fahrzeit:";
            label_distanceCalculatorTimeScale.Content = $"Zeitskalierung: {timeScaleConstant}";
            label_distanceCalculatorText1_2.Visibility = Visibility.Hidden;
            label_distanceCalculatorText1_3.Visibility = Visibility.Hidden;
            doubleUpDown_distanceCalculatorTime2.Visibility = Visibility.Hidden;
            doubleUpDown_distanceCalculatorTime3.Visibility = Visibility.Hidden;
            label_distanceCalculatorText2_2.Visibility = Visibility.Hidden;
            label_distanceCalculatorText2_3.Visibility = Visibility.Hidden;
            if (calc_distanceWasLastChanged)
                UpdateDistanceCalculator(calc_LastChangedType.Distance);
            else
                UpdateDistanceCalculator(calc_LastChangedType.Time);
        }
        private void radioButton_distanceCalculatorExtended_Click(object sender, RoutedEventArgs e)
        {
            doubleUpDown_distanceCalculatorTime2.Value = 0;
            doubleUpDown_distanceCalculatorTime3.Value = 0;
            label_distanceCalculatorText1_1.Content = "Fahrzeit (19):";
            label_distanceCalculatorTimeScale.Content = "Zeitskalierung: individuell";
            label_distanceCalculatorText1_2.Visibility = Visibility.Visible;
            label_distanceCalculatorText1_3.Visibility = Visibility.Visible;
            doubleUpDown_distanceCalculatorTime2.Visibility = Visibility.Visible;
            doubleUpDown_distanceCalculatorTime3.Visibility = Visibility.Visible;
            label_distanceCalculatorText2_2.Visibility = Visibility.Visible;
            label_distanceCalculatorText2_3.Visibility = Visibility.Visible;
            if (calc_distanceWasLastChanged)
                UpdateDistanceCalculator(calc_LastChangedType.Distance);
            else
                UpdateDistanceCalculator(calc_LastChangedType.Time);
        }

        private void UpdateDistanceCalculator(calc_LastChangedType lastChangedType)
        {

            if (MainWindowIsInitialized)
            {
                switch (lastChangedType)
                {
                    case calc_LastChangedType.Time:
                        if (calc_IsDefault)
                            integerUpDown_distanceCalculatorDistance.Value = (int)(timeScaleConstant * doubleUpDown_distanceCalculatorTime1.Value * integerUpDown_distanceCalculatorAverageSpeed.Value);
                        else
                            integerUpDown_distanceCalculatorDistance.Value = (int)(((19 * doubleUpDown_distanceCalculatorTime1.Value) + (15 * doubleUpDown_distanceCalculatorTime2.Value) + (3 * doubleUpDown_distanceCalculatorTime3.Value)) * integerUpDown_distanceCalculatorAverageSpeed.Value);
                        break;

                    case calc_LastChangedType.Speed:
                        if (calc_distanceWasLastChanged)
                        {
                            if (calc_IsDefault)
                                doubleUpDown_distanceCalculatorTime1.Value = (double)((integerUpDown_distanceCalculatorDistance.Value / timeScaleConstant) / integerUpDown_distanceCalculatorAverageSpeed.Value);
                            else
                            {
                                doubleUpDown_distanceCalculatorTime1.Value = (double)Math.Round((double)((Convert.ToDouble(integerUpDown_distanceCalculatorDistance.Value) / 19) / integerUpDown_distanceCalculatorAverageSpeed.Value), 2);
                                doubleUpDown_distanceCalculatorTime2.Value = (double)Math.Round((double)((Convert.ToDouble(integerUpDown_distanceCalculatorDistance.Value) / 15) / integerUpDown_distanceCalculatorAverageSpeed.Value), 2);
                                doubleUpDown_distanceCalculatorTime3.Value = (double)Math.Round((double)((Convert.ToDouble(integerUpDown_distanceCalculatorDistance.Value) / 3) / integerUpDown_distanceCalculatorAverageSpeed.Value), 2);
                            }
                        }
                        else
                        {
                            if (calc_IsDefault)
                                integerUpDown_distanceCalculatorDistance.Value = (int)(timeScaleConstant * doubleUpDown_distanceCalculatorTime1.Value * integerUpDown_distanceCalculatorAverageSpeed.Value);
                            else
                                integerUpDown_distanceCalculatorDistance.Value = (int)(((19 * doubleUpDown_distanceCalculatorTime1.Value) + (15 * doubleUpDown_distanceCalculatorTime2.Value) + (3 * doubleUpDown_distanceCalculatorTime3.Value)) * integerUpDown_distanceCalculatorAverageSpeed.Value);
                        }
                        break;

                    case calc_LastChangedType.Distance:
                        if (calc_IsDefault)
                            doubleUpDown_distanceCalculatorTime1.Value = (double)Math.Round((double)((Convert.ToDouble(integerUpDown_distanceCalculatorDistance.Value) / timeScaleConstant) / integerUpDown_distanceCalculatorAverageSpeed.Value), 2);
                        else
                        {
                            doubleUpDown_distanceCalculatorTime1.Value = (double)Math.Round((double)((Convert.ToDouble(integerUpDown_distanceCalculatorDistance.Value) / 19) / integerUpDown_distanceCalculatorAverageSpeed.Value), 2);
                            doubleUpDown_distanceCalculatorTime2.Value = (double)Math.Round((double)((Convert.ToDouble(integerUpDown_distanceCalculatorDistance.Value) / 15) / integerUpDown_distanceCalculatorAverageSpeed.Value), 2);
                            doubleUpDown_distanceCalculatorTime3.Value = Math.Round((double)((Convert.ToDouble(integerUpDown_distanceCalculatorDistance.Value) / 3) / integerUpDown_distanceCalculatorAverageSpeed.Value), 2);
                        }
                        break;
                }
            }
        }
        #endregion

        #region "Menu Items"
        private void menuItem_shiftScheduleLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadShiftSchedule();
        }
        private void menuItem_shiftScheduleSave_Click(object sender, RoutedEventArgs e)
        {
            SaveShiftSchedule();
        }

        private void menuItem_settings_Click(object sender, RoutedEventArgs e)
        {
            if (!IsWindowOpen<SettingsWindow>())
            {
                SettingsWindow window = new SettingsWindow();
                window.Show();
                window.button_Save.Click += Button_Save_Click;
            }
        }
        public static bool IsWindowOpen<T>(string name = "") where T : Window
        {
            return string.IsNullOrEmpty(name)
               ? Application.Current.Windows.OfType<T>().Any()
               : Application.Current.Windows.OfType<T>().Any(w => w.Name.Equals(name));
        }

        private void menuItem_antiKick_Click(object sender, RoutedEventArgs e)
        {
            if (menuItem_antiKick.IsChecked)
                AntiKick.Start();
            else
                AntiKick.Stop();
        }

        private void menuItem_serverInstall_Click(object sender, RoutedEventArgs e)
        {// even if header says "re-install" or just "install" it re-installs in both scenarios to prevent multiple entries.
            TSTServer.ReSetPowerShellEntries();
        }
        private void menuItem_serverUninstall_Click(object sender, RoutedEventArgs e)
        {
            TSTServer.DeletePowerShellEntries();
        }
        private void menuItem_serverStart_Click(object sender, RoutedEventArgs e)
        {
            if (TSTServer.IsOnline)
                TSTServer.Stop();
            else
                TSTServer.TryStart(true);
        }
        private void menuItem_serverIP_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"{StaticValues.FullIPAddress}   (Port: {StaticValues.Port})", "IP Adresse", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion

        #region "MainWindow ImportantEvents"
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ContractHelper.TryAutoSave();
            // Todo: add message box warning if trying to leave and manual saving is on. (for that add [static bool SettingsSaved] and then you can set label_contractStatus color to Goldenrod as well)
        }
        #endregion
    }
}