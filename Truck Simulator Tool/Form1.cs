using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Truck_Simulator_Tool
{
    public partial class Form1 : Form // main form
    {
        // Variables Start

        bool bTelemetryOnline = false;
        Rootobject TelemetryData = new Rootobject();
        int timercounter = 0;
        string situation = "None";
        double currentaveragespeed = 0;
        double bestcurrentaveragespeed = 0;
        double speedsummary = 0;
        double distancesummary = 0;
        double drivendistance = 0;

        // Variables End

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1_calculate.Start();
        }


        async Task UpdateTelemetry()
        {
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


        private async void timer1_calculate_Tick(object sender, EventArgs e)
        {
            await UpdateTelemetry();
            label14_datetimetime.Text = DateTime.Now.ToLongTimeString();
            label15_datetimedate.Text = DateTimeFormatInfo.CurrentInfo.GetDayName(DateTime.Now.DayOfWeek) + "\n" + DateTime.Now.ToShortDateString();



            // Telemetry online
            if (bTelemetryOnline == true)
            {//Telemetry Online
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
                            label3_currentarrival.Text = "Derzeitige Ankunft:";
                            label4_currentbestarrival.Text = "";
                        }
                    }
                    if (bestcurrentaveragespeed > 0 && currentaveragespeed > 0)
                    { // Average Calculations SHOW
                        DateTime dt_currentarrival = DateTime.Now.AddSeconds((((TelemetryData.ets2.truck.navigationEstimatedDistance / 1000) / currentaveragespeed) / 19) * 3600);                  // ADD CONSTANT (replace "19") [IMPORTANT]
                        TimeSpan ts_currentarrival = dt_currentarrival.Subtract(DateTime.Now);

                        DateTime dt_bestcurrentarrival = DateTime.Now.AddSeconds((((TelemetryData.ets2.truck.navigationEstimatedDistance / 1000) / bestcurrentaveragespeed) / 19) * 3600);          // ADD CONSTANT (replace "19") [IMPORTANT]
                        TimeSpan ts_bestcurrentarrival = dt_bestcurrentarrival.Subtract(DateTime.Now);
                        if (ts_currentarrival.TotalMinutes - ts_bestcurrentarrival.TotalMinutes > 60)
                        {
                            label3_currentarrival.ForeColor = Color.Brown;
                        }
                        else if (ts_currentarrival.TotalMinutes - ts_bestcurrentarrival.TotalMinutes > 30 && ts_currentarrival.TotalMinutes - ts_bestcurrentarrival.TotalMinutes < 60)
                        {
                            label3_currentarrival.ForeColor = Color.Goldenrod;
                        }
                        else
                        {
                            label3_currentarrival.ForeColor = Color.LimeGreen;
                        }

                        TimeSpanConvert("", label4_currentbestarrival, ts_bestcurrentarrival);
                        label4_currentbestarrival.Text += "    (" + dt_bestcurrentarrival.ToLongTimeString() + ")" + " " + Math.Round(bestcurrentaveragespeed, 1) + " km/h";

                        TimeSpanConvert("Derzeitige Ankunft: ", label3_currentarrival, ts_currentarrival);
                        label3_currentarrival.Text += "    (" + dt_currentarrival.ToLongTimeString() + ")" + " " + Math.Round(currentaveragespeed, 1) + " km/h";
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
                                if (currentaveragespeed > 0)
                                {
                                    DateTime dt_currentarrival = DateTime.Now.AddSeconds((((TelemetryData.ets2.truck.navigationEstimatedDistance / 1000) / currentaveragespeed) / 19) * 3600);                  // ADD CONSTANT (replace "19") [IMPORTANT]
                                    TimeSpan ts_currentarrival = dt_currentarrival.Subtract(DateTime.Now);
                                    label3_currentarrival.Text = "Derzeitige Ankunft: " + ((ts_currentarrival.Days * 24) + ts_currentarrival.Hours) + "h " + ts_currentarrival.Minutes + "m " + ts_currentarrival.Seconds + "s " + "      (" + dt_currentarrival.ToLongTimeString() + ")" + " " + Math.Round(currentaveragespeed, 1) + "km/h";
                                }
                                situation = "DestinationOrFreeDrive";
                            }
                        }


                    }


                    // Pause label
                    if (TelemetryData.ets2.game.paused == false)
                    {
                        label1_paused.Text = "Verbunden!";
                        label1_paused.BackColor = System.Drawing.Color.LimeGreen;
                    }
                    else if (TelemetryData.ets2.game.paused == true)
                    {
                        label1_paused.Text = "Spiel pausiert!";
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
                        label8_nextpausetime.Text = "Ruhezeit: " + TimeSpanConvertToAvailableValuesOnly(ts_nextpausetime);
                        
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
                                label6_timebuffer.ForeColor = Color.Gold;
                            }
                            else
                            {
                                label6_timebuffer.ForeColor = Color.LimeGreen;
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
                    label_vehicleinformation.Text = String.Format("Rundumleuchte: {0}\nKraftstoffverbrauch: {1} l/100km", beaconStatus, Math.Round(TelemetryData.ets2.truck.fuelAverageConsumption * 100, 2));


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
                    label1_paused.Text = "Keine Verbindung zum Spiel!";
                    label1_paused.BackColor = System.Drawing.Color.Brown;


                    label2_timescale.Text = "Zeitskalierung: -";
                }
            }
            else if (bTelemetryOnline == false)
            {
                label1_paused.Text = "Keine Verbindung zum Server!";
                label2_timescale.Text = "Zeitskalierung: -";
                label1_paused.BackColor = System.Drawing.Color.Brown;
            }

        }

        private void button1_settings_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
        }

        void TimeSpanConvert(string str, System.Windows.Forms.Label label, TimeSpan ts)
        {// Method TimeSpanConvert (eg. currentarrival)
            if (ts.Days > 0)
            {
                label.Text = str + (ts.Hours + (ts.Days * 24)) + " Std. ";
                if (ts.Minutes > 9)
                {
                    label.Text += ts.Minutes + " Min.";
                }
                else
                {
                    label.Text += "  " + ts.Minutes + " Min.";
                }
            }
            else
            {
                if (ts.Hours > 9)
                {
                    label.Text = str + ts.Hours + " Std. ";
                }
                else
                {
                    label.Text = str + "  " + ts.Hours + " Std. ";
                }
                if (ts.Minutes > 9)
                {
                    label.Text += ts.Minutes + " Min.";
                }
                else
                {
                    label.Text += "  " + ts.Minutes + " Min.";
                }

            }

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
    }

}



