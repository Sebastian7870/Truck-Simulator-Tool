using System;
using System.IO;
using System.Threading.Tasks;
using Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Classes;
using Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Json;

namespace Truck_Simulator_Tool__WPF_.TruckSimulatorTool.StaticClasses
{
    public static class CalcData
    {
        #region "Variables"
        // Raw Data
        public enum game
        {
            unknown,
            ets2,
            ats
        };
        public static game _game = new game();

        public enum jobStatus
        {
            unknown,
            onJob,
            destination,
            freeDrive
        };
        private static jobStatus _jobStatus = new jobStatus();

        private static Rootobject_Telemetry _data { get; set; }
        public static Rootobject_Telemetry _Data
        {
            get
            {
                if (_data != null)
                {
                    return _data;
                }
                else
                {
                    return new Rootobject_Telemetry();
                }
            }
        }

        // Calculated Data
        private static bool isAllowedToUpdate;
        public static bool IsAllowedToUpdate
        {
            get { return isAllowedToUpdate; }
            set { isAllowedToUpdate = value; }
        }

        public static double timerCounter { get; set; }
        public static double timerInvervalFactor { get; set; }


        private static double speedSummary;
        public static double SpeedSummary
        {
            get { return speedSummary; }
        }


        private static double speedCurrentAverage;
        public static double SpeedCurrentAverage
        {
            get
            {
                if (timerCounter > 0 && SpeedSummary > 0)
                    speedCurrentAverage = SpeedSummary / (timerCounter * timerInvervalFactor);
                else
                    speedCurrentAverage = 0;
                return speedCurrentAverage;
            }
        }


        private static double speedCurrentBestAverage;
        public static double SpeedCurrentBestAverage
        {
            get
            {
                speedCurrentBestAverage = Unit.navigationDistanceC / ((double)_data.ets2.truck.navigationEstimatedTime / 3600);
                return speedCurrentBestAverage;
            }
        }


        private static double distanceDriven;
        public static double DistanceDriven
        {
            get
            {
                if (ContractHelper.ContractJson.OdometerStartValue <= Unit.currentOdometer && ContractHelper.ContractJson.OdometerStartValue > (Unit.currentOdometer - 25000))
                    distanceDriven = Unit.currentOdometer - ContractHelper.ContractJson.OdometerStartValue;
                else
                    distanceDriven = 0;
                return distanceDriven;
            }
        }


        private static double distanceSummary;
        public static double DistanceSummary
        {
            get
            {
                distanceSummary = distanceDriven + Unit.navigationDistanceC;
                return distanceSummary;
            }
        }


        // Time Values
        private static bool hasBestArrival;


        private static DateTime dt_currentArrival;
        public static DateTime dt_CurrentArrival
        {
            get
            {
                if (_data.ets2.truck.navigationEstimatedDistance != 0 && SpeedCurrentAverage != 0)
                    dt_currentArrival = DateTime.Now.AddSeconds(Unit.navigationDistanceC / SpeedCurrentAverage / SettingsHelper.SettingsJson.TimeScaleValue * 3600);
                else
                    dt_currentArrival = DateTime.MinValue;
                return dt_currentArrival;
            }
        }


        private static TimeSpan ts_currentArrival;
        public static TimeSpan ts_CurrentArrival
        {
            get
            {
                if (dt_CurrentArrival > DateTime.MinValue.AddYears(1))
                    ts_currentArrival = dt_currentArrival.Subtract(DateTime.Now);
                else
                    ts_currentArrival = TimeSpan.FromSeconds(0);
                return ts_currentArrival;
            }
        }


        private static DateTime dt_currentBestArrival;
        public static DateTime dt_CurrentBestArrival
        {
            get
            {
                if (_data.ets2.truck.navigationEstimatedDistance != 0)
                    dt_currentBestArrival = DateTime.Now.AddSeconds(Unit.navigationDistanceC / SpeedCurrentBestAverage / SettingsHelper.SettingsJson.TimeScaleValue * 3600);
                else
                    dt_currentBestArrival = DateTime.MinValue;
                return dt_currentBestArrival;
            }
        }


        private static TimeSpan ts_currentBestArrival;
        public static TimeSpan ts_CurrentBestArrival
        {
            get
            {
                if (dt_CurrentBestArrival > DateTime.MinValue.AddYears(1))
                    ts_currentBestArrival = dt_currentBestArrival.Subtract(DateTime.Now);
                else
                    ts_currentBestArrival = TimeSpan.FromSeconds(0);
                return ts_currentBestArrival;
            }
        }

        private static DateTime dt_bestArrivalStart;
        public static DateTime dt_BestArrivalStart
        {
            get { return dt_bestArrivalStart; }
        }

        private static DateTime dt_bestArrival;
        public static DateTime dt_BestArrival
        {
            get { return dt_bestArrival; }
        }


        private static TimeSpan ts_bestArrival;
        public static TimeSpan ts_BestArrival
        {
            get
            {
                if (_data.ets2.truck.navigationEstimatedDistance != 0 && counter >= 3)
                {
                    if (!hasBestArrival)
                    {
                        ts_bestArrival = TimeSpan.FromSeconds((int)Unit.navigationDistanceC / CalcData.SpeedCurrentBestAverage / SettingsHelper.SettingsJson.TimeScaleValue * 3600);
                        dt_bestArrivalStart = DateTime.Now;
                        dt_bestArrival = DateTime.Now.Add(ts_bestArrival);
                        hasBestArrival = true;
                    }
                    ts_bestArrival = dt_BestArrival - DateTime.Now;
                    return ts_bestArrival;
                }
                else
                {
                    return TimeSpan.FromSeconds(0);
                }
            }
        }


        private static TimeSpan ts_timebuffer;
        public static TimeSpan ts_Timebuffer
        {
            get
            {
                if (_data.ets2.job.cargo.id != string.Empty)
                {
                    ts_timebuffer = ts_RemainingTime - ts_EstimatedTime;
                    if (ts_NextPauseTime < ts_EstimatedTime)
                    {
                        switch (_game)
                        {
                            case game.ets2:
                                ReturnTimebuffer(11, 9);
                                break;
                            case game.ats:
                                ReturnTimebuffer(14, 10);
                                break;
                            case game.unknown:
                                ReturnTimebuffer(11, 9);
                                break;
                        }
                    }
                }
                return ts_timebuffer;
            }
        }


        private static TimeSpan ts_nextPauseTime;
        public static TimeSpan ts_NextPauseTime
        {
            get { return ts_nextPauseTime = TimeSpan.FromSeconds(_data.ets2.game.nextRestStopTime); }
        }


        private static TimeSpan ts_remainingTime;
        public static TimeSpan ts_RemainingTime
        {
            get { return ts_remainingTime = TimeSpan.FromSeconds(_data.ets2.job.remainingTime); }
        }


        private static TimeSpan ts_estimatedTime;
        public static TimeSpan ts_EstimatedTime
        {
            get { return ts_estimatedTime = TimeSpan.FromSeconds(_data.ets2.truck.navigationEstimatedTime); }
        }
        #endregion

        public static async void SetGameValues(Rootobject_Telemetry data)
        {
            _data = data;

            switch (data.ets2.game.gameID)
            {
                case "eut2":
                    {
                        _game = game.ets2;
                        break;
                    }
                case "ats":
                    {
                        _game = game.ats;
                        break;
                    }
                default:
                    {
                        _game = game.unknown;
                        break;
                    }
            }

            //timerCounter has to be set from outside (from SCSSdkTelemetry)
            if (!data.ets2.game.paused && Math.Abs(data.ets2.truck.speed) > 5)
                speedSummary += Unit.truckSpeed * timerInvervalFactor;

            #region "checkJobStatus"
            if (ContractHelper.ContractJson.LastProfile != Path.GetFileName(data.ets2.game.lastProfile))
            {
                isAllowedToUpdate = false;
                await jobStateChanged();
            }
            ContractHelper.ContractJson.LastProfile = Path.GetFileName(data.ets2.game.lastProfile);

            if (data.ets2.job.cargo.id == string.Empty)
            {
                if (isAllowedToUpdate)
                {// else: profile change
                    try
                    {
                        ContractHelper.AutoDelete();
                    }
                    catch
                    {
                        //nothing to delete (no contract) => /!\ N O  E R R O R /!\
                    }
                }
            }

            if (!data.ets2.game.paused)
            {
                if (counter >= 3)
                {
                    if (data.ets2.job.cargo.id != string.Empty)
                    {//OnJob
                        try
                        {
                            if (isAllowedToUpdate)
                                ContractHelper.TryAutoSave();
                        }
                        catch
                        {
                            //Todo: LogEntry
                        }

                        if (_jobStatus != jobStatus.onJob)
                        {
                            await jobStateChanged();
                            _jobStatus = jobStatus.onJob;
                        }
                    }
                    else
                    {
                        if (data.ets2.truck.navigationEstimatedDistance != 0)
                        {//Destination
                            if (_jobStatus != jobStatus.destination)
                            {
                                await jobStateChanged();
                                _jobStatus = jobStatus.destination;
                            }
                        }
                        else
                        {//FreeDrive
                            if (_jobStatus != jobStatus.freeDrive)
                            {
                                await jobStateChanged();
                                _jobStatus = jobStatus.freeDrive;
                            }
                        }
                    }
                }
            }

            if (CalcData.IsAllowedToUpdate)
            {
                ContractJson contractJson = new ContractJson();
                contractJson.Game = data.ets2.game.gameID;
                contractJson.LastProfile = Path.GetFileName(data.ets2.game.lastProfile);
                contractJson.CitySource = data.ets2.job.sourceCity;
                contractJson.CityDestination = data.ets2.job.destinationCity;
                contractJson.Income = data.ets2.job.income;
                contractJson.Mass = data.ets2.job.cargo.totalMass;
                contractJson.OdometerStartValue = ContractHelper.ContractJson.OdometerStartValue;
                contractJson.timerCounter = CalcData.timerCounter;
                contractJson.speedSummary = CalcData.SpeedSummary;
                contractJson.distanceDriven = CalcData.DistanceDriven;
                contractJson.distanceSummary = CalcData.DistanceSummary;
                ContractHelper.ContractJson = contractJson;

                if (!data.ets2.game.paused && counter >= 3)
                {
                    ContractHelper.AutoLoadIfStartup();
                }
            }
            #endregion
        }

        private static int counter = 0;
        private static async Task jobStateChanged()
        {
            counter = 0;
            await Task.Delay(300);

            CalcData.ResetValues(true);
            ContractHelper.ResetValues();
            ContractHelper.ContractJson.OdometerStartValue = Unit.currentOdometer;

            counter = 10; // reset for eg. currentBestArrival
        }


        public static void ResetValues(bool resetSpeed)
        {

            if (resetSpeed)
            {
                timerCounter = 0;
                speedSummary = 0;
            }
            distanceDriven = 0;
            distanceSummary = 0;
            speedCurrentAverage = 0;
            speedCurrentBestAverage = 0;

            hasBestArrival = false;
            ts_currentBestArrival = TimeSpan.FromSeconds(0);
            dt_currentBestArrival = DateTime.MinValue;
            ts_currentArrival = TimeSpan.FromSeconds(0);
            dt_currentArrival = DateTime.MinValue;
            ts_bestArrival = TimeSpan.FromSeconds(0);
            dt_bestArrival = DateTime.MinValue;
            dt_bestArrivalStart = DateTime.MinValue;
            ts_nextPauseTime = TimeSpan.FromSeconds(0);
            ts_remainingTime = TimeSpan.FromSeconds(0);
            ts_timebuffer = TimeSpan.FromSeconds(0);
        }
        public static void ResetCurrentAverageSpeed()
        {
            timerCounter = 0;
            speedSummary = 0;
            speedCurrentAverage = 0;
            ContractHelper.ContractJson.timerCounter = 0;
            ContractHelper.ContractJson.speedSummary = 0;
        }
        public static void ResetBestArrival()
        {
            hasBestArrival = false;
        }


        public static void LoadValues(Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Json.ContractJson contractJson)
        {
            timerCounter = contractJson.timerCounter;
            speedSummary = contractJson.speedSummary;
            distanceDriven = contractJson.distanceDriven;
            distanceSummary = contractJson.distanceSummary;
        }

        private static TimeSpan ReturnTimebuffer(int driveTime, int sleepTime) //-> driveTime [ETS2 / ATS] => [11h / 14h] -+- -+- -+- sleepTime [ETS2 / ATS] => [9h / 10h]
        {
            double d = Math.Ceiling((ts_EstimatedTime.TotalSeconds - ts_NextPauseTime.TotalSeconds) / (driveTime * 3600));
            return ts_RemainingTime - (ts_EstimatedTime.Add(TimeSpan.FromHours(d * sleepTime)));
        }
    }
}