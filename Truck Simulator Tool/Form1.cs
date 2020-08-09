using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Truck_Simulator_Tool
{
    public partial class Form1 : Form // main form
    {
        // Variables Start

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


        // Variables End

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1_calculate.Start();


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

            //Schedule
            dateTimePicker_schedule.Value = DateTime.Now.AddDays(1);
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



        private async void timer1_calculate_Tick(object sender, EventArgs e)
        {
            await UpdateTelemetry();
            await UpdateTruckersFM();
            label14_datetimetime.Text = DateTime.Now.ToString("HH:mm");
            label_datetimenowseconds.Text = DateTime.Now.ToString("ss");
            label15_datetimedate.Text = DateTimeFormatInfo.CurrentInfo.GetDayName(DateTime.Now.DayOfWeek) + "\n" + DateTime.Now.ToShortDateString();
            dateTimePicker_schedule.MinDate = DateTime.Now.AddDays((-1) * ((Convert.ToDouble(numericUpDown_durationSchedule.Value) - 0.5)));

            // schedule
            if (scheduleLoaded == true)
            {
                if (listWorkshifts[listWorkshifts.Count - 1].EndDate > DateTime.Now)
                {// Check if schedule is not outdated

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
                        TimeSpan ts = new TimeSpan();
                        ts = timespan.TimeSpan;
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
                        label_shiftText.BackColor = Color.Blue;
                        label_shiftText.Text = "Schicht aktiv";

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
                            label_shiftText.BackColor = Color.MediumBlue;
                            label_shiftText.Text = "Schichtpause";

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

                        label_shiftText.BackColor = Color.MediumBlue;
                        label_shiftText.Text = "Schicht nicht aktiv";

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
                }

            }
            else
            {
                label_shiftText.BackColor = Color.Brown;
                label_shiftText.Text = "Keine Schicht geladen";

                button_LoadDeleteSchedule.BackColor = Color.LightSteelBlue;
                button_LoadDeleteSchedule.Text = "Schichtplan laden";

                listBox_schedule.Items.Clear();
                listWorkshifts.Clear();
            }

            if (bTruckersfmOnline == true)


            {// TFM online
                HttpClient client = new HttpClient();
                Stream stream = await client.GetStreamAsync(TruckersfmsongData.art.ToString());
                pictureBox_TruckersfmSong.BackgroundImage = System.Drawing.Image.FromStream(stream);

                label_TFMsongname.Text = TruckersfmsongData.title;
                label_TFMsongartist.Text = TruckersfmsongData.artist;
                label_TFMdjname.Text = "Moderator: " + TruckersfmdjData.result.dj.name;
            }


            if (bTelemetryOnline == true)
            {// Telemetry online
                if (TelemetryData.ets2.game.connected == true)
                {// Game connected

                    if (TelemetryData.ets2.truck.speed > 5 && TelemetryData.ets2.game.paused == false)
                    {// Average Variables Calcuations
                        timercounter += 1;
                        speedsummary += TelemetryData.ets2.truck.speed;
                        currentaveragespeed = speedsummary / timercounter;
                        if (TelemetryData.ets2.truck.navigationEstimatedDistance > 0)
                        {
                            bestcurrentaveragespeed = (TelemetryData.ets2.truck.navigationEstimatedDistance / 1000) / (Convert.ToDouble(TelemetryData.ets2.truck.navigationEstimatedTime) / 3600);
                        }
                        else if (TelemetryData.ets2.truck.navigationEstimatedDistance == 0)
                        {
                            label_currentarrival.Text = "Ankunft:";
                            label_currentbestarrival.Text = "";
                        }
                    }
                    if (bestcurrentaveragespeed > 0 && currentaveragespeed > 0)
                    { // Average Calculations SHOW
                        DateTime dt_currentarrival = DateTime.Now.AddSeconds((((TelemetryData.ets2.truck.navigationEstimatedDistance / 1000) / currentaveragespeed) / 19) * 3600);                  // ADD CONSTANT (replace "19") [IMPORTANT]
                        TimeSpan ts_currentarrival = dt_currentarrival.Subtract(DateTime.Now);

                        DateTime dt_bestcurrentarrival = DateTime.Now.AddSeconds((((TelemetryData.ets2.truck.navigationEstimatedDistance / 1000) / bestcurrentaveragespeed) / 19) * 3600);          // ADD CONSTANT (replace "19") [IMPORTANT]
                        TimeSpan ts_bestcurrentarrival = dt_bestcurrentarrival.Subtract(DateTime.Now);
                        if (ts_currentarrival.TotalMinutes - ts_bestcurrentarrival.TotalMinutes > 60)
                        {// current arrival (color)
                            panel7.BackColor = Color.Brown;
                        }
                        else if (ts_currentarrival.TotalMinutes - ts_bestcurrentarrival.TotalMinutes > 30 && ts_currentarrival.TotalMinutes - ts_bestcurrentarrival.TotalMinutes < 60)
                        {
                            panel7.BackColor = Color.Goldenrod;
                        }
                        else
                        {
                            panel7.BackColor = Color.LimeGreen;
                        }

                        label_currentarrival.Text = String.Format("Ankunft ca.:      {0}", dt_currentarrival.ToString("HH:mm"));
                        label_currentarrival2.Text = String.Format("({0})", TimeSpanConvertToAvailableValuesOnly(ts_currentarrival));

                        label_currentbestarrival.Text = String.Format("{0}", dt_bestcurrentarrival.ToString("HH:mm"));
                        label_currentbestarrival2.Text = String.Format("({0})", TimeSpanConvertToAvailableValuesOnly(ts_bestcurrentarrival));

                        if (bestarrivalset == false)
                        {// best arrival (color)
                            DateTime dt_bestarrival = DateTime.Now.AddSeconds((((TelemetryData.ets2.truck.navigationEstimatedDistance / 1000) / bestcurrentaveragespeed) / 19) * 3600);          // ADD CONSTANT (replace "19") [IMPORTANT]
                            TimeSpan ts_bestarrival = dt_bestarrival.Subtract(DateTime.Now);
                            string bestarrivaltext = bestarrivaltext = String.Format("(+{0})", TimeSpanConvertToAvailableValuesOnly(TimeSpan.FromSeconds(ts_bestarrival.TotalSeconds * (-1))));
                            if (ts_bestarrival.TotalSeconds > 0)
                            {
                                bestarrivaltext = String.Format("(-{0})", TimeSpanConvertToAvailableValuesOnly(ts_bestarrival));
                            }
                            label_bestarrival.Text = String.Format("{0}", dt_bestarrival.ToString("HH:mm"));
                            label_bestarrival2.Text = bestarrivaltext;

                        }

                    }

                    if (TelemetryData.ets2.game.paused == false)
                    {// Not Paused Only

                        if (TelemetryData.ets2.job.cargo.id != "")
                        {// Contract-Only  
                            if (situation != "Contract")
                            {
                                timercounter = 0;
                                speedsummary = 0;
                                currentaveragespeed = 0;
                            }
                            if (TelemetryData.ets2.job.cargo.totalDamage > 0)
                            {// ProgressBar Damage
                                PictureBoxCustomProgressBar(pictureBox2_cargodamage, Color.White, TelemetryData.ets2.job.cargo.totalDamage * 100, Math.Round(TelemetryData.ets2.job.cargo.totalDamage, 2).ToString("p0"), "Microsoft Sans Serif", Brushes.Brown);
                            }

                            situation = "Contract";
                        }
                        else if (TelemetryData.ets2.job.cargo.id == "")
                        {
                            if (TelemetryData.ets2.truck.navigationEstimatedDistance > 0)
                            {// DestinationOrFreeDrive-Only
                                if (situation != "DestinationOrFreeDrive")
                                {
                                    timercounter = 0;
                                    speedsummary = 0;
                                    currentaveragespeed = 0;
                                }
                                situation = "DestinationOrFreeDrive";
                            }
                        }


                    }


                    // Pause label
                    if (TelemetryData.ets2.game.paused == false)
                    {
                        label1_paused.Text = "Verbunden";
                        label1_paused.BackColor = System.Drawing.Color.LimeGreen;
                    }
                    else if (TelemetryData.ets2.game.paused == true)
                    {
                        label1_paused.Text = "Spiel pausiert";
                        label1_paused.BackColor = System.Drawing.Color.Goldenrod;
                    }


                    // TimeScale
                    label2_timescale.Text = "Zeitskalierung: " + TelemetryData.ets2.game.scale.ToString();


                    // JobInfo
                    if (TelemetryData.ets2.job.cargo.id != "")
                    {
                        label5_jobinfo.Text = TelemetryData.ets2.job.cargo.name + "\n" + Math.Round(TelemetryData.ets2.job.cargo.totalMass / 1000, 1) + " t\n" + TelemetryData.ets2.job.income.ToString("c0") + " (" + Math.Round(Convert.ToDecimal(TelemetryData.ets2.job.income) / Convert.ToDecimal(TelemetryData.ets2.job.cargo.plannedDistanceKM), 2) + " €/km)";
                        label10_sourcedata.Text = TelemetryData.ets2.job.sourceCity + "\n" + TelemetryData.ets2.job.sourceCompany;
                        label11_destinationdata.Text = TelemetryData.ets2.job.destinationCity + "\n" + TelemetryData.ets2.job.destinationCompany;
                    }

                    // JobRemainingTime
                    if (TelemetryData.ets2.job.cargo.id != "")
                    {
                        TimeSpan ts_remainingtime = TimeSpan.FromSeconds(TelemetryData.ets2.job.remainingTime);
                        TimeSpan ts_nextpausetime = TimeSpan.FromSeconds(TelemetryData.ets2.game.nextRestStopTime);
                        TimeSpan ts_estimatedtime = TimeSpan.FromSeconds(TelemetryData.ets2.truck.navigationEstimatedTime);
                        TimeSpan ts_timebuffer = ts_remainingtime - ts_estimatedtime;
                        if (ts_nextpausetime < ts_estimatedtime)
                        {
                            double d = Math.Ceiling((ts_estimatedtime.TotalSeconds - ts_nextpausetime.TotalSeconds) / (11 * 3600));
                            ts_timebuffer = ts_remainingtime - (ts_estimatedtime.Add(TimeSpan.FromHours(d * 9)));

                        }

                        // remaining time (color)
                        if (ts_remainingtime.TotalSeconds > 0)
                        {
                            if (ts_remainingtime.TotalHours < 1)
                            {
                                label7_remainingtime.ForeColor = Color.Gold;
                            }
                            label7_remainingtime.ForeColor = Color.LimeGreen;
                        }
                        else
                        {
                            label7_remainingtime.ForeColor = Color.Brown;
                        }
                        label7_remainingtime.Text = "Restzeit: " + TimeSpanConvertToAvailableValuesOnly(ts_remainingtime);

                        // next pause time (color)
                        if (ts_nextpausetime.TotalSeconds > 0)
                        {
                            if (ts_nextpausetime.TotalHours < 1)
                            {
                                label8_nextpausetime.ForeColor = Color.Gold;
                            }
                            label8_nextpausetime.ForeColor = Color.LimeGreen;
                        }
                        else
                        {
                            label8_nextpausetime.ForeColor = Color.Brown;
                        }
                        label8_nextpausetime.Text = "Pause in: " + TimeSpanConvertToAvailableValuesOnly(ts_nextpausetime);

                        // time buffer (color and negate)
                        if (ts_timebuffer.TotalSeconds < 0)
                        {
                            label6_timebuffer.ForeColor = Color.Brown;
                            ts_timebuffer = TimeSpan.FromSeconds(ts_timebuffer.TotalSeconds * (-1));
                            label6_timebuffer.Text = "Zeitpuffer: -" + TimeSpanConvertToAvailableValuesOnly(ts_timebuffer);
                        }
                        else
                        {
                            if (ts_timebuffer.TotalHours < 2.5)
                            {
                                label6_timebuffer.BackColor = Color.Gold;
                            }
                            else
                            {
                                label6_timebuffer.BackColor = Color.LimeGreen;
                            }
                            label6_timebuffer.Text = "Zeitpuffer: " + TimeSpanConvertToAvailableValuesOnly(ts_timebuffer);
                        }

                    }


                    // ProgressBar Distance
                    int pb_distanceWIDTH = pictureBox1_distance.Width, pb_distanceHEIGHT = pictureBox1_distance.Height;
                    if (distancesummary <= TelemetryData.ets2.truck.navigationEstimatedDistance / 1000)
                    {
                        distancesummary = drivendistance + TelemetryData.ets2.truck.navigationEstimatedDistance / 1000;
                    }
                    else if (distancesummary > TelemetryData.ets2.truck.navigationEstimatedDistance / 1000)
                    {
                        drivendistance = distancesummary - TelemetryData.ets2.truck.navigationEstimatedDistance / 1000;
                        distancesummary = drivendistance + TelemetryData.ets2.truck.navigationEstimatedDistance / 1000;
                    }
                    double pb_distanceProgress = drivendistance / distancesummary;

                    if (TelemetryData.ets2.truck.navigationEstimatedDistance > 0)
                    {
                        string pb_distanceText = String.Format("{0} km   /   {1} km", Math.Round(drivendistance, 0), Math.Round(distancesummary, 0));
                        PictureBoxCustomProgressBar(pictureBox1_distance, Color.White, pb_distanceProgress * 100, pb_distanceText, "Microsoft Sans Serif", Brushes.LimeGreen);
                        label12_progresspercentage.Text = (pb_distanceProgress.ToString("p0"));
                        label13_remainingdistance.Text = "Noch " + Math.Round(TelemetryData.ets2.truck.navigationEstimatedDistance / 1000, 0).ToString() + " km";
                    }
                    else if (TelemetryData.ets2.truck.navigationEstimatedDistance == 0)
                    {
                        distancesummary = 0;
                        drivendistance = 0;
                        pb_distanceProgress = 0;
                    }


                    // vehicle info
                    string beaconStatus = "";
                    if (TelemetryData.ets2.truck.lightsBeaconOn == true)
                    {
                        beaconStatus = "eingeschaltet";
                    }
                    else
                    {
                        beaconStatus = "ausgeschaltet";
                    }
                    label_vehicleinformation.Text = String.Format("Rundumleuchte: {0}", beaconStatus);
                    label_vehicleinformation2.Text = String.Format("Ø Geschwindigkeit: {0} km/h", currentaveragespeed.ToString("n2"));
                    label_vehicleinformation3.Text = String.Format("Kraftstoffverbrauch: {0} l/100km", Math.Round(TelemetryData.ets2.truck.fuelAverageConsumption * 100, 2));


                    // ProgressBar fuel
                    string pb_fuelText = String.Format("{0} l / {1} l ({2} km)", Math.Round(TelemetryData.ets2.truck.fuel, 0), Math.Round(TelemetryData.ets2.truck.fuelCapacity, 0), Math.Round(TelemetryData.ets2.truck.fuelRange, 0));
                    if (TelemetryData.ets2.truck.fuelWarningOn)
                    {
                        PictureBoxCustomProgressBar(pictureBox3_fuel, Color.White, ((TelemetryData.ets2.truck.fuel / TelemetryData.ets2.truck.fuelCapacity) * 100), pb_fuelText, "Microsoft Sans Serif", Brushes.Brown);
                    }
                    else
                    {
                        PictureBoxCustomProgressBar(pictureBox3_fuel, Color.White, ((TelemetryData.ets2.truck.fuel / TelemetryData.ets2.truck.fuelCapacity * 100)), pb_fuelText, "Microsoft Sans Serif", Brushes.LimeGreen);
                    }

                }
                else if (TelemetryData.ets2.game.connected == false)
                {
                    label1_paused.Text = "Keine Verbindung zum Spiel";
                    label1_paused.BackColor = System.Drawing.Color.Brown;


                    label2_timescale.Text = "Zeitskalierung: -";
                }
            }
            else if (bTelemetryOnline == false)
            {
                label1_paused.Text = "Keine Verbindung zum Server";
                label2_timescale.Text = "Zeitskalierung: -";
                label1_paused.BackColor = System.Drawing.Color.Brown;
            }

        }

        private void button1_settings_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
        }

        void PictureBoxCustomProgressBar(PictureBox pb, Color colorBack, double dProgress, string sBarText, string sFont, Brush brushProgressColor)
        {// Method Custom ProgressBar with Picture Box
            if (WindowState != FormWindowState.Minimized)
            {
                try
                {
                    double pbUnit = pb.Width / 100;
                    Bitmap bmp;
                    Graphics graphics;
                    bmp = new Bitmap(pb.Width, pb.Height);
                    graphics = Graphics.FromImage(bmp);
                    graphics.Clear(colorBack);
                    graphics.FillRectangle(brushProgressColor, new Rectangle(0, 0, (int)(dProgress * pbUnit), pb.Height)); //(int)
                    StringFormat stringFormat = new StringFormat();
                    stringFormat.Alignment = StringAlignment.Center;
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
            if (ts.Days > 0)
            {
                return ((ts.Days * 24) + ts.Hours) + " Std. " + ts.Minutes + " Min.";
            }
            else
            {
                if (ts.Hours > 0)
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
                        numericUpDown_time1.Value = (numericUpDown_km.Value / 19) / numericUpDown_speed.Value;             // ADD CONSTANT (replace "19") [IMPORTANT]
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
                        numericUpDown_km.Value = 19 * (numericUpDown_time1.Value * numericUpDown_speed.Value); // ADD CONSTANT (replace "19") [IMPORTANT]
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
                        numericUpDown_km.Value = 19 * (numericUpDown_time1.Value * numericUpDown_speed.Value); // ADD CONSTANT (replace "19") [IMPORTANT]
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
                        numericUpDown_time1.Value = (numericUpDown_km.Value / 19) / numericUpDown_speed.Value;      // ADD CONSTANT (replace "19") [IMPORTANT]

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


        // Schedule planner
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
        private void button_CreateSchedule_Click(object sender, EventArgs e)
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
                Workshift newWorkshift = new Workshift(counter, start_dt, start_dt.AddHours(DriveTime + 0.75), start_dt.AddHours(DriveTime / 2), start_dt.AddHours((DriveTime / 2) + 0.75));
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
        }

        private void button_LoadDeleteSchedule_Click(object sender, EventArgs e)
        {
            if (scheduleLoaded == true)
            {// DELETE

                if (MessageBox.Show("Möchten Sie den aktuellen Schichtplan löschen?", "Warnung!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    scheduleLoaded = false;
                }
            }
            else
            {// LOAD (Get Json Data from file)

                if (openFileDialog_Schedule.ShowDialog() == DialogResult.OK)
                {
                    try
                    {// Check if FileFormat is correct
                        Stream stream = File.Open(openFileDialog_Schedule.FileName, FileMode.Open);

                        StreamReader sr = new StreamReader(stream);
                        string str = sr.ReadToEnd();
                        sr.Close();
                        JsonConvert.DeserializeObject<Workshift>(str);

                        if (listWorkshifts[listWorkshifts.Count].EndDate > DateTime.Now)
                        {// Check if Schedule is not oudtdated
                            scheduleLoaded = true;
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Die angegebene Datei hat ein falsches Format.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error); ;
                        scheduleLoaded = false;
                    }


                }
            }
        }
        private void button_LoadDeleteScheduleMenu_Click(object sender, EventArgs e)
        {// LOAD (Get Json Data from file)

            if (openFileDialog_Schedule.ShowDialog() == DialogResult.OK)
            {
                if (scheduleLoaded == true)
                {
                    if (MessageBox.Show("Möchte Sie fortfahren und damit den derzeitigen Zeitplan löschen?", "Warnung!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        scheduleLoaded = false;
                        try
                        {// Check if FileFormat is correct
                            FileStream fs = File.Open(openFileDialog_Schedule.FileName, FileMode.Open);

                            StreamReader sr = new StreamReader(fs);
                            string str = sr.ReadToEnd();
                            sr.Close();
                            listWorkshifts.Add(JsonConvert.DeserializeObject<Workshift>(str));


                            if (listWorkshifts[listWorkshifts.Count].EndDate > DateTime.Now)
                            {// Check if Schedule is not oudtdated
                                scheduleLoaded = true;
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Die angegebene Datei hat ein falsches Format.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error); ;
                            scheduleLoaded = false;
                        }
                    }
                }
                else
                {
                    try
                    {// Check if FileFormat is correct
                        FileStream fs = File.Open(saveFileDialog_Schedule.FileName, FileMode.Open);

                        StreamReader sr = new StreamReader(fs);
                        string str = sr.ReadToEnd();
                        sr.Close();
                        listWorkshifts.Add(JsonConvert.DeserializeObject<Workshift>(str));


                        if (listWorkshifts[listWorkshifts.Count].EndDate > DateTime.Now)
                        {// Check if Schedule is not oudtdated
                            scheduleLoaded = true;
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Die angegebene Datei hat ein falsches Format.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error); ;
                        scheduleLoaded = false;
                    }
                }

            }

        }


        //Save Schedule

        void SaveSchedule(object sender, EventArgs e)
        {
            if (scheduleLoaded == true)
            {
                saveFileDialog_Schedule.FileName = String.Format("TimeSchedule_{0} - {1}", listWorkshifts[0].StartDate.ToString("dd-MM-yyyy HHmm"), listWorkshifts[listWorkshifts.Count - 1].EndDate.ToString("dd-MM-yyyy HHmm"));
                if (saveFileDialog_Schedule.ShowDialog() == DialogResult.OK)
                {
                    string s = JsonConvert.SerializeObject(listWorkshifts);
                    File.WriteAllText(saveFileDialog_Schedule.FileName, s);
                }
            }
            else
            {
                MessageBox.Show("Derzeit ist kein Schichtplan geladen worden. Es konnte nichts gespeichert werden.", "Kein Schichtplan!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }



        // Set Location
        Point SetLocation(Point Location, int OffsetX, int OffsetY)
        {
            int x = Location.X;
            int y = Location.Y;
            x += OffsetX;
            y += OffsetY;

            return new Point(x, y);
        }


    }
}



