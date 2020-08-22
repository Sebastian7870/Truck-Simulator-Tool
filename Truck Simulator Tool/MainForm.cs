using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Truck_Simulator_Tool
{
    public partial class MainForm : Form
    {
        // Variables Start

        HttpWebServer server = new HttpWebServer();
        bool bTelemetryOnline = false;
        bool bTruckersfmOnline = false;
        Rootobject TelemetryData = new Rootobject();
        Rootobject_TFMdj TruckersfmdjData = new Rootobject_TFMdj();
        Rootobject_TFMsong TruckersfmsongData = new Rootobject_TFMsong();
        int timercounter = 0;
        string situation = "None";
        double currentaveragespeed = 0;
        double bestcurrentaveragespeed = 0;
        double speedsummary = 0;
        double distancesummary = 0;
        double drivendistance = 0;
        bool bestarrivalset = false;
        List<Workshift> listWorkshifts = new List<Workshift>();
        bool ShiftActive = false;
        bool scheduleLoaded = false;
        DateTime dt_bestarrival = DateTime.Now;
        DateTime dt_currentarrival = DateTime.Now;
        TimeSpan ts_bestarrival = new TimeSpan();
        double TimeScaleConstant = 19;
        readonly string SoftwarePath = Application.StartupPath;
        SavedContract savedcontract = new SavedContract();
        bool StartApplicationContractLoaded = false;
        float LastKnownEstimatedDistance = 0f;
        Settings settings = new Settings();
        bool ContractSaved = false;
        readonly DateTimeFormatInfo dateTimeFormatInfoDE = CultureInfo.GetCultureInfo("de-DE").DateTimeFormat;
        readonly DateTimeFormatInfo dateTimeFormatInfoUSA = CultureInfo.GetCultureInfo("en-US").DateTimeFormat;


        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr H);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();


        // Variables End

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadSettingsOrCreate();

            // Check for SchedulePlans and ContractInfo Files
            if (Directory.Exists(SoftwarePath + @"\work shifts") && Directory.Exists(SoftwarePath + @"\contracts"))
            { // Load Data if available

            }
            else
            {
                if (MessageBox.Show(String.Format("Es wurden fehlende Ordner im Installationspfad ({0}) gefunden. Installieren Sie die Software neu, um die Speicherung von Arbeitsschichten und von Auftragsdaten zu ermöglichen. Möchten Sie die Anwendung schließen?", SoftwarePath), "Warnung!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    Application.Exit();
                }
            }

            // Schedulepicker Time
            dateTimePicker_schedule.Value = DateTime.Now.AddDays(1);
            //DateTime Now
            label14_datetimetime.Text = DateTime.Now.ToString("HH:mm");
            label_datetimenowseconds.Text = DateTime.Now.ToString("ss");
            label15_datetimedate.Text = DateTimeFormatInfo.CurrentInfo.GetDayName(DateTime.Now.DayOfWeek) + "\n" + DateTime.Now.ToShortDateString();

            timer1_calculate.Start();
            timer2_calculateMinute.Start();
            Timer1_calculate_MethodTick();
            Timer2_calculate_MethodTick();

            // Distance Calculator
            radioButton_extended.Checked = false;
            radioButton_standart.Checked = true;
            label_Calculatortime1.Text = "Fahrzeit:";
            label_Calculatortime2.Visible = false;
            label_Calculatortime3.Visible = false;
            numericUpDown_time2.Visible = false;
            numericUpDown_time3.Visible = false;
            label_CalculatortimeH2.Visible = false;
            label_CalculatortimeH3.Visible = false;
            numericUpDown_km.ReadOnly = true;

            // Check for SchedulePlans and ContractInfo Files
            if (Directory.Exists(SoftwarePath + @"\work shifts") && Directory.Exists(SoftwarePath + @"\contracts"))
            { // Load Data if available
                string[] files = Directory.GetFiles(SoftwarePath + @"\contracts");
                foreach (string file in files)
                {
                    FileInfo fileinfo = new FileInfo(file);
                    if (fileinfo.LastWriteTime < DateTime.Now.AddMonths(-3))
                    {
                        fileinfo.Delete();
                    }
                }
            }

            //Server
            server.Start();// -- Check for Entry
            server.Stop();// --
            if (settings.ServerDefaultStart == true)
            {
                StartServer();
            }
            SetServerLabels();
        }


        // Load Settings (or create)
        public void LoadSettingsOrCreate()
        {
            // Check for settings file
            if (File.Exists(SoftwarePath + @"\config.json"))
            { // Load Data if available
                settings = (JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SoftwarePath + @"\config.json")));
            }
            else
            {
                try
                {
                    settings.AutoSaveActive = true;
                    settings.AntiKickDefaultOn = true;
                    settings.ManualTimescaleValue = 19;
                    settings.BackgroundImageFilePath = "";
                    settings.ServerDefaultStart = true;

                    string sJson = JsonConvert.SerializeObject(settings);
                    File.WriteAllText((String.Format(SoftwarePath + @"\config.json")), sJson);
                }
                catch
                {
                    MessageBox.Show("Schwerwiegender Fehler gefunden! Bitte Autor kontaktieren.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            TimeScaleConstant = Convert.ToDouble(settings.ManualTimescaleValue);
            if (settings.AntiKickDefaultOn == true)
            {
                antikickToolStripMenuItem1.Checked = true;
                timer3_antikick.Start();
            }
            else
            {
                antikickToolStripMenuItem1.Checked = false;
            }

            // Background Image settings
            if (settings.BackgroundImageFilePath != "")
            {
                try
                {// Set Image
                    Image img = Image.FromFile(settings.BackgroundImageFilePath);
                    panel_BackgroundImage.BackgroundImage = img;

                    // Panel transparency on
                    panel_ContractdistanceData.BackColor = Color.FromArgb(175, Color.Gainsboro);
                    panel_TruckerFMBox.BackColor = Color.FromArgb(175, Color.Gainsboro);
                    panel_Shiftbox.BackColor = Color.FromArgb(175, Color.Gainsboro);
                    panel_Calculator.BackColor = Color.FromArgb(175, Color.Gainsboro);
                    panel_Vehicleinfo.BackColor = Color.FromArgb(175, Color.Gainsboro);
                    panel_Jobinfo.BackColor = Color.FromArgb(175, Color.Gainsboro);
                }
                catch
                {
                    MessageBox.Show("Das Hintergrundbild hat ein falsches Format. Die Standarteinstellung wird als Hintergrund verwendet.", "Falsches Format!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    try
                    {
                        settings.BackgroundImageFilePath = "";
                        string sJson = JsonConvert.SerializeObject(settings);
                        File.WriteAllText((String.Format(SoftwarePath + @"\config.json")), sJson);

                        // Panel transparency off
                        panel_ContractdistanceData.BackColor = Color.FromArgb(255, Color.LightGray);
                        panel_TruckerFMBox.BackColor = Color.FromArgb(255, Color.Gainsboro);
                        panel_Shiftbox.BackColor = Color.FromArgb(255, Color.Gainsboro);
                        panel_Calculator.BackColor = Color.FromArgb(255, Color.Gainsboro);
                        panel_Vehicleinfo.BackColor = Color.FromArgb(255, Color.Gainsboro);
                        panel_Jobinfo.BackColor = Color.FromArgb(255, Color.DarkGray);
                    }
                    catch
                    {
                        MessageBox.Show("Schwerwiegender Fehler gefunden! Bitte Autor kontaktieren.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                panel_BackgroundImage.BackgroundImage = null;

                // Panel transparency off
                panel_ContractdistanceData.BackColor = Color.FromArgb(255, Color.LightGray);
                panel_TruckerFMBox.BackColor = Color.FromArgb(255, Color.Gainsboro);
                panel_Shiftbox.BackColor = Color.FromArgb(255, Color.Gainsboro);
                panel_Calculator.BackColor = Color.FromArgb(255, Color.Gainsboro);
                panel_Vehicleinfo.BackColor = Color.FromArgb(255, Color.Gainsboro);
                panel_Jobinfo.BackColor = Color.FromArgb(255, Color.DarkGray);
            }
        }

        async Task UpdateTelemetry()
        {// Update Telemetry
            try
            {
                HttpClient client = new HttpClient();
                Stream stream = await client.GetStreamAsync("http://192.168.178.22:25552/");

                StreamReader sr = new StreamReader(stream);
                string sTelemetryJson = sr.ReadToEnd();
                sr.Close();

                TelemetryData = JsonConvert.DeserializeObject<Rootobject>(sTelemetryJson);
                bTelemetryOnline = true;
            }
            catch
            {
                bTelemetryOnline = false;
            }

        }
        async Task UpdateTruckersFM()
        {// Update TruckersFM
            try
            {
                HttpClient client = new HttpClient();
                Stream stream = await client.GetStreamAsync("https://panel.truckers.fm/api/current");
                Stream stream1 = await client.GetStreamAsync("https://panel.truckers.fm/api/song/current");

                StreamReader sr = new StreamReader(stream);
                string sTruckersfmdjJson = sr.ReadToEnd();
                sr.Close();
                StreamReader sr1 = new StreamReader(stream1);
                string sTruckersfmsongJson = sr1.ReadToEnd();
                sr1.Close();

                TruckersfmdjData = JsonConvert.DeserializeObject<Rootobject_TFMdj>(sTruckersfmdjJson);
                TruckersfmsongData = JsonConvert.DeserializeObject<Rootobject_TFMsong>(sTruckersfmsongJson);
                bTruckersfmOnline = true;
            }
            catch
            {
                bTruckersfmOnline = false;
            }

        }



        private void Timer1_calculate_Tick(object sender, EventArgs e)
        {
            Timer1_calculate_MethodTick();
        }

        private async void Timer1_calculate_MethodTick()
        {
            await UpdateTelemetry();
            await UpdateTruckersFM();
            label14_datetimetime.Text = DateTime.Now.ToString("HH:mm");
            label_datetimenowseconds.Text = DateTime.Now.ToString("ss");
            label15_datetimedate.Text = DateTimeFormatInfo.CurrentInfo.GetDayName(DateTime.Now.DayOfWeek) + "\n" + DateTime.Now.ToShortDateString();
            dateTimePicker_schedule.MinDate = DateTime.Now.AddDays((-1) * ((Convert.ToDouble(numericUpDown_durationSchedule.Value) - 0.5)));

            SetServerLabels();

            if (bTruckersfmOnline == true)
            {// Get Picture and set TFM data from class.
                try
                {
                    HttpClient client = new HttpClient();
                    Stream stream = await client.GetStreamAsync(TruckersfmsongData.art.ToString());
                    pictureBox_TruckersfmSong.BackgroundImage = System.Drawing.Image.FromStream(stream);

                    label_TFMsongname.Text = TruckersfmsongData.title;
                    label_TFMsongartist.Text = TruckersfmsongData.artist;
                    label_TFMdjname.Text = "DJ " + TruckersfmdjData.result.dj.name;
                    label_TFMdjTimeleft.Text = TimeSpanConvertToAvailableValuesOnly(TimeSpan.FromSeconds((Convert.ToDouble(TruckersfmdjData.result.slot.timeend) - Convert.ToDouble(TruckersfmdjData.result.slot.timestart))));
                }
                catch
                {// --> TFM or PC offline
                    pictureBox_TruckersfmSong.BackgroundImage = null;
                    label_TFMsongname.Text = "-";
                    label_TFMsongartist.Text = "-";
                    label_TFMdjname.Text = "-";
                    label_TFMdjTimeleft.Text = "-";
                }
            }
            else
            {
                pictureBox_TruckersfmSong.BackgroundImage = null;
                label_TFMsongname.Text = "-";
                label_TFMsongartist.Text = "-";
                label_TFMdjname.Text = "-";
                label_TFMdjTimeleft.Text = "-";
            }

            if (bTelemetryOnline == true)
            {// Telemetry online
                if (TelemetryData.ets2.game.connected == true)
                {// Game connected

                    // Average Variables Calcuations
                    currentaveragespeed = speedsummary / timercounter;
                    if (TelemetryData.ets2.truck.speed > 5 && TelemetryData.ets2.game.paused == false)
                    {// Current Average Speed

                        if (TelemetryData.ets2.game.gameID == "eut2")
                        {// ETS2
                            timercounter += 1;
                            speedsummary += TelemetryData.ets2.truck.speed;
                            currentaveragespeed = speedsummary / timercounter;
                        }
                        else
                        {// ATS
                            timercounter += 1;
                            speedsummary += TelemetryData.ets2.truck.speed / 1.609344;
                            currentaveragespeed = speedsummary / timercounter;
                        }
                    }

                    if (TelemetryData.ets2.game.gameID == "eut2")
                    {// ETS2
                        if (TelemetryData.ets2.truck.navigationEstimatedDistance > 0)
                        {// best current average speed
                            bestcurrentaveragespeed = (TelemetryData.ets2.truck.navigationEstimatedDistance / 1000) / (Convert.ToDouble(TelemetryData.ets2.truck.navigationEstimatedTime) / 3600);
                        }
                        else if (TelemetryData.ets2.truck.navigationEstimatedDistance == 0)
                        {
                            bestarrivalset = false;
                            bestcurrentaveragespeed = 0;

                            panel_ArrivalinfoTop.BackColor = Color.Brown;

                            label_currentarrival.Text = "Ankunft ca.:      00:00 Uhr";
                            label_currentarrival2.Text = "(0 Min.)";

                            label_currentbestarrival.Text = "00:00 Uhr";
                            label_currentbestarrival2.Text = "(0 Min.)";


                            label_bestarrival.Text = "00:00 Uhr";
                            label_bestarrival2.Text = "(0 Min.)";
                        }
                    }
                    else
                    {// ATS
                        if (TelemetryData.ets2.truck.navigationEstimatedDistance > 0)
                        {// best current average speed
                            bestcurrentaveragespeed = ((TelemetryData.ets2.truck.navigationEstimatedDistance / 1000) / 1.609344) / (Convert.ToDouble(TelemetryData.ets2.truck.navigationEstimatedTime) / 3600);
                        }
                        else if (TelemetryData.ets2.truck.navigationEstimatedDistance == 0)
                        {
                            bestarrivalset = false;
                            bestcurrentaveragespeed = 0;

                            panel_ArrivalinfoTop.BackColor = Color.Brown;

                            label_currentarrival.Text = "Ankunft ca.:      00:00 Uhr";
                            label_currentarrival2.Text = "(0 Min.)";

                            label_currentbestarrival.Text = "00:00 Uhr";
                            label_currentbestarrival2.Text = "(0 Min.)";


                            label_bestarrival.Text = "00:00 Uhr";
                            label_bestarrival2.Text = "(0 Min.)";
                        }
                    }

                    if (bestcurrentaveragespeed > 0)
                    {
                        // Best Current Average Speed
                        _ = TimeSpan.FromSeconds(0);
                        TimeSpan ts_bestcurrentarrival;
                        if (TelemetryData.ets2.game.gameID == "eut2")
                        { // ETS2
                            DateTime dt_bestcurrentarrival = DateTime.Now.AddSeconds((((TelemetryData.ets2.truck.navigationEstimatedDistance / 1000) / bestcurrentaveragespeed) / TimeScaleConstant) * 3600);
                            ts_bestcurrentarrival = dt_bestcurrentarrival.Subtract(DateTime.Now);
                            label_currentbestarrival.Text = String.Format("{0} Uhr", dt_bestcurrentarrival.ToString("HH:mm"));
                            label_currentbestarrival2.Text = String.Format("({0})", TimeSpanConvertToAvailableValuesOnly(ts_bestcurrentarrival));
                        }
                        else
                        { // ATS
                            DateTime dt_bestcurrentarrival = DateTime.Now.AddSeconds(((((TelemetryData.ets2.truck.navigationEstimatedDistance / 1000) / 1.609344) / bestcurrentaveragespeed) / TimeScaleConstant) * 3600);
                            ts_bestcurrentarrival = dt_bestcurrentarrival.Subtract(DateTime.Now);
                            label_currentbestarrival.Text = String.Format("{0} Uhr", dt_bestcurrentarrival.ToString("HH:mm"));
                            label_currentbestarrival2.Text = String.Format("({0})", TimeSpanConvertToAvailableValuesOnly(ts_bestcurrentarrival));
                        }

                        // Best Arrival
                        if (bestarrivalset == false)
                        {

                            if (TelemetryData.ets2.game.gameID == "eut2")
                            {// ETS2
                                ts_bestarrival = TimeSpan.FromSeconds((((TelemetryData.ets2.truck.navigationEstimatedDistance / 1000) / bestcurrentaveragespeed) / TimeScaleConstant) * 3600);
                                bestarrivalset = true;
                            }
                            else
                            {// ATS
                                ts_bestarrival = TimeSpan.FromSeconds(((((TelemetryData.ets2.truck.navigationEstimatedDistance / 1000) / 1.609344) / bestcurrentaveragespeed) / TimeScaleConstant) * 3600);
                                bestarrivalset = true;
                            }

                            dt_bestarrival = DateTime.Now.Add(ts_bestarrival);
                            label_bestarrival.Text = dt_bestarrival.ToString("HH:mm") + " Uhr";
                        }

                        ts_bestarrival = dt_bestarrival - DateTime.Now;

                        if (ts_bestarrival.TotalSeconds > 0)
                        {
                            label_bestarrival2.Text = String.Format("(-{0})", TimeSpanConvertToAvailableValuesOnly(ts_bestarrival));
                        }
                        else
                        {
                            label_bestarrival2.Text = String.Format("(+{0})", TimeSpanConvertToAvailableValuesOnly(TimeSpan.FromSeconds(ts_bestarrival.TotalSeconds * (-1))));
                        }


                        // Current Average Speed
                        if (currentaveragespeed > 0)
                        {
                            if (TelemetryData.ets2.game.gameID == "eut2")
                            {// ETS2
                                dt_currentarrival = DateTime.Now.AddSeconds(((((TelemetryData.ets2.truck.navigationEstimatedDistance / 1000) / currentaveragespeed) / TimeScaleConstant) * 3600));
                            }
                            else
                            {// ATS
                                dt_currentarrival = DateTime.Now.AddSeconds((((((TelemetryData.ets2.truck.navigationEstimatedDistance / 1000) / 1.609344) / currentaveragespeed) / TimeScaleConstant) * 3600));
                            }
                            TimeSpan ts_currentarrival = dt_currentarrival.Subtract(DateTime.Now);

                            if (ts_currentarrival.TotalMinutes - ts_bestcurrentarrival.TotalMinutes > 60)
                            {// current arrival (color)
                                panel_ArrivalinfoTop.BackColor = Color.Brown;
                            }
                            else if (ts_currentarrival.TotalMinutes - ts_bestcurrentarrival.TotalMinutes > 30 && ts_currentarrival.TotalMinutes - ts_bestcurrentarrival.TotalMinutes < 60)
                            {
                                panel_ArrivalinfoTop.BackColor = Color.Goldenrod;
                            }
                            else
                            {
                                panel_ArrivalinfoTop.BackColor = Color.LimeGreen;
                            }

                            label_currentarrival.Text = String.Format("Ankunft ca.:      {0} Uhr", dt_currentarrival.ToString("HH:mm"));
                            label_currentarrival2.Text = String.Format("({0})", TimeSpanConvertToAvailableValuesOnly(ts_currentarrival));
                        }

                    }// End average Calculations

                    if (TelemetryData.ets2.job.cargo.id != "")
                    {// Contract-Only  
                        if (situation != "Contract")
                        {
                            timercounter = 0;
                            speedsummary = 0;
                            currentaveragespeed = 0;

                            distancesummary = 0;
                            drivendistance = 0;

                            bestarrivalset = false;
                            ContractSaved = false;
                        }
                        situation = "Contract";
                    }
                    else if (TelemetryData.ets2.job.cargo.id == "")
                    {
                        // DestinationOrFreeDrive-Only
                        if (situation != "DestinationOrFreeDrive")
                        {
                            timercounter = 0;
                            speedsummary = 0;
                            currentaveragespeed = 0;
                            bestcurrentaveragespeed = 0;

                            distancesummary = 0;
                            drivendistance = 0;
                            bestarrivalset = false;

                            try
                            {
                                if (File.Exists(String.Format(SoftwarePath + @"\contracts\{0}_AutoSaveContract_{1} - {2}___id-{3}.json", savedcontract.GameId, savedcontract.SourceCity, savedcontract.DestinationCity, (savedcontract.Income + savedcontract.TotalMass))))
                                {
                                    File.Delete(String.Format(SoftwarePath + @"\contracts\{0}_AutoSaveContract_{1} - {2}___id-{3}.json", savedcontract.GameId, savedcontract.SourceCity, savedcontract.DestinationCity, (savedcontract.Income + savedcontract.TotalMass)));
                                }
                            }
                            catch
                            {

                            }

                        }
                        situation = "DestinationOrFreeDrive";

                    }


                    // Contract status label
                    if (TelemetryData.ets2.job.cargo.id != "")
                    {
                        if (ContractSaved == false)
                        {
                            label_contractstatus.BackColor = Color.Goldenrod;
                            label_contractstatus.Text = "Auftrag nicht gespeichert";
                        }
                        else if (ContractSaved == true)
                        {
                            label_contractstatus.BackColor = Color.LimeGreen;
                            label_contractstatus.Text = "Auftrag aktiv";
                        }
                    }
                    else if (TelemetryData.ets2.job.cargo.id == "")
                    {
                        label_contractstatus.BackColor = Color.Brown;
                        label_contractstatus.Text = "Keinen aktiven Auftrag";
                    }


                    // TimescaleConstant Calculator label
                    label_TimeScaleConstantCalculator.Text = "Zeitskalierung: " + Math.Round(TimeScaleConstant, 2);


                    // Pause label
                    if (TelemetryData.ets2.game.paused == false)
                    {
                        label_connectionstatus.Text = "Verbunden";
                        label_connectionstatus.BackColor = System.Drawing.Color.LimeGreen;
                    }
                    else if (TelemetryData.ets2.game.paused == true)
                    {
                        label_connectionstatus.Text = "Spiel pausiert";
                        label_connectionstatus.BackColor = System.Drawing.Color.Goldenrod;
                    }


                    //Ingametime Label
                    TimeSpan Ingametime = TimeSpan.FromSeconds(TelemetryData.ets2.game.gameTime);
                    DateTime dt_ingametime = new DateTime(1, 1, 1, 0, 0, 0).Add(Ingametime);
                    if (TelemetryData.ets2.game.gameID == "eut2")
                    {
                        label_ingametime.Text = String.Format("{0} {1}", dateTimeFormatInfoDE.GetShortestDayName(dt_ingametime.DayOfWeek), dt_ingametime.ToString("H:mm", CultureInfo.GetCultureInfo("de-DE")));
                    }
                    else
                    {
                        label_ingametime.Text = String.Format("{0} {1}", dateTimeFormatInfoUSA.GetShortestDayName(dt_ingametime.DayOfWeek), dt_ingametime.ToString("h:mm tt", CultureInfo.GetCultureInfo("en-US")));
                    }

                    // TimeScale
                    label2_timescale.Text = "Zeitskalierung: " + TelemetryData.ets2.game.scale.ToString();


                    // JobInfo
                    if (TelemetryData.ets2.game.gameID == "eut2")
                    {// ETS2
                        if (TelemetryData.ets2.job.cargo.id != "")
                        {
                            label5_jobinfo.Text = TelemetryData.ets2.job.cargo.name + "\n" + (TelemetryData.ets2.job.cargo.totalMass / 1000).ToString("n1") + " t\n" + TelemetryData.ets2.job.income.ToString("c0") + " (" + Math.Round(Convert.ToDecimal(TelemetryData.ets2.job.income) / Convert.ToDecimal(TelemetryData.ets2.job.cargo.plannedDistanceKM), 2) + " €/km)";
                            label10_sourcedata.Text = TelemetryData.ets2.job.sourceCity + "\n" + TelemetryData.ets2.job.sourceCompany;
                            label11_destinationdata.Text = TelemetryData.ets2.job.destinationCity + "\n" + TelemetryData.ets2.job.destinationCompany;
                        }
                        else
                        {
                            label5_jobinfo.Text = "Leerfahrt\n0 t\n0 € (0 €/ km)";
                            label10_sourcedata.Text = "";
                            label11_destinationdata.Text = "";
                        }
                    }
                    else
                    {// ATS
                        if (TelemetryData.ets2.job.cargo.id != "")
                        {
                            label5_jobinfo.Text = TelemetryData.ets2.job.cargo.name + "\n" + (TelemetryData.ets2.job.cargo.totalMass / 0.453595347).ToString("n0") + " lb\n" + TelemetryData.ets2.job.income.ToString("c0", new CultureInfo("en-US")) + " (" + Math.Round(Convert.ToDecimal(TelemetryData.ets2.job.income) / Convert.ToDecimal(TelemetryData.ets2.job.cargo.plannedDistanceKM / 1.609344), 2) + " $/mi)";
                            label10_sourcedata.Text = TelemetryData.ets2.job.sourceCity + "\n" + TelemetryData.ets2.job.sourceCompany;
                            label11_destinationdata.Text = TelemetryData.ets2.job.destinationCity + "\n" + TelemetryData.ets2.job.destinationCompany;
                        }
                        else
                        {
                            label5_jobinfo.Text = "Leerfahrt\n0 lb\n0 $ (0 $/mi)";
                            label10_sourcedata.Text = "";
                            label11_destinationdata.Text = "";
                        }
                    }


                    // next pause time (color)
                    TimeSpan ts_nextpausetime = TimeSpan.FromSeconds(TelemetryData.ets2.game.nextRestStopTime);
                    if (ts_nextpausetime.TotalSeconds > 0)
                    {
                        if (ts_nextpausetime.TotalHours < 5)
                        {
                            label8_nextpausetime.ForeColor = Color.Goldenrod;
                        }
                        else
                        {
                            label8_nextpausetime.ForeColor = Color.LimeGreen;
                        }
                    }
                    else
                    {
                        label8_nextpausetime.ForeColor = Color.Brown;
                    }
                    label8_nextpausetime.Text = "Pause in: " + TimeSpanConvertToAvailableValuesOnly(ts_nextpausetime);


                    // JobRemainingTime
                    if (TelemetryData.ets2.job.cargo.id != "")
                    {
                        TimeSpan ts_remainingtime = TimeSpan.FromSeconds(TelemetryData.ets2.job.remainingTime);
                        TimeSpan ts_estimatedtime = TimeSpan.FromSeconds(TelemetryData.ets2.truck.navigationEstimatedTime);
                        TimeSpan ts_timebuffer = ts_remainingtime - ts_estimatedtime;
                        if (ts_nextpausetime < ts_estimatedtime)
                        {
                            if (TelemetryData.ets2.game.gameID == "eut2")
                            {// ETS2
                                double d = Math.Ceiling((ts_estimatedtime.TotalSeconds - ts_nextpausetime.TotalSeconds) / (11 * 3600));
                                ts_timebuffer = ts_remainingtime - (ts_estimatedtime.Add(TimeSpan.FromHours(d * 9)));
                            }
                            else
                            {// ATS
                                double d = Math.Ceiling((ts_estimatedtime.TotalSeconds - ts_nextpausetime.TotalSeconds) / (14 * 3600));
                                ts_timebuffer = ts_remainingtime - (ts_estimatedtime.Add(TimeSpan.FromHours(d * 10)));
                            }
                        }

                        // Remaining time
                        if (ts_remainingtime.TotalSeconds > 0 && ts_remainingtime.TotalSeconds < 10000000)
                        {
                            if (ts_remainingtime.TotalHours < 1)
                            {
                                label8_nextpausetime.ForeColor = Color.Goldenrod;
                            }
                            else
                            {
                                label7_remainingtime.ForeColor = Color.LimeGreen;
                            }
                            label7_remainingtime.Text = "Restzeit: " + TimeSpanConvertToAvailableValuesOnly(ts_remainingtime);
                        }
                        else if (ts_remainingtime.TotalSeconds > 10000000)
                        {
                            label7_remainingtime.ForeColor = Color.CornflowerBlue;
                            label7_remainingtime.Text = "Restzeit: WoT";
                        }
                        else if (ts_remainingtime.TotalSeconds < 0)
                        {
                            label7_remainingtime.ForeColor = Color.Brown;
                            label7_remainingtime.Text = "0";
                        }

                        // time buffer
                        if (ts_timebuffer.TotalSeconds < 0)
                        {
                            label_Timebuffer.BackColor = Color.Brown;
                            label_Timebuffer.Text = "Zeitpuffer: 0 Min.";
                        }
                        else
                        {
                            if (ts_timebuffer.TotalHours < 5)
                            {
                                label_Timebuffer.BackColor = Color.Goldenrod;
                            }
                            else
                            {
                                label_Timebuffer.BackColor = Color.LimeGreen;
                            }

                            if (ts_remainingtime.TotalSeconds < 100000000)
                            {// Set timebuffer time (only if it is not external contract
                                label_Timebuffer.Text = "Zeitpuffer: " + TimeSpanConvertToAvailableValuesOnly(ts_timebuffer);
                            }
                            else
                            {
                                label_Timebuffer.BackColor = Color.CornflowerBlue;
                                label_Timebuffer.Text = "Zeitpuffer: WoT";
                            }

                        }

                    }
                    else
                    {
                        label_Timebuffer.BackColor = Color.Brown;
                        label7_remainingtime.ForeColor = Color.Brown;
                        label7_remainingtime.Text = "Restzeit: 0 Min.";
                        label_Timebuffer.Text = "Zeitpuffer: 0 Min.";
                    }


                    // vehicle info
                    string beaconStatus;
                    if (TelemetryData.ets2.truck.lightsBeaconOn == true)
                    {
                        beaconStatus = "eingeschaltet";
                    }
                    else
                    {
                        beaconStatus = "ausgeschaltet";
                    }
                    if (TelemetryData.ets2.game.gameID == "eut2")
                    {// ETS2
                        label_vehicleinformation.Text = String.Format("Rundumleuchte: {0}", beaconStatus);
                        label_vehicleinformation2.Text = String.Format("Ø Geschwindigkeit: {0} km/h", currentaveragespeed.ToString("n2"));
                        label_vehicleinformation3.Text = String.Format("Kraftstoffverbrauch: {0} l/100km", (TelemetryData.ets2.truck.fuelAverageConsumption * 100).ToString("n2"));
                    }
                    else
                    {// ATS
                        label_vehicleinformation.Text = String.Format("Rundumleuchte: {0}", beaconStatus);
                        label_vehicleinformation2.Text = String.Format("Ø Geschwindigkeit: {0} mph", (currentaveragespeed).ToString("n2"));
                        label_vehicleinformation3.Text = String.Format("Kraftstoffverbrauch: {0} mpg", ((TelemetryData.ets2.truck.fuelAverageConsumption * 100) / 6.43242746591568).ToString("n2"));
                    }



                    // ProgressBar Distance
                    double pb_distanceProgress;
                    if (TelemetryData.ets2.truck.navigationEstimatedDistance > 0)
                    {
                        if (TelemetryData.ets2.game.gameID == "eut2")
                        {// ETS2
                            if (TelemetryData.ets2.game.paused == false)
                            {
                                if (TelemetryData.ets2.truck.speed > 0.01)
                                {
                                    drivendistance += TelemetryData.ets2.truck.speed / 3600 * TelemetryData.ets2.game.scale;
                                }
                                else if (TelemetryData.ets2.truck.speed < -0.01)
                                {
                                    drivendistance += ((-1) * TelemetryData.ets2.truck.speed) / 3600 * TelemetryData.ets2.game.scale;
                                }
                                distancesummary = drivendistance + TelemetryData.ets2.truck.navigationEstimatedDistance / 1000;
                            }
                            if (TelemetryData.ets2.truck.navigationEstimatedDistance == 0 && TelemetryData.ets2.job.cargo.id == "")
                            {
                                distancesummary = 0;
                                drivendistance = 0;
                            }

                            // ProgressBar Create ETS2
                            pb_distanceProgress = drivendistance / distancesummary;
                            PictureBoxCustomProgressBar(pictureBox1_distance, Color.White, pb_distanceProgress * 100, String.Format("{0} km   /   {1} km", Math.Round(drivendistance, 0), Math.Round(distancesummary, 0)), "Microsoft Sans Serif", Brushes.LimeGreen);
                            label12_progresspercentage.Text = (pb_distanceProgress.ToString("p2"));
                            label13_remainingdistance.Text = "Noch " + Math.Round(TelemetryData.ets2.truck.navigationEstimatedDistance / 1000, 0).ToString() + " km";
                        }
                        else
                        {// ATS
                            if (TelemetryData.ets2.game.paused == false)
                            {
                                if (TelemetryData.ets2.truck.speed > 0.01)
                                {
                                    drivendistance += (TelemetryData.ets2.truck.speed / 1.609344) / 3600 * TelemetryData.ets2.game.scale;
                                }
                                else if (TelemetryData.ets2.truck.speed < -0.01)
                                {
                                    drivendistance += ((-1) * (TelemetryData.ets2.truck.speed / 1.609344) / 3600) * TelemetryData.ets2.game.scale;
                                }
                                distancesummary = drivendistance + (TelemetryData.ets2.truck.navigationEstimatedDistance / 1.609344) / 1000;
                            }
                            if (TelemetryData.ets2.truck.navigationEstimatedDistance == 0 && TelemetryData.ets2.job.cargo.id == "")
                            {
                                distancesummary = 0;
                                drivendistance = 0;
                            }

                            // ProgressBar Create ATS
                            pb_distanceProgress = drivendistance / distancesummary;
                            PictureBoxCustomProgressBar(pictureBox1_distance, Color.White, pb_distanceProgress * 100, String.Format("{0} mi   /   {1} mi", Math.Round(drivendistance, 0), Math.Round(distancesummary, 0)), "Microsoft Sans Serif", Brushes.LimeGreen);
                            label12_progresspercentage.Text = (pb_distanceProgress.ToString("p2"));
                            label13_remainingdistance.Text = "Noch " + Math.Round(((TelemetryData.ets2.truck.navigationEstimatedDistance / 1000) / 1.609344), 0).ToString() + " mi";

                        }

                    }
                    if (TelemetryData.ets2.truck.navigationEstimatedDistance == 0)
                    { // ProgressBarDistance reset
                        if (TelemetryData.ets2.game.gameID == "eut2")
                        {// ETS2
                            PictureBoxCustomProgressBar(pictureBox1_distance, Color.White, 0, String.Format("0 km   /   0 km"), "Microsoft Sans Serif", Brushes.LimeGreen);
                            label12_progresspercentage.Text = ("0,00 %");
                            label13_remainingdistance.Text = "Noch 0 km";
                        }
                        else
                        {// ATS
                            PictureBoxCustomProgressBar(pictureBox1_distance, Color.White, 0, String.Format("0 mi   /   0 mi"), "Microsoft Sans Serif", Brushes.LimeGreen);
                            label12_progresspercentage.Text = ("0,00 %");
                            label13_remainingdistance.Text = "Noch 0 mi";
                        }
                    }


                    // ProgressBar Damage
                    if (TelemetryData.ets2.job.cargo.totalDamage > 0)
                    {
                        PictureBoxCustomProgressBar(pictureBox2_cargodamage, Color.White, TelemetryData.ets2.job.cargo.totalDamage * 100, Math.Round(TelemetryData.ets2.job.cargo.totalDamage, 2).ToString("p0"), "Microsoft Sans Serif", Brushes.Brown);
                    }
                    else
                    {// ProgressBar Damage reset
                        PictureBoxCustomProgressBar(pictureBox2_cargodamage, Color.White, 0, "0,00 %", "Microsoft Sans Serif", Brushes.Brown);
                    }


                    // ProgressBar fuel
                    if (TelemetryData.ets2.game.gameID == "eut2")
                    {// ETS2
                        if (TelemetryData.ets2.truck.id != "")
                        {
                            if (TelemetryData.ets2.truck.fuelWarningOn)
                            {
                                PictureBoxCustomProgressBar(pictureBox3_fuel, Color.White, (TelemetryData.ets2.truck.fuel / TelemetryData.ets2.truck.fuelCapacity) * 100, String.Format("{0} l / {1} l ({2} km)", Math.Round(TelemetryData.ets2.truck.fuel, 0), Math.Round(TelemetryData.ets2.truck.fuelCapacity, 0), Math.Round(TelemetryData.ets2.truck.fuelRange, 0)), "Microsoft Sans Serif", Brushes.Brown);
                            }
                            else
                            {
                                PictureBoxCustomProgressBar(pictureBox3_fuel, Color.White, (TelemetryData.ets2.truck.fuel / TelemetryData.ets2.truck.fuelCapacity) * 100, String.Format("{0} l / {1} l ({2} km)", Math.Round(TelemetryData.ets2.truck.fuel, 0), Math.Round(TelemetryData.ets2.truck.fuelCapacity, 0), Math.Round(TelemetryData.ets2.truck.fuelRange, 0)), "Microsoft Sans Serif", Brushes.LimeGreen);
                            }
                        }
                        else
                        {// ProgressBar fuel reset
                            PictureBoxCustomProgressBar(pictureBox3_fuel, Color.White, 0, String.Format("0 l / 0 l (0 km)"), "Microsoft Sans Serif", Brushes.LimeGreen);
                        }
                    }
                    else
                    {// ATS
                        if (TelemetryData.ets2.truck.id != "")
                        {
                            if (TelemetryData.ets2.truck.fuelWarningOn)
                            {
                                PictureBoxCustomProgressBar(pictureBox3_fuel, Color.White, ((TelemetryData.ets2.truck.fuel / 3.7886952) / (TelemetryData.ets2.truck.fuelCapacity / 3.7886952)) * 100, String.Format("{0} gal / {1} gal ({2} mi)", Math.Round(TelemetryData.ets2.truck.fuel / 3.7886952, 0), Math.Round(TelemetryData.ets2.truck.fuelCapacity / 3.7886952, 0), Math.Round(TelemetryData.ets2.truck.fuelRange / 1.609344, 0)), "Microsoft Sans Serif", Brushes.Brown);
                            }
                            else
                            {
                                PictureBoxCustomProgressBar(pictureBox3_fuel, Color.White, ((TelemetryData.ets2.truck.fuel / 3.7886952) / (TelemetryData.ets2.truck.fuelCapacity / 3.7886952)) * 100, String.Format("{0} gal / {1} gal ({2} mi)", Math.Round(TelemetryData.ets2.truck.fuel / 3.7886952, 0), Math.Round(TelemetryData.ets2.truck.fuelCapacity / 3.7886952, 0), Math.Round(TelemetryData.ets2.truck.fuelRange / 1.609344, 0)), "Microsoft Sans Serif", Brushes.LimeGreen);
                            }
                        }
                        else
                        {// ProgressBar fuel reset
                            PictureBoxCustomProgressBar(pictureBox3_fuel, Color.White, 0, String.Format("0 gal / 0 gal (0 mi)"), "Microsoft Sans Serif", Brushes.LimeGreen);
                        }
                    }

                    // Calculator label
                    if (TelemetryData.ets2.game.gameID == "eut2")
                    {// ETS2
                        label8.Text = "km/h";
                        label9.Text = "km";
                    }
                    else
                    {// ATS
                        label8.Text = "mph";
                        label9.Text = "mi";
                    }

                    //Set savedcontract
                    LastKnownEstimatedDistance = TelemetryData.ets2.truck.navigationEstimatedTime;
                    savedcontract.GameId = TelemetryData.ets2.game.gameID;
                    savedcontract.SourceCity = TelemetryData.ets2.job.sourceCity;
                    savedcontract.SourceCompany = TelemetryData.ets2.job.sourceCompany;
                    savedcontract.DestinationCity = TelemetryData.ets2.job.destinationCity;
                    savedcontract.DestinationCompany = TelemetryData.ets2.job.destinationCompany;
                    savedcontract.Income = TelemetryData.ets2.job.income;
                    savedcontract.TotalMass = (float)Math.Round(TelemetryData.ets2.job.cargo.totalMass, 0);
                    savedcontract.LastProfile = TelemetryData.ets2.game.lastProfile;
                    savedcontract.SpeedSummary = speedsummary;
                    savedcontract.TimerCounter = timercounter;
                    savedcontract.DrivenDistance = drivendistance;

                    // Get ContractData if available
                    if (StartApplicationContractLoaded == false)
                    {
                        if (settings.AutoSaveActive == true && TelemetryData.ets2.job.sourceCity != "" && TelemetryData.ets2.job.sourceCompany != "" && TelemetryData.ets2.job.destinationCity != "" && TelemetryData.ets2.job.destinationCompany != "" && TelemetryData.ets2.game.lastProfile != "" && TelemetryData.ets2.job.income != 0)
                        {
                            try
                            {
                                savedcontract = (JsonConvert.DeserializeObject<SavedContract>(File.ReadAllText(String.Format(SoftwarePath + @"\contracts\{0}_AutoSaveContract_{1} - {2}___id-{3}.json", savedcontract.GameId, savedcontract.SourceCity, savedcontract.DestinationCity, (savedcontract.Income + savedcontract.TotalMass)))));

                                if (savedcontract.SourceCity == TelemetryData.ets2.job.sourceCity && savedcontract.SourceCompany == TelemetryData.ets2.job.sourceCompany && savedcontract.DestinationCity == TelemetryData.ets2.job.destinationCity && savedcontract.DestinationCompany == TelemetryData.ets2.job.destinationCompany && savedcontract.Income == TelemetryData.ets2.job.income && savedcontract.LastProfile == TelemetryData.ets2.game.lastProfile)
                                {
                                    speedsummary = savedcontract.SpeedSummary;
                                    timercounter = savedcontract.TimerCounter;
                                    drivendistance = savedcontract.DrivenDistance;
                                    StartApplicationContractLoaded = true;
                                    ContractSaved = true;
                                }
                            }
                            catch
                            {
                                StartApplicationContractLoaded = true;
                                if (TelemetryData.ets2.truck.navigationEstimatedDistance < (TelemetryData.ets2.job.cargo.plannedDistanceKM - 5))
                                {
                                    MessageBox.Show("Es scheint, dass Sie den derzeitigen Auftrag ohne diese Software begonnen haben oder der Auftrag älter als ein Monat ist. Beachten Sie bitte, dass die Auftragsdaten (gefahrene KM, Durchschnittsgeschwindigkeit) zurückgesetzt werden und es dadurch zu abweichungen in den gennanten Punkten kommen kann.", "Information!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                        }
                        else if (TelemetryData.ets2.truck.brandID != "" && TelemetryData.ets2.job.sourceCity == "" && TelemetryData.ets2.job.sourceCompany == "" && TelemetryData.ets2.job.destinationCity == "" && TelemetryData.ets2.job.destinationCompany == "" && TelemetryData.ets2.job.income == 0)
                        {
                            StartApplicationContractLoaded = true;
                        }
                    }

                }
                else if (TelemetryData.ets2.game.connected == false)
                {
                    label_connectionstatus.Text = "Keine Verbindung zum Spiel";
                    label_connectionstatus.BackColor = System.Drawing.Color.Brown;
                    label2_timescale.Text = "Zeitskalierung: -";
                }
            }
            else if (bTelemetryOnline == false)
            {
                label_connectionstatus.Text = "Keine Verbindung zum Server";
                label2_timescale.Text = "Zeitskalierung: -";
                label_connectionstatus.BackColor = System.Drawing.Color.Brown;
            }
        }


        // Timer2_calculateMinute (Every Minute one Tick)
        private void Timer2_calculateMinute_Tick(object sender, EventArgs e)
        {
            Timer2_calculate_MethodTick();
        }
        private void Timer2_calculate_MethodTick()
        {
            // Backupper
            try
            {

                if (settings.AutoSaveActive == true && bTelemetryOnline == true && TelemetryData.ets2.game.connected == true && speedsummary > 0 && timercounter > 0 && drivendistance > 0 && LastKnownEstimatedDistance >= 5 && savedcontract.SourceCity != "" && savedcontract.SourceCompany != "" && savedcontract.DestinationCity != "" && savedcontract.DestinationCompany != "" && savedcontract.LastProfile != "" && savedcontract.Income != 0)
                {
                    string sJson = JsonConvert.SerializeObject(savedcontract);
                    File.WriteAllText(String.Format(SoftwarePath + @"\contracts\{0}_AutoSaveContract_{1} - {2}___id-{3}.json", savedcontract.GameId, TelemetryData.ets2.job.sourceCity, TelemetryData.ets2.job.destinationCity, TelemetryData.ets2.job.income + Math.Round(TelemetryData.ets2.job.cargo.totalMass, 0)), sJson);
                    ContractSaved = true;
                }
            }
            catch
            {
                MessageBox.Show("Error! (Method Automatic Backupper Save)", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ContractSaved = false;
            }


            // schedule
            if (scheduleLoaded == true)
            {
                if (listWorkshifts[listWorkshifts.Count - 1].EndDate > DateTime.Now)
                {// Check if schedule is not outdated

                    tableLayoutPanel_Bottom.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

                    button_LoadDeleteSchedule.BackColor = Color.Brown;
                    button_LoadDeleteSchedule.Text = "Schichtplan löschen";

                    // Set listalldates
                    int foreachCounter = 0;
                    List<AlldatesTimeSpan> listalldates = new List<AlldatesTimeSpan>();
                    foreach (Workshift Item in listWorkshifts)
                    {
                        foreachCounter++;
                        AlldatesTimeSpan startdate = new AlldatesTimeSpan(foreachCounter, Item.StartDate.Subtract(DateTime.Now), "StartDate");
                        AlldatesTimeSpan enddate = new AlldatesTimeSpan(foreachCounter, Item.EndDate.Subtract(DateTime.Now), "EndDate");
                        AlldatesTimeSpan startpause = new AlldatesTimeSpan(foreachCounter, Item.StartPause.Subtract(DateTime.Now), "StartPause");
                        AlldatesTimeSpan endpause = new AlldatesTimeSpan(foreachCounter, Item.EndPause.Subtract(DateTime.Now), "EndPause");
                        if (Item.StartDate.Ticks > DateTime.Now.Ticks) { listalldates.Add(startdate); }
                        if (Item.EndDate.Ticks > DateTime.Now.Ticks) { listalldates.Add(enddate); }
                        if (Item.StartPause.Ticks > DateTime.Now.Ticks) { listalldates.Add(startpause); }
                        if (Item.EndPause.Ticks > DateTime.Now.Ticks) { listalldates.Add(endpause); }
                    }

                    // Set Timespans
                    List<TimeSpan> timeSpans = new List<TimeSpan>();
                    foreach (AlldatesTimeSpan timespan in listalldates)
                    {
                        _ = new TimeSpan();
                        TimeSpan ts = timespan.TimeSpan;
                        timeSpans.Add(ts);
                    }

                    // Get Min Value of Timespans and set Index for it (next event)
                    int CurrentIndex = -1;
                    string CurrentType = "";
                    foreach (AlldatesTimeSpan Item in listalldates)
                    {
                        if (Item.TimeSpan.Ticks == timeSpans.Min().Ticks)
                        {
                            CurrentIndex = Item.Index;
                            CurrentType = Item.Type;
                        }
                    }

                    bool currentShiftPauseOver = false;
                    bool schedulePause = false;
                    CurrentIndex--;
                    if (CurrentType == "StartDate")
                    {
                        ShiftActive = false;
                        label_nextscheduleevent.Text = String.Format("Nächstes Schichtereignis: [Schichtbeginn]   {0} Uhr,  {1}", listWorkshifts[CurrentIndex].StartDate.ToString("HH:mm"), listWorkshifts[CurrentIndex].StartDate.ToShortDateString());
                    }
                    else if (CurrentType == "EndDate")
                    {
                        currentShiftPauseOver = true;
                        ShiftActive = true;
                        label_nextscheduleevent.Text = String.Format("Nächstes Schichtereignis: [Schichtende]   {0} Uhr", listWorkshifts[CurrentIndex].EndDate.ToString("HH:mm"));
                    }
                    else if (CurrentType == "StartPause")
                    {
                        ShiftActive = true;
                        label_nextscheduleevent.Text = String.Format("Nächstes Schichtereignis: [Schichtpausenbeginn]   {0} Uhr", listWorkshifts[CurrentIndex].StartPause.ToString("HH:mm"));
                    }
                    else if (CurrentType == "EndPause")
                    {
                        schedulePause = true;
                        ShiftActive = true;
                        label_nextscheduleevent.Text = String.Format("Nächstes Schichtereignis: [Schichtpausenende]   {0} Uhr", listWorkshifts[CurrentIndex].EndPause.ToString("HH:mm"));
                    }

                    // label shiftcount
                    label_shiftcount.Text = String.Format("Schicht: {0} / {1}", (CurrentIndex + 1), foreachCounter);

                    if (ShiftActive == true)
                    {
                        label_shiftstatus.BackColor = Color.LimeGreen;
                        label_shiftstatus.Text = "Schicht aktiv";

                        // label_currentshift
                        label_currentshift.Text = String.Format("Derzeitige Schicht: {0} Uhr,  {1}  -  {2} Uhr,  {3}", listWorkshifts[CurrentIndex].StartDate.ToString("HH:mm"), listWorkshifts[CurrentIndex].StartDate.ToShortDateString(), listWorkshifts[CurrentIndex].EndDate.ToString("HH:mm"), listWorkshifts[CurrentIndex].EndDate.ToShortDateString());

                        label_nextpausestartend.Location = new Point(650, label_nextpausestartend.Location.Y);
                        label_timetoshiftend.Location = new Point(974, label_timetoshiftend.Location.Y);
                        label_currentshift.Location = new Point(1293, label_currentshift.Location.Y);
                        label_currentshift.Width = 432;

                        timeSpans.Sort();
                        // Get next shift end
                        CurrentIndex = -1;
                        foreach (AlldatesTimeSpan Item in listalldates)
                        {
                            if (timeSpans.Count >= 3)
                            {
                                if (Item.TimeSpan.Ticks == timeSpans[0].Ticks && Item.Type == "EndDate")
                                {
                                    CurrentIndex = Item.Index;
                                }
                                else if (Item.TimeSpan.Ticks == timeSpans[1].Ticks && Item.Type == "EndDate")
                                {
                                    CurrentIndex = Item.Index;
                                }
                                else if (Item.TimeSpan.Ticks == timeSpans[2].Ticks && Item.Type == "EndDate")
                                {
                                    CurrentIndex = Item.Index;
                                }
                                else if (Item.TimeSpan.Ticks == timeSpans[3].Ticks && Item.Type == "EndDate")
                                {
                                    CurrentIndex = Item.Index;
                                }
                            }
                            else
                            {
                                CurrentIndex = 1; // (this value later goes to zero)
                            }
                        }
                        CurrentIndex--;
                        TimeSpan shiftTimeLeft = TimeSpan.FromTicks(listWorkshifts[CurrentIndex].EndDate.Ticks - DateTime.Now.Ticks);
                        label_timetoshiftend.Text = String.Format("Übrige Schichtlänge: {0}", TimeSpanConvertToAvailableValuesOnly(shiftTimeLeft));


                        // Get next shift pausestart
                        if (schedulePause == true)
                        {
                            label_shiftstatus.BackColor = Color.Goldenrod;
                            label_shiftstatus.Text = "Schichtpause";

                            CurrentIndex = -1;
                            foreach (AlldatesTimeSpan Item in listalldates)
                            {
                                if (timeSpans.Count >= 3)
                                {
                                    if (Item.TimeSpan.Ticks == timeSpans[0].Ticks && Item.Type == "EndPause")
                                    {
                                        CurrentIndex = Item.Index;
                                    }
                                    else if (Item.TimeSpan.Ticks == timeSpans[1].Ticks && Item.Type == "EndPause")
                                    {
                                        CurrentIndex = Item.Index;
                                    }
                                    else if (Item.TimeSpan.Ticks == timeSpans[2].Ticks && Item.Type == "EndPause")
                                    {
                                        CurrentIndex = Item.Index;
                                    }
                                    else if (Item.TimeSpan.Ticks == timeSpans[3].Ticks && Item.Type == "EndPause")
                                    {
                                        CurrentIndex = Item.Index;
                                    }
                                }
                                else
                                {
                                    CurrentIndex = 1; // (this value later goes to zero)
                                }
                            }
                            CurrentIndex--;
                            TimeSpan nextPauseEnd = TimeSpan.FromTicks(listWorkshifts[CurrentIndex].EndPause.Ticks - DateTime.Now.Ticks);
                            label_nextpausestartend.Text = String.Format("Pausenende in: {0}", TimeSpanConvertToAvailableValuesOnly(nextPauseEnd));
                        }
                        else
                        {
                            if (currentShiftPauseOver == false)
                            {
                                CurrentIndex = -1;
                                foreach (AlldatesTimeSpan Item in listalldates)
                                {
                                    if (timeSpans.Count >= 3)
                                    {
                                        if (Item.TimeSpan.Ticks == timeSpans[0].Ticks && Item.Type == "StartPause")
                                        {
                                            CurrentIndex = Item.Index;
                                        }
                                        else if (Item.TimeSpan.Ticks == timeSpans[1].Ticks && Item.Type == "StartPause")
                                        {
                                            CurrentIndex = Item.Index;
                                        }
                                        else if (Item.TimeSpan.Ticks == timeSpans[2].Ticks && Item.Type == "StartPause")
                                        {
                                            CurrentIndex = Item.Index;
                                        }
                                        else if (Item.TimeSpan.Ticks == timeSpans[3].Ticks && Item.Type == "StartPause")
                                        {
                                            CurrentIndex = Item.Index;
                                        }
                                    }
                                    else
                                    {
                                        CurrentIndex = 1; // (this value later goes to zero)
                                    }
                                }
                                CurrentIndex--;
                                TimeSpan nextPauseStart = TimeSpan.FromTicks(listWorkshifts[CurrentIndex].StartPause.Ticks - DateTime.Now.Ticks);
                                label_nextpausestartend.Text = String.Format("Nächste Pause in: {0}", TimeSpanConvertToAvailableValuesOnly(nextPauseStart));
                            }
                            else
                            {
                                label_nextpausestartend.Text = "Nächste Pause in: ---";
                            }
                        }

                    }
                    else
                    {
                        label_timetoshiftend.Text = "Übrige Schichtlänge: ---";
                        label_nextpausestartend.Text = "Nächste Pause in: ---";
                        label_currentshift.Text = "Derzeitige Schicht: ---";

                        label_shiftstatus.BackColor = Color.Goldenrod;
                        label_shiftstatus.Text = "Schicht nicht aktiv";

                        label_nextpausestartend.Location = new Point(750, label_nextpausestartend.Location.Y);
                        label_timetoshiftend.Location = new Point(1074, label_timetoshiftend.Location.Y);
                        label_currentshift.Location = new Point(1393, label_currentshift.Location.Y);
                        label_currentshift.Width = 200;
                    }

                }
                else
                {
                    MessageBox.Show("Der Zeitplan wurde abgeschlossen!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    scheduleLoaded = false;

                    // ScheduleLoaded = false direct calculations
                    tableLayoutPanel_Bottom.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;

                    label_shiftcount.Text = "";
                    label_nextscheduleevent.Text = "";
                    label_timetoshiftend.Text = "";
                    label_nextpausestartend.Text = "";
                    label_currentshift.Text = "";


                    label_shiftstatus.BackColor = Color.Brown;
                    label_shiftstatus.Text = "Keine Schicht geladen";

                    button_LoadDeleteSchedule.BackColor = Color.LightSteelBlue;
                    button_LoadDeleteSchedule.Text = "Schichtplan laden";

                    listBox_schedule.Items.Clear();
                    listWorkshifts.Clear();
                }

            }
            else
            {
                tableLayoutPanel_Bottom.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;

                label_shiftcount.Text = "";
                label_nextscheduleevent.Text = "";
                label_timetoshiftend.Text = "";
                label_nextpausestartend.Text = "";
                label_currentshift.Text = "";


                label_shiftstatus.BackColor = Color.Brown;
                label_shiftstatus.Text = "Keine Schicht geladen";

                button_LoadDeleteSchedule.BackColor = Color.LightSteelBlue;
                button_LoadDeleteSchedule.Text = "Schichtplan laden";

                listBox_schedule.Items.Clear();
                listWorkshifts.Clear();
            }
        }


        void PictureBoxCustomProgressBar(PictureBox pb, Color colorBack, double dProgress, string sBarText, string sFont, Brush brushProgressColor)
        {// Method Custom ProgressBar with Picture Box
            if (WindowState != FormWindowState.Minimized)
            {
                try
                {
                    double pbUnit = pb.Width;
                    pbUnit /= 100;
                    Bitmap bmp;
                    Graphics graphics;
                    bmp = new Bitmap(pb.Width, pb.Height);
                    graphics = Graphics.FromImage(bmp);
                    graphics.Clear(colorBack);
                    graphics.FillRectangle(brushProgressColor, new Rectangle(0, 0, (int)(pbUnit * dProgress), pb.Height)); //(int)
                    StringFormat stringFormat = new StringFormat
                    {
                        Alignment = StringAlignment.Center
                    };
                    graphics.DrawString(sBarText, new Font(sFont, pb.Height / 2), Brushes.Black, new PointF(pb.Width / 2, pb.Height / 10), stringFormat);
                    pb.Image = bmp;
                }
                catch
                {

                }
            }
        }

        static string TimeSpanConvertToAvailableValuesOnly(TimeSpan ts)
        {// Only shows available values (00:02 ---> 2 min. (and not 0 hrs. 2 min.))
            if (ts.Days != 0)
            {
                return ((ts.Days * 24) + ts.Hours) + " Std. " + ts.Minutes + " Min.";
            }
            else
            {
                if (ts.Hours != 0)
                {
                    return ts.Hours + " Std. " + ts.Minutes + " Min.";
                }
                else
                {
                    return ts.Minutes + " Min.";
                }
            }
        }



        // Distance calculator
        private void Calculator_RadioButtonChanged(object sender, EventArgs e)
        {
            if (radioButton_standart.Checked == true && radioButton_extended.Checked == false)
            {
                label_Calculatortime1.Text = "Fahrzeit:";
                label_Calculatortime2.Visible = false;
                label_Calculatortime3.Visible = false;
                numericUpDown_time2.Visible = false;
                numericUpDown_time3.Visible = false;
                label_CalculatortimeH2.Visible = false;
                label_CalculatortimeH3.Visible = false;
                numericUpDown_time2.Value = 0;
                numericUpDown_time3.Value = 0;
            }
            else if (radioButton_extended.Checked == true && radioButton_standart.Checked == false)
            {
                label_Calculatortime1.Text = "Fahrzeit (19):";
                label_Calculatortime2.Visible = true;
                label_Calculatortime3.Visible = true;
                numericUpDown_time2.Visible = true;
                numericUpDown_time3.Visible = true;
                label_CalculatortimeH2.Visible = true;
                label_CalculatortimeH3.Visible = true;
                if (TimeToKm == false)
                {
                    numericUpDown_time1.Value = (numericUpDown_km.Value / 19) / numericUpDown_speed.Value;
                    numericUpDown_time2.Value = (numericUpDown_km.Value / 15) / numericUpDown_speed.Value;
                    numericUpDown_time3.Value = (numericUpDown_km.Value / 3) / numericUpDown_speed.Value;
                }
            }
        }

        private void Calculator_NumericFocusLost(object sender, CancelEventArgs e)
        {
            if (numericUpDown_time1.Text == "")
            {
                numericUpDown_time1.Value = 0;
                numericUpDown_time1.Text = "0";
            }
            else if (numericUpDown_time2.Text == "")
            {
                numericUpDown_time2.Value = 0;
                numericUpDown_time2.Text = "0";
            }
            else if (numericUpDown_time3.Text == "")
            {
                numericUpDown_time3.Value = 0;
                numericUpDown_time3.Text = "0";
            }
            else if (numericUpDown_speed.Text == "")
            {
                numericUpDown_speed.Value = 65;
                numericUpDown_speed.Text = "65";
            }
            else if (numericUpDown_km.Text == "")
            {
                numericUpDown_km.Value = 0;
                numericUpDown_km.Text = "0";
            }
        }

        bool TimeToKm;

        private void Calculator_NumericTimePressed(object sender, EventArgs e)
        {
            TimeToKm = true;
            numericUpDown_km.ReadOnly = true;
            numericUpDown_time1.ReadOnly = false;
            numericUpDown_time2.ReadOnly = false;
            numericUpDown_time3.ReadOnly = false;
        }

        private void Calculator_NumericKmPressed(object sender, EventArgs e)
        {
            TimeToKm = false;
            numericUpDown_time1.ReadOnly = true;
            numericUpDown_time2.ReadOnly = true;
            numericUpDown_time3.ReadOnly = true;
            numericUpDown_km.ReadOnly = false;
        }

        private void Calculator_NumericKMValueChanged(object sender, EventArgs e)
        {

            if (TimeToKm == false)
            {
                if (radioButton_standart.Checked == true && radioButton_extended.Checked == false)
                {
                    try
                    {
                        numericUpDown_time1.Value = (numericUpDown_km.Value / Convert.ToDecimal(TimeScaleConstant)) / numericUpDown_speed.Value;
                    }
                    catch
                    {
                        MessageBox.Show("Zahl zu groß!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        numericUpDown_time1.Value = 0;
                        numericUpDown_time2.Value = 0;
                        numericUpDown_time3.Value = 0;
                        numericUpDown_km.Value = 0;
                    }
                }
                else if (radioButton_extended.Checked == true && radioButton_standart.Checked == false)
                {
                    try
                    {
                        numericUpDown_time1.Value = (numericUpDown_km.Value / 19) / numericUpDown_speed.Value;
                        numericUpDown_time2.Value = (numericUpDown_km.Value / 15) / numericUpDown_speed.Value;
                        numericUpDown_time3.Value = (numericUpDown_km.Value / 3) / numericUpDown_speed.Value;
                    }
                    catch
                    {
                        MessageBox.Show("Zahl zu groß!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        numericUpDown_time1.Value = 0;
                        numericUpDown_time2.Value = 0;
                        numericUpDown_time3.Value = 0;
                        numericUpDown_km.Value = 0;
                    }
                }
            }

        }
        private void CalculatorNumericTIMEChanged(object sender, EventArgs e)
        {

            if (TimeToKm == true)
            {
                if (radioButton_standart.Checked == true && radioButton_extended.Checked == false)
                {
                    try
                    {
                        numericUpDown_km.Value = Convert.ToDecimal(TimeScaleConstant) * (numericUpDown_time1.Value * numericUpDown_speed.Value);
                    }
                    catch
                    {
                        MessageBox.Show("Zahl zu groß!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        numericUpDown_time1.Value = 0;
                        numericUpDown_time2.Value = 0;
                        numericUpDown_time3.Value = 0;
                        numericUpDown_km.Value = 0;
                    }
                }
                else if (radioButton_extended.Checked == true && radioButton_standart.Checked == false)
                {
                    try
                    {
                        numericUpDown_km.Value = (19 * (numericUpDown_time1.Value * numericUpDown_speed.Value)) + (15 * (numericUpDown_time2.Value * numericUpDown_speed.Value)) + (3 * (numericUpDown_time3.Value * numericUpDown_speed.Value));
                    }
                    catch
                    {
                        MessageBox.Show("Zahl zu groß!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        numericUpDown_time1.Value = 0;
                        numericUpDown_time2.Value = 0;
                        numericUpDown_time3.Value = 0;
                        numericUpDown_km.Value = 0;
                    }
                }
            }

        }

        private void CalculatorNumericSPEEDChanged(object sender, EventArgs e)
        {
            if (radioButton_standart.Checked == true && radioButton_extended.Checked == false)
            {
                if (TimeToKm == true)
                {
                    try
                    {
                        numericUpDown_km.Value = Convert.ToDecimal(TimeScaleConstant) * (numericUpDown_time1.Value * numericUpDown_speed.Value);
                    }
                    catch
                    {
                        MessageBox.Show("Zahl zu groß!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        numericUpDown_time1.Value = 0;
                        numericUpDown_time2.Value = 0;
                        numericUpDown_time3.Value = 0;
                        numericUpDown_km.Value = 0;
                    }
                }
                else if (TimeToKm == false)
                {
                    try
                    {
                        numericUpDown_time1.Value = (numericUpDown_km.Value / Convert.ToDecimal(TimeScaleConstant)) / numericUpDown_speed.Value;

                    }
                    catch
                    {
                        MessageBox.Show("Zahl zu groß!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        numericUpDown_time1.Value = 0;
                        numericUpDown_time2.Value = 0;
                        numericUpDown_time3.Value = 0;
                        numericUpDown_km.Value = 0;
                    }
                }
            }
            else if (radioButton_extended.Checked == true && radioButton_standart.Checked == false)
            {
                if (TimeToKm == true)
                {
                    try
                    {
                        numericUpDown_km.Value = (19 * (numericUpDown_time1.Value * numericUpDown_speed.Value)) + (15 * (numericUpDown_time2.Value * numericUpDown_speed.Value)) + (3 * (numericUpDown_time3.Value * numericUpDown_speed.Value));
                    }
                    catch
                    {
                        MessageBox.Show("Zahl zu groß!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        numericUpDown_time1.Value = 0;
                        numericUpDown_time2.Value = 0;
                        numericUpDown_time3.Value = 0;
                        numericUpDown_km.Value = 0;
                    }
                }
                else if (TimeToKm == false)
                {
                    try
                    {
                        numericUpDown_time1.Value = (numericUpDown_km.Value / 19) / numericUpDown_speed.Value;
                        numericUpDown_time2.Value = (numericUpDown_km.Value / 15) / numericUpDown_speed.Value;
                        numericUpDown_time3.Value = (numericUpDown_km.Value / 3) / numericUpDown_speed.Value;

                    }
                    catch
                    {
                        MessageBox.Show("Zahl zu groß!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        numericUpDown_time1.Value = 0;
                        numericUpDown_time2.Value = 0;
                        numericUpDown_time3.Value = 0;
                        numericUpDown_km.Value = 0;
                    }
                }
            }
        }


        // Schedule planner : Focus
        private void Schedule_NumericFocusLost(object sender, CancelEventArgs e)
        {
            if (numericUpDown_drivetimeSchedule.Text == "")
            {
                numericUpDown_drivetimeSchedule.Value = 9;
                numericUpDown_drivetimeSchedule.Text = "9";
            }
            else if (numericUpDown_durationSchedule.Text == "")
            {
                numericUpDown_durationSchedule.Value = 7;
                numericUpDown_durationSchedule.Text = "7";
            }
            else if (numericUpDown_pausetimeSchedule.Text == "")
            {
                numericUpDown_pausetimeSchedule.Value = 12;
                numericUpDown_pausetimeSchedule.Text = "12";
            }
        }

        // Schedule planner : Button Create
        private void Button_CreateSchedule_Click(object sender, EventArgs e)
        {
            listWorkshifts.Clear();
            DateTime start_dt = dateTimePicker_schedule.Value;

            double TargetDays = Convert.ToDouble(numericUpDown_durationSchedule.Value);
            double DriveTime = Convert.ToDouble(numericUpDown_drivetimeSchedule.Value);
            double PauseTime = Convert.ToDouble(numericUpDown_pausetimeSchedule.Value);

            int counter = 0;
            listBox_schedule.Items.Clear();
            do
            {
                counter += 1;

                // Create new Workshift object and add it to our Workshift List
                Workshift newWorkshift = new Workshift
                {
                    Count = counter,
                    StartDate = start_dt,
                    EndDate = start_dt.AddHours(DriveTime + 0.75),
                    StartPause = start_dt.AddHours(DriveTime / 2),
                    EndPause = start_dt.AddHours((DriveTime / 2) + 0.75)
                };
                listWorkshifts.Add(newWorkshift);


                start_dt = start_dt.AddHours(PauseTime + DriveTime);
            }
            while (start_dt.Date < dateTimePicker_schedule.Value.AddDays(TargetDays));

            listBox_schedule.Items.Add("##################################################################################");
            foreach (Workshift Item in listWorkshifts)
            {
                listBox_schedule.Items.Add("Schicht: " + Item.Count);
                listBox_schedule.Items.Add("\n");
                listBox_schedule.Items.Add("Schichtbeginn               : " + Item.StartDate + "   [" + DateTimeFormatInfo.CurrentInfo.GetDayName(Item.StartDate.DayOfWeek) + "]");
                listBox_schedule.Items.Add("Schichtpausenbeginn  : " + Item.StartPause + "   [" + DateTimeFormatInfo.CurrentInfo.GetDayName(Item.StartPause.DayOfWeek) + "]");
                listBox_schedule.Items.Add("\n");
                listBox_schedule.Items.Add("Schichtpausenende     : " + Item.EndPause + "   [" + DateTimeFormatInfo.CurrentInfo.GetDayName(Item.EndPause.DayOfWeek) + "]");
                listBox_schedule.Items.Add("Schichtende                   : " + Item.EndDate + "   [" + DateTimeFormatInfo.CurrentInfo.GetDayName(Item.EndDate.DayOfWeek) + "]");
                listBox_schedule.Items.Add("\n");
                listBox_schedule.Items.Add("##################################################################################");
            }

            scheduleLoaded = true;
            Timer2_calculate_MethodTick();

        }

        // Schedule planner : Load Method
        void SchedulePlannerLoad()
        {
            try
            {// Check if FileFormat is correct

                listWorkshifts = new List<Workshift>(JsonConvert.DeserializeObject<List<Workshift>>(File.ReadAllText(openFileDialog_Schedule.FileName)));

                if (listWorkshifts[listWorkshifts.Count - 1].EndDate > DateTime.Now)
                {// Check if Schedule is not oudtdated
                    listBox_schedule.Items.Add("##################################################################################");
                    foreach (Workshift Item in listWorkshifts)
                    {
                        listBox_schedule.Items.Add("Schicht: " + Item.Count);
                        listBox_schedule.Items.Add("\n");
                        listBox_schedule.Items.Add("Schichtbeginn               : " + Item.StartDate + "   [" + DateTimeFormatInfo.CurrentInfo.GetDayName(Item.StartDate.DayOfWeek) + "]");
                        listBox_schedule.Items.Add("Schichtpausenbeginn  : " + Item.StartPause + "   [" + DateTimeFormatInfo.CurrentInfo.GetDayName(Item.StartPause.DayOfWeek) + "]");
                        listBox_schedule.Items.Add("\n");
                        listBox_schedule.Items.Add("Schichtpausenende     : " + Item.EndPause + "   [" + DateTimeFormatInfo.CurrentInfo.GetDayName(Item.EndPause.DayOfWeek) + "]");
                        listBox_schedule.Items.Add("Schichtende                   : " + Item.EndDate + "   [" + DateTimeFormatInfo.CurrentInfo.GetDayName(Item.EndDate.DayOfWeek) + "]");
                        listBox_schedule.Items.Add("\n");
                        listBox_schedule.Items.Add("##################################################################################");
                    }
                    scheduleLoaded = true;
                    Timer2_calculate_MethodTick();
                }
            }
            catch
            {
                MessageBox.Show("Die angegebene Datei hat ein falsches Format.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error); ;
                scheduleLoaded = false;
            }
        }

        // Schedule planner : Button LoadDelete 
        private void Button_LoadDeleteSchedule_Click(object sender, EventArgs e)
        {
            if (scheduleLoaded == true)
            {// DELETE

                if (MessageBox.Show("Möchten Sie den aktuellen Schichtplan löschen?", "Warnung!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    scheduleLoaded = false;
                    Timer2_calculate_MethodTick();
                }
            }
            else
            {// LOAD (Get Json Data from file)

                openFileDialog_Schedule.InitialDirectory = (SoftwarePath + @"\work shifts");
                if (openFileDialog_Schedule.ShowDialog() == DialogResult.OK)
                {
                    SchedulePlannerLoad();

                }
            }
        }

        // Schedule planner : Button LoadDelete_MENU
        private void Button_LoadDeleteScheduleMenu_Click(object sender, EventArgs e)
        {// LOAD (Get Json Data from file)

            openFileDialog_Schedule.InitialDirectory = (SoftwarePath + @"\work shifts");
            if (openFileDialog_Schedule.ShowDialog() == DialogResult.OK)
            {
                if (scheduleLoaded == true)
                {
                    if (MessageBox.Show("Möchte Sie fortfahren und damit den derzeitigen Zeitplan löschen?", "Warnung!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        SchedulePlannerLoad();
                    }
                }
                else
                {
                    SchedulePlannerLoad();
                }

            }

        }

        // Schedule planner : Button Save_MENU
        void Button_SaveScheduleMenu_click(object sender, EventArgs e)
        {
            try
            {
                if (scheduleLoaded == true)
                {
                    saveFileDialog_Schedule.InitialDirectory = (SoftwarePath + @"\work shifts");
                    saveFileDialog_Schedule.FileName = String.Format("TimeSchedule_{0} - {1}", listWorkshifts[0].StartDate.ToString("dd-MM-yyyy HHmm"), listWorkshifts[listWorkshifts.Count - 1].EndDate.ToString("dd-MM-yyyy HHmm"));
                    if (saveFileDialog_Schedule.ShowDialog() == DialogResult.OK)
                    {
                        Timer2_calculate_MethodTick();

                        string sJson = JsonConvert.SerializeObject(listWorkshifts, Formatting.Indented);
                        File.WriteAllText(saveFileDialog_Schedule.FileName, sJson);
                    }
                }
                else
                {
                    MessageBox.Show("Derzeit ist kein Schichtplan geladen worden. Es konnte nichts gespeichert werden.", "Kein Schichtplan!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch
            {
                MessageBox.Show("Error! (Method SaveScheduleMenu)", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Contract Data : Save
        private void AuftragsdateSpeichernToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Sie haben automatisches Speichern eingeschaltet. Es ist nicht notwendig manuell zu speichern. Möchten Sie trotzdem fortfahren?", "Manuelles Speichern nicht notwendig!", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                try
                {
                    if (speedsummary > 0 && timercounter > 0 && drivendistance > 0 && LastKnownEstimatedDistance >= 5 && savedcontract.SourceCity != "" && savedcontract.SourceCompany != "" && savedcontract.DestinationCity != "" && savedcontract.DestinationCompany != "" && savedcontract.LastProfile != "" && savedcontract.Income != 0)
                    {
                        saveFileDialog_Contract.InitialDirectory = (SoftwarePath + @"\contracts");
                        saveFileDialog_Contract.FileName = String.Format("{0}_Contract_{1} - {2}___id-{3}", savedcontract.GameId, savedcontract.SourceCity, savedcontract.DestinationCity, savedcontract.Income + savedcontract.TotalMass);
                        if (saveFileDialog_Contract.ShowDialog() == DialogResult.OK)
                        {
                            string sJson = JsonConvert.SerializeObject(savedcontract);
                            File.WriteAllText(saveFileDialog_Contract.FileName, sJson);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Es konnten nicht alle Daten gefunden werden. Haben Sie einen aktiven Auftrag? - Beachten Sie, dass Sie mindestens für ~2 s schneller als 5 km/h gefahren sein müssen.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch
                {
                    MessageBox.Show("Error! (Method SaveContract)", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Contract Data : Load
        private void AuftragsdatenLadenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Sie haben automatisches Speichern eingeschaltet. Es ist nicht notwendig manuell zu speichern. Möchten Sie trotzdem fortfahren?", "Manuelles Speichern nicht notwendig!", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                try
                {
                    if (bTelemetryOnline == true && TelemetryData.ets2.game.connected == true && TelemetryData.ets2.job.sourceCity != "" && TelemetryData.ets2.job.sourceCompany != "" && TelemetryData.ets2.job.destinationCity != "" && TelemetryData.ets2.job.destinationCompany != "" && TelemetryData.ets2.game.lastProfile != "" && TelemetryData.ets2.job.income != 0)
                    {
                        openFileDialog_Contract.InitialDirectory = (SoftwarePath + @"\contracts");
                        openFileDialog_Contract.FileName = String.Format("{0}_AutoSaveContract_{1} - {2}___id-{3}.json", savedcontract.GameId, TelemetryData.ets2.job.sourceCity, TelemetryData.ets2.job.destinationCity, TelemetryData.ets2.job.income + Math.Round(TelemetryData.ets2.job.cargo.totalMass, 0));
                        if (openFileDialog_Contract.ShowDialog() == DialogResult.OK)
                        {
                            savedcontract = (JsonConvert.DeserializeObject<SavedContract>(File.ReadAllText(openFileDialog_Contract.FileName)));

                            if (savedcontract.SourceCity == TelemetryData.ets2.job.sourceCity && savedcontract.SourceCompany == TelemetryData.ets2.job.sourceCompany && savedcontract.DestinationCity == TelemetryData.ets2.job.destinationCity && savedcontract.DestinationCompany == TelemetryData.ets2.job.destinationCompany && savedcontract.Income == TelemetryData.ets2.job.income && savedcontract.LastProfile == TelemetryData.ets2.game.lastProfile)
                            {
                                speedsummary = savedcontract.SpeedSummary;
                                timercounter = savedcontract.TimerCounter;
                                drivendistance = savedcontract.DrivenDistance;
                            }
                            else
                            {
                                MessageBox.Show("Um die Auftragsdaten zu laden müssen Sie den gleiche Auftrag im Spiel aktiv haben.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Um die Auftragsdaten zu laden müssen Sie den gleiche Auftrag im Spiel aktiv haben.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch
                {
                    MessageBox.Show("Es wurde entweder eine falsche Datei geöffntet oder die Datei wurde beschädigt. ", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void Timer3_antikick_Tick(object sender, EventArgs e)
        {
            Process[] process1 = Process.GetProcessesByName("eurotrucks2");
            Process[] process2 = Process.GetProcessesByName("amtrucks");
            if (process1.Length != 0)
            {
                if (process1[0].MainWindowHandle == MainForm.GetForegroundWindow())
                {
                    SendKeys.Send("y/p{Enter}");
                }
            }

            if (process2.Length != 0)
            {
                if (process2[0].MainWindowHandle == MainForm.GetForegroundWindow())
                {
                    SendKeys.Send("y/p{Enter}");
                }
            }
        }

        // Open settings
        private void EinstellungenToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SettingsForm settingform = new SettingsForm();
            settingform.Show();
        }


        // Application Close
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Save Contractdata if available
            try
            {

                if (settings.AutoSaveActive == true && speedsummary > 0 && timercounter > 0 && drivendistance > 0 && LastKnownEstimatedDistance >= 5 && savedcontract.SourceCity != "" && savedcontract.SourceCompany != "" && savedcontract.DestinationCity != "" && savedcontract.DestinationCompany != "" && savedcontract.LastProfile != "" && savedcontract.Income != 0)
                {
                    string sJson = JsonConvert.SerializeObject(savedcontract);
                    File.WriteAllText(String.Format(SoftwarePath + @"\contracts\{0}_AutoSaveContract_{1} - {2}___id-{3}.json", savedcontract.GameId, TelemetryData.ets2.job.sourceCity, TelemetryData.ets2.job.destinationCity, TelemetryData.ets2.job.income + Math.Round(TelemetryData.ets2.job.cargo.totalMass, 0)), sJson);
                }
            }
            catch
            {
                MessageBox.Show("Die Auftragsdaten wurden aufgrund eines Fehlers nicht gespeichert. (Der letzte Autosave wurde maximal eine Minute früher erstellt)", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MainForm_ResizeBegin(object sender, EventArgs e)
        {
            this.SuspendLayout();
        }

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            this.ResumeLayout();
        }


        // Try to start Server
        private void StartServer()
        {
            server.Start();

            if (server.HasEntries() == false)
            {
                if (MessageBox.Show("Es scheint, dass der Server dieser Anwendung nicht richtig implementiert wurde. Soll dieses Problem jetzt behoben werden?", "Fehlende Einträge des Servers!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    server.SetPowerShellEntries();
                }
            }
            SetServerLabels();
        }
        // Try to stop server
        private void StopServer()
        {
            if (server.IsRunning() == true)
            {
                server.Stop();
            }
            SetServerLabels();
        }
        // Try to install server
        private void InstallServer()
        {
            if (server.HasEntries() == false)
            {
                server.SetPowerShellEntries();
            }
            SetServerLabels();
        }
        // Try to uninstall server
        private void UninstallServer()
        {
            if (server.HasEntries() == true)
            {
                server.DeletePowerShellEntries();
            }
            SetServerLabels();
        }
        // Server buttons 
        private void serverInstallUninstallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (server.HasEntries() == true)
            {
                UninstallServer();
            }
            else
            {
                InstallServer();
            }
        }

        private void IPAdresseAnzeigenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (server.IsRunning() == true)
            {
                try
                {
                    IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                    string ipAddress = "000.000.000.00";
                    foreach (IPAddress ip in host.AddressList)
                    {
                        if (ip.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ipAddress = ip.ToString();
                        }
                    }

                    MessageBox.Show(String.Format("http://{0}:{1}/", ipAddress, Port.iPort), "IpAdresse", MessageBoxButtons.OK);
                }
                catch
                {
                    MessageBox.Show("Für diesen Computer konnte keine IpAdresse gefunden werden. Sind Sie mit dem Internet verbunden?", "Keine IpAdresse gefunden!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Bitte starten Sie den Server.", "Server wurde nicht gestartet!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void serverStartStopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (server.IsRunning() == true)
            {
                StopServer();
            }
            else
            {
                StartServer();
            }
        }
        private void SetServerLabels()
        {
            // Set server labels
            if (server.HasEntries() == true)
            {
                serverInstallUninstallToolStripMenuItem.Text = "Server deinstallieren";
            }
            else
            {
                serverInstallUninstallToolStripMenuItem.Text = "Server installieren";
            }
            if (server.IsRunning() == true)
            {
                serverToolStripMenuItem.BackColor = Color.LimeGreen;
                serverStartStopToolStripMenuItem.Text = "Server stoppen";
            }
            else
            {
                serverToolStripMenuItem.BackColor = Color.Brown;
                serverStartStopToolStripMenuItem.Text = "Server starten";
            }
        }


        // AntiKick CheckedState changed
        private void AntikickToolStripMenuItem1_CheckedChanged(object sender, EventArgs e)
        {
            if (antikickToolStripMenuItem1.Checked == true)
            {
                timer3_antikick.Start();
            }
            else
            {
                timer3_antikick.Stop();
            }
        }
    }
}


