using Microsoft.Win32;
using Newtonsoft.Json;
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
        Rootobject_Telemetry data = new Rootobject_Telemetry();
        Rootobject_TFMdj tfmDJ_data = new Rootobject_TFMdj();
        Rootobject_TFMsong tfmSong_data = new Rootobject_TFMsong();
        bool tfmIsOnline = false;
        bool telemetryIsOnline = false;

        DispatcherTimer timer_telemetry;

        int timeScaleConstant
        {
            get
            {
                if (SettingsHelper.SettingsJson != null)
                    return SettingsHelper.SettingsJson.TimeScaleValue;
                else
                    return 19;
            }
        }

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

            SettingsHelper.LoadCreateSettings();
            GetSettingsOnStartup();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
            timer.Start();

            timer_telemetry = new DispatcherTimer();
            timer_telemetry.Interval = TimeSpan.FromMilliseconds(100);
            timer_telemetry.Tick += timer_telemetry_Tick;
            timer_telemetry.Start();

            dateTimePicker_shiftStart.Minimum = DateTime.Now.AddDays(-10);
            dateTimePicker_shiftStart.Value = DateTime.Now.AddDays(1);
        }

        #region "GetSettings"
        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {//SettingsWindows Button_Save
            SetBackground();
        }

        private void SetBackground()
        {
            if (SettingsHelper.SettingsJson.BackgroundPath != string.Empty)
            {
                ImageBrush imageBrush = new ImageBrush(new BitmapImage(new Uri(SettingsHelper.SettingsJson.BackgroundPath)));
                imageBrush.Stretch = Stretch.UniformToFill;
                this.Background = imageBrush;
            }
            else
                this.Background = new SolidColorBrush(Colors.LightGray);
        }

        private void GetSettingsOnStartup()
        {
            menuItem_antiKick.IsChecked = SettingsHelper.SettingsJson.AntiKickAutoStart;
            if (menuItem_antiKick.IsChecked)
                AntiKick.Start();
            else
                AntiKick.Stop();
            if (SettingsHelper.SettingsJson.TSTServerAutoStart)
                TSTServer.TryStart(true);
            SetBackground();
        }
        #endregion

        #region "Telemetry_Data"

        private async void timer_telemetry_Tick(object sender, EventArgs e)
        {
            await UpdateTelemetry();
            SetTSTServerMessage();

            if (!telemetryIsOnline)
            {
                accessText_connectionStatus.Text = "Keine Verbindung zum Server";
                label_connectionStatus.Background = new SolidColorBrush(Colors.Brown);

                ResetSDK();
            }
            else
            {
                if (!data.ets2.game.connected)
                {//notConnected
                    accessText_connectionStatus.Text = "Keine Verbindung zum Spiel";
                    label_connectionStatus.Background = new SolidColorBrush(Colors.Brown);

                    ResetSDK();
                }
                else
                {//connected
                    CalcData.SetGameValues(data);
                    if (!CalcData.IsAllowedToUpdate && !data.ets2.game.paused)
                        CalcData.IsAllowedToUpdate = true;

                    CalcData.timerInvervalFactor = timer_telemetry.Interval.TotalSeconds;

                    if (data.ets2.game.paused)
                    {// paused-only
                        accessText_connectionStatus.Text = "Spiel pausiert";
                        label_connectionStatus.Background = new SolidColorBrush(Colors.Goldenrod);
                    }
                    else
                    {// notPaused-only
                        accessText_connectionStatus.Text = "Verbunden";
                        label_connectionStatus.Background = new SolidColorBrush(Colors.LimeGreen);

                        if (Math.Abs(data.ets2.truck.speed) > 5)
                        {
                            CalcData.timerCounter += 1;
                        }
                    }
                    label_averageSpeed.Content = $"{Math.Round(CalcData.SpeedCurrentAverage, 2)} {Unit.USpeed}";

                    label_dt_currentBestArrival.Content = $"{CalcData.dt_CurrentBestArrival.ToString("HH:mm")} Uhr";
                    label_ts_currentBestArrival.Content = $"{ConverterHelper.ConvertTimespanToCustomString(CalcData.ts_CurrentBestArrival)}";

                    if (CalcData.dt_BestArrival > DateTime.Now.AddDays(-10))
                        label_dt_bestArrival.Content = $"{CalcData.dt_BestArrivalStart.ToString("HH:mm")} Uhr - {CalcData.dt_BestArrival.ToString("HH:mm")} Uhr";
                    else
                        label_dt_bestArrival.Content = "00:00 Uhr - 00:00 Uhr";

                    if (CalcData.ts_BestArrival.TotalSeconds > 0)
                        label_ts_bestArrival.Content = $"-{ConverterHelper.ConvertTimespanToCustomString(CalcData.ts_BestArrival)}";
                    else
                        label_ts_bestArrival.Content = $"+{ConverterHelper.ConvertTimespanToCustomString(TimeSpan.FromSeconds(CalcData.ts_BestArrival.TotalSeconds * (-1)))}";

                    System.Windows.Controls.Label[] labels = { label_currentArrivalText, label_dt_currentArrival, label_ts_currentArrival };
                    if (CalcData.ts_CurrentArrival.TotalMinutes - CalcData.ts_CurrentBestArrival.TotalMinutes >= 60)
                        SetCurrentArrivalLabelColor(labels, Colors.Brown);
                    else if (CalcData.ts_CurrentArrival.TotalMinutes - CalcData.ts_CurrentBestArrival.TotalMinutes > 30 && CalcData.ts_CurrentArrival.TotalMinutes - CalcData.ts_CurrentBestArrival.TotalMinutes < 60)
                        SetCurrentArrivalLabelColor(labels, Colors.Goldenrod);
                    else
                        SetCurrentArrivalLabelColor(labels, Colors.LimeGreen);

                    label_dt_currentArrival.Content = $"{CalcData.dt_CurrentArrival.ToString("HH:mm")} Uhr";
                    label_ts_currentArrival.Content = $"{ConverterHelper.ConvertTimespanToCustomString(CalcData.ts_CurrentArrival)}";

                    if (!(CalcData.DistanceDriven / CalcData.DistanceSummary).Equals(double.NaN))
                        progressBar_distance.Value = Math.Round(100 * (CalcData.DistanceDriven / CalcData.DistanceSummary), 2);
                    else
                        progressBar_distance.Value = 0;
                    label_progressBar_distanceText.Content = $"{Math.Round(CalcData.DistanceDriven, 1)} {Unit.UDistance} / {Math.Round(CalcData.DistanceSummary, 1)} {Unit.UDistance}";
                    label_drivenDistanceProgress.Content = (progressBar_distance.Value / 100).ToString("p2");
                    label_remainingDistance.Content = $"Noch {Math.Round(Unit.navigationDistanceC, 0)} {Unit.UDistance}";

                    label_citySource.Content = data.ets2.job.sourceCity;
                    label_cityDestination.Content = data.ets2.job.destinationCity;
                    label_companySource.Content = data.ets2.job.sourceCompany;
                    label_companyDestination.Content = data.ets2.job.destinationCompany;
                    // label remainingTime
                    if (CalcData.ts_RemainingTime.TotalSeconds <= 0)
                    {
                        label_remainingDeliveryTime.Foreground = new SolidColorBrush(Colors.Brown);
                        label_remainingDeliveryTime.Content = "Restzeit: 0 Min.";
                    }
                    else
                    {
                        if (CalcData.ts_RemainingTime.TotalHours < 3)
                        {
                            label_remainingDeliveryTime.Foreground = new SolidColorBrush(Colors.Brown);
                            label_remainingDeliveryTime.Content = $"Restzeit: {ConverterHelper.ConvertTimespanToCustomString(CalcData.ts_RemainingTime)}";
                        }
                        else
                        {
                            if (CalcData.ts_RemainingTime.TotalDays > 10)
                            {
                                label_remainingDeliveryTime.Foreground = new SolidColorBrush(Colors.CornflowerBlue);
                                label_remainingDeliveryTime.Content = "Restzeit: WoT";
                            }
                            else
                            {
                                if (CalcData.ts_RemainingTime.TotalHours < 3)
                                    label_remainingDeliveryTime.Foreground = new SolidColorBrush(Colors.Goldenrod);
                                else
                                    label_remainingDeliveryTime.Foreground = new SolidColorBrush(Colors.LimeGreen);
                                label_remainingDeliveryTime.Content = $"Restzeit: {ConverterHelper.ConvertTimespanToCustomString(CalcData.ts_RemainingTime)}";
                            }
                        }
                    }
                    //label timeBuffer
                    if (CalcData.ts_Timebuffer.TotalSeconds <= 0)
                        label_timebuffer.Background = new SolidColorBrush(Colors.Brown);
                    else if (CalcData.ts_Timebuffer.TotalHours <= 5 && CalcData.ts_Timebuffer.TotalHours > 0)
                        label_timebuffer.Background = new SolidColorBrush(Colors.Goldenrod);
                    else
                        label_timebuffer.Background = new SolidColorBrush(Colors.LimeGreen);
                    if (TimeSpan.FromSeconds(data.ets2.job.remainingTime).TotalDays > 10)
                    {
                        label_timebuffer.Background = new SolidColorBrush(Colors.CornflowerBlue);
                        label_timebuffer.Content = "Zeitpuffer: WoT";
                    }
                    else
                    {
                        if (CalcData.ts_Timebuffer.TotalSeconds <= 0)
                            label_timebuffer.Content = $"Zeitpuffer: 0 Min.";
                        else
                            label_timebuffer.Content = $"Zeitpuffer: {ConverterHelper.ConvertTimespanToCustomString(CalcData.ts_Timebuffer)}";
                    }

                    //label beacon
                    if (data.ets2.truck.lightsBeaconOn)
                        label_beaconState.Content = "eingeschaltet";
                    else
                        label_beaconState.Content = "ausgeschaltet";
                    //label jobInfo
                    if (data.ets2.job.cargo.id != string.Empty)
                    {
                        label_jobInfoFreight.Content = data.ets2.job.cargo.name;
                        label_jobInfoMass.Content = $"{Unit.jobInfoMassC.ToString("n1")} {Unit.UMass}";
                        label_jobInfoIncome.Content = $"{data.ets2.job.income.ToString("c0", Unit.UCultureInfo)}  ({Math.Round(Convert.ToDecimal(data.ets2.job.income / Unit.plannedDistanceKM), 2)} {Unit.UMoneyDistance})";
                    }
                    else
                    {
                        label_jobInfoFreight.Content = "Leerfahrt";
                        label_jobInfoMass.Content = $"0 {Unit.UMass}";
                        label_jobInfoIncome.Content = $"{0.ToString("c0", Unit.UCultureInfo)}  (0 {Unit.UMoneyDistance})";
                    }
                    //label fuel
                    label_averageFuelConsumption.Content = $"{Unit.fuelAverageConsumptionC.ToString("n2")} {Unit.UAverageFuelConsumption}";
                    if (!(Unit.fuelCurrent / Unit.fuelCapacity).Equals(double.NaN))
                        progressBar_fuel.Value = Unit.fuelCurrent / Unit.fuelCapacity * 100;
                    else
                        progressBar_fuel.Value = 0;
                    label_progressBar_fuelText.Content = $"{Math.Round(Unit.fuelCurrent, 0)} {Unit.UFluid}  /  {Math.Round(Unit.fuelCapacity),0} {Unit.UFluid}  ({Math.Round(Unit.fuelRange, 0)} {Unit.UDistance})";
                    if (data.ets2.truck.fuelWarningOn)
                        progressBar_fuel.Foreground = new SolidColorBrush(Colors.Brown);
                    else
                        progressBar_fuel.Foreground = new SolidColorBrush(Colors.LimeGreen);
                    //label nextRestStop
                    TimeSpan ts_nextPauseTime = TimeSpan.FromSeconds(Math.Abs(data.ets2.game.nextRestStopTime));
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
                    progressBar_damage.Value = data.ets2.job.cargo.totalDamage * 100;
                    label_progressBar_damageText.Content = data.ets2.job.cargo.totalDamage.ToString("p1");
                    //label timeScale
                    label_timeScale.Content = $"Zeitskalierung: {data.ets2.game.scale}";
                    //contractStatus
                    if (data.ets2.job.cargo.id != string.Empty)
                    {
                        accessText_contractStatus.Text = "Auftrag aktiv";
                        label_contractStatus.Background = new SolidColorBrush(Colors.LimeGreen);
                    }
                    else
                    {
                        accessText_contractStatus.Text = "Keinen aktiven Auftrag";
                        label_contractStatus.Background = new SolidColorBrush(Colors.Brown);
                    }
                }
            }

            /*if (telemetryIsOnline && data.ets2.game.connected && Unit.navigationDistanceC > 5)
            {
                menuItem_contractLoad.IsEnabled = true;
                menuItem_contractSave.IsEnabled = true;
            }
            else
            {
                menuItem_contractLoad.IsEnabled = false;
                menuItem_contractSave.IsEnabled = false;
            }*/
        }

        private void ResetSDK()
        {
            //ContractHelper.StopBackupper();
            ContractHelper.contractOnStartLoaded = false;
            ContractHelper.ResetValues();
            CalcData.ResetValues(true);
            CalcData.IsAllowedToUpdate = true;
        }

        private void SetCurrentArrivalLabelColor(System.Windows.Controls.Label[] labels, Color color)
        {
            SolidColorBrush colBrush = new SolidColorBrush(color);
            label_currentArrivalText.Background = colBrush;
            label_dt_currentArrival.Background = colBrush;
            label_ts_currentArrival.Background = colBrush;
        }


        private void SetTSTServerMessage()
        {
            TSTServerJson tstServerJson = new TSTServerJson();
            tstServerJson.connectionStatusText = accessText_connectionStatus.Text;
            tstServerJson.connectionStatusBrush = label_connectionStatus.Background;
            tstServerJson.contractStatusText = accessText_contractStatus.Text;
            tstServerJson.contractStatusBrush = label_contractStatus.Background;
            tstServerJson.shiftStatusText = accessText_shiftStatus.Text;
            tstServerJson.shiftStatusBrush = label_shiftStatus.Background;
            tstServerJson.currentArrival_dtText = label_dt_currentArrival.Content.ToString();
            tstServerJson.currentArrival_tsText = label_ts_currentArrival.Content.ToString();
            tstServerJson.currentArrivalBrush = label_dt_currentArrival.Background;
            tstServerJson.currentBestArrival_dtText = label_dt_currentBestArrival.Content.ToString();
            tstServerJson.currentBestArrival_tsText = label_ts_currentBestArrival.Content.ToString();
            tstServerJson.bestArrival_dtText = label_dt_bestArrival.Content.ToString();
            tstServerJson.bestArrival_tsText = label_ts_bestArrival.Content.ToString();
            tstServerJson.nextPauseTimeText = label_nextRestStop.Content.ToString();
            tstServerJson.nextPauseTimeBrush = label_nextRestStop.Foreground;
            tstServerJson.remainingTimeText = label_remainingDeliveryTime.Content.ToString();
            tstServerJson.remainingTimeBrush = label_remainingDeliveryTime.Foreground;
            tstServerJson.jobInfo_FreightText = label_jobInfoFreight.Content.ToString();
            tstServerJson.jobInfo_MassText = label_jobInfoMass.Content.ToString();
            tstServerJson.jobInfo_IncomeText = label_jobInfoIncome.Content.ToString();
            tstServerJson.sourceText = $"{label_citySource.Content.ToString()}\n{label_companySource.Content.ToString()}";
            tstServerJson.destinationText = $"{label_cityDestination.Content.ToString()}\n{label_companyDestination.Content.ToString()}";
            tstServerJson.progressBarPercentage = label_drivenDistanceProgress.Content.ToString();
            tstServerJson.timebufferText = label_timebuffer.Content.ToString();
            tstServerJson.timebufferBrush = label_timebuffer.Background;
            tstServerJson.remainingDistanceText = label_remainingDistance.Content.ToString();
            tstServerJson.timescaleText = label_timeScale.Content.ToString();
            tstServerJson.pb_distanceProgress = progressBar_distance.Value;
            tstServerJson.pb_distanceText = label_progressBar_distanceText.Content.ToString();
            tstServerJson.pb_damageProgress = progressBar_damage.Value;
            tstServerJson.pb_damageText = label_progressBar_damageText.Content.ToString();

            tstServerJson.hasShift = ShiftSchedule.HasShift;
            tstServerJson.nextShiftEvent = label_nextShiftEvent.Content.ToString();
            tstServerJson.nextShiftPause = label_nextShiftPause.Content.ToString();
            tstServerJson.shiftTimeLeft = label_shiftTimeLeft.Content.ToString();

            TSTServer.SetMessage(tstServerJson);
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
                DateTime timestart = DateTime.MinValue;
                DateTime timeend = DateTime.MinValue;
                try { timestart = DateTime.MinValue.Add(TimeSpan.FromSeconds(Convert.ToDouble(tfmDJ_data.result.slot.timestart) + 7200)); } catch { timestart = DateTime.MinValue; } // +7200: From UTC to UTC+2
                try { timeend = DateTime.MinValue.Add(TimeSpan.FromSeconds(Convert.ToDouble(tfmDJ_data.result.slot.timeend) + 7200)); } catch { timeend = DateTime.MinValue; } // +7200: From UTC to UTC+
                if (timestart == timeend)
                    label_tfmDuration.Content = string.Empty;
                else
                    label_tfmDuration.Content = $"{timestart.ToString("HH:mm")} - {timeend.ToString("HH:mm")} Uhr";
                lastTFMPicturePath = tfmSong_data.art.ToString();
            }
            else
            {
                canvas_tfmSongPicture.Background = new SolidColorBrush(Colors.Transparent);
                label_tfmSongTitle.Content = string.Empty;
                label_tfmSongAuthor.Content = string.Empty;
                label_tfmDJName.Content = string.Empty;
                label_tfmDuration.Content = string.Empty;
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

                label_shiftCount.Content = $"Schicht: {ShiftSchedule.ShiftCurrentCount} / {ShiftSchedule.ShiftTotalCount}";
                label_nextShiftEvent.Content = $"Nächstes Schichtereignis: {ReturnNextShiftEventString(ShiftSchedule.NextShiftEvent)}";
                if (ShiftSchedule.NextShiftEvent.Item1 < DateTime.Now.AddMinutes(30))
                {
                    if (ShiftSchedule.NextShiftEvent.Item1 < DateTime.Now.AddMinutes(5))
                        label_nextShiftEvent.Background = new SolidColorBrush(Colors.Brown);
                    else
                        label_nextShiftEvent.Background = new SolidColorBrush(Colors.Goldenrod);
                }
                else
                {
                    label_nextShiftEvent.Background = new SolidColorBrush(Colors.Transparent);
                }

                if (ShiftSchedule.CurrentShiftIsActive)
                {
                    TimeSpan ts_shiftTimeLeft = ShiftSchedule.NextShiftEnd - DateTime.Now;
                    label_shiftTimeLeft.Content = $"Übrige Schichtlänge: {ConverterHelper.ConvertTimespanToCustomString(ts_shiftTimeLeft)}";
                    label_currentShift.Content = $"Derzeitige Schicht: {ShiftSchedule.CurrentShiftStartEnd[0].ToString("HH:mm")} Uhr,  {ShiftSchedule.CurrentShiftStartEnd[0].ToShortDateString()}   -   {ShiftSchedule.CurrentShiftStartEnd[1].ToString("HH:mm")} Uhr,  {ShiftSchedule.CurrentShiftStartEnd[1].ToShortDateString()}";

                    if (ShiftSchedule.CurrentShiftHasPause)
                    {
                        if (ShiftSchedule.ShiftPaused)
                        {
                            TimeSpan ts_nextShiftPauseEnd = ShiftSchedule.NextShiftPauseEnd - DateTime.Now;
                            label_nextShiftPause.Content = $"Pausenende in: {ConverterHelper.ConvertTimespanToCustomString(ts_nextShiftPauseEnd)}";
                        } 
                        else
                        {
                            TimeSpan ts_nextShiftPauseStart = ShiftSchedule.NextShiftPauseStart - DateTime.Now;
                            label_nextShiftPause.Content = $"Nächste Pause in: {ConverterHelper.ConvertTimespanToCustomString(ts_nextShiftPauseStart)}";
                        }
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

        #region "InternetDownloads"
        private async Task UpdateTelemetry()
        {
            try
            {
                HttpClient client = new HttpClient();
                Stream stream = await client.GetStreamAsync("http://127.0.0.1:25552/");

                StreamReader sr = new StreamReader(stream);
                string jsonTelemetry = sr.ReadToEnd();
                sr.Close();

                data = JsonConvert.DeserializeObject<Rootobject_Telemetry>(jsonTelemetry);
                telemetryIsOnline = true;
            }
            catch
            {
                telemetryIsOnline = false;
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
                if (MessageBox.Show("Sie haben zurzeit eine Schicht geladen, möchten Sie diese ersetzen?", "Schicht schon aktiv!", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
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
                if (MessageBox.Show("Wenn Sie fortfahren, werden nicht gespeicherte Daten gelöscht. Möchten Sie den aktuellen Schichtplan löschen?", "Soll der Schichtplan gelöscht werden?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                    return;
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
                if (MessageBox.Show("Sie haben bereits einen Schichtplan geladen. Wenn Sie den Schichtplan nicht gespeichert haben, wird er dauerhaft gelöscht.", "Soll der derzeitige Schichtplan gelöscht werden?", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.No)
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
            label_nextShiftEvent.Background = new SolidColorBrush(Colors.Transparent);
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
                                doubleUpDown_distanceCalculatorTime1.Value = (double)Math.Round((double)((Convert.ToDouble(integerUpDown_distanceCalculatorDistance.Value) / timeScaleConstant) / integerUpDown_distanceCalculatorAverageSpeed.Value), 2);
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
                            doubleUpDown_distanceCalculatorTime3.Value = (double)Math.Round((double)((Convert.ToDouble(integerUpDown_distanceCalculatorDistance.Value) / 3) / integerUpDown_distanceCalculatorAverageSpeed.Value), 2);
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

        /*private void menuItem_contractSave_Click(object sender, RoutedEventArgs e)
        {
            ContractHelper.TryManualSave();
        }
        private void menuItem_contractLoad_Click(object sender, RoutedEventArgs e)
        {
            ContractHelper.ManualLoad();
        }*/
        private void menuItem_resetAverageSpeed_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Sind Sie sich sicher, dass Sie die Durchschnittsgeschwindigkeit zurücksetzen möchten?", "Möchten Sie fortfahren?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                CalcData.ResetCurrentAverageSpeed();
        }
        private void menuItem_resetBestArrival_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Sind Sie sich sicher, dass Sie die geplante Fahrzeit zurücksetzen möchten?", "Möchten Sie fortfahren?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                CalcData.ResetBestArrival();
        }
        #endregion

        #region "MainWindow ImportantEvents"
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Todo: add message box warning if trying to leave and manual saving is on. (for that add [static bool SettingsSaved] and then you can set label_contractStatus color to Goldenrod as well)
            // ^- NOT IMPLEMENTED YET -^
        }
        #endregion
    }
}