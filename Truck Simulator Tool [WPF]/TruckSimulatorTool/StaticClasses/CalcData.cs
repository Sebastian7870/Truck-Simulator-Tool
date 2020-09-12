using System;
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

        private static Rootobject_Telemetry _data { get; set; }
        public static Rootobject_Telemetry _Data
        {
            get
            {
                if (_data != null)
                    return _data;
                else
                    return new Rootobject_Telemetry();
            }
        }

        // Calculated Data
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
            get { return speedCurrentAverage; }
        }

        private static double speedCurrentBestAverage;
        public static double SpeedCurrentBestAverage
        {
            get { return speedCurrentBestAverage; }
        }

        private static double distanceDriven;
        public static double DistanceDriven
        {
            get { return distanceDriven; }
        }

        private static double distanceSummary;
        public static double DistanceSummary
        {
            get { return distanceSummary; }
        }

        // Time Values
        private static bool hasBestArrival;
        public static bool HasBestArrival
        {
            get { return hasBestArrival; }
            set { hasBestArrival = value; }
        }

        private static DateTime dt_currenArrival;
        public static DateTime dt_CurrentArrival
        {
            get { return dt_currenArrival; }
        }

        private static TimeSpan ts_currentArrival;
        public static TimeSpan ts_CurrentArrival
        {
            get { return ts_currentArrival; }
        }

        private static DateTime dt_currentBestArrival;
        public static DateTime dt_CurrentBestArrival
        {
            get { return dt_currentBestArrival; }
        }

        private static TimeSpan ts_currentBestArrival;
        public static TimeSpan ts_CurrentBestArrival
        {
            get { return ts_currentBestArrival; }
        }

        private static DateTime dt_bestArrival;
        public static DateTime dt_BestArrival
        {
            get { return dt_bestArrival; }
        }

        private static TimeSpan ts_bestArrival;
        public static TimeSpan ts_BestArrival
        {
            get { return ts_bestArrival; }
        }

        private static TimeSpan ts_timebuffer;
        public static TimeSpan ts_Timebuffer
        {
            get { return ts_timebuffer; }
        }

        private static TimeSpan ts_nextPauseTime;
        public static TimeSpan ts_NextPauseTime
        {
            get { return ts_nextPauseTime; }
        }

        private static TimeSpan ts_remainingTime;
        public static TimeSpan ts_RemainingTime
        {
            get { return ts_remainingTime; }
        }

        private static TimeSpan ts_estimatedTime;
        public static TimeSpan ts_EstimatedTime
        {
            get { return ts_estimatedTime; }
        }
        #endregion

        public static void SetGameValues(Rootobject_Telemetry data)
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

            if (timerCounter > 0 && speedSummary > 0)
                speedCurrentAverage = speedSummary / (timerCounter * timerInvervalFactor);
            else
                speedCurrentAverage = 0;

            speedCurrentBestAverage = Unit.navigationDistanceC / ((double)data.ets2.truck.navigationEstimatedTime / 3600);
            if (data.ets2.truck.navigationEstimatedDistance != 0)
            {
                distanceDriven = Unit.currentOdometer - ContractHelper.ContractJson.OdometerStartValue;
                distanceSummary = distanceDriven + Unit.navigationDistanceC;

                //CurrentBestArrival
                dt_currentBestArrival = DateTime.Now.AddSeconds(Unit.navigationDistanceC / CalcData.speedCurrentBestAverage / SettingsHelper.SettingsJson.TimeScaleValue * 3600);
                ts_currentBestArrival = dt_currentBestArrival.Subtract(DateTime.Now);

                //BestArrival
                if (!CalcData.hasBestArrival)
                {
                    ts_bestArrival = TimeSpan.FromSeconds((int)Unit.navigationDistanceC / CalcData.SpeedCurrentBestAverage / SettingsHelper.SettingsJson.TimeScaleValue * 3600);
                    dt_bestArrival = DateTime.Now.Add(ts_bestArrival);
                    CalcData.hasBestArrival = true;
                }
                ts_bestArrival = CalcData.dt_BestArrival - DateTime.Now;

                //CurrentArrival
                if (CalcData.SpeedCurrentAverage > 0)
                {
                    dt_currenArrival = DateTime.Now.AddSeconds(Unit.navigationDistanceC / CalcData.SpeedCurrentAverage / SettingsHelper.SettingsJson.TimeScaleValue * 3600);
                    ts_currentArrival = dt_currenArrival.Subtract(DateTime.Now);
                }

                //nextPauseTime
                ts_nextPauseTime = TimeSpan.FromSeconds(data.ets2.game.nextRestStopTime);

                //remainingTime
                ts_remainingTime = TimeSpan.FromSeconds(data.ets2.job.remainingTime);

                //estimatedTime
                ts_estimatedTime = TimeSpan.FromSeconds(data.ets2.truck.navigationEstimatedTime);

                //timebuffer
                if (data.ets2.job.cargo.id != string.Empty)
                {
                    ts_timebuffer = CalcData.ts_RemainingTime - ts_estimatedTime;
                    if (CalcData.ts_NextPauseTime < ts_estimatedTime)
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
                    else
                    {
                        ts_timebuffer = TimeSpan.FromSeconds(0);
                    }
                }
            }
            else
            {
                ResetValues(false);
                ContractHelper.ContractJson.OdometerStartValue = Unit.currentOdometer;
            }
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
            dt_currenArrival = DateTime.MinValue;
            ts_bestArrival = TimeSpan.FromSeconds(0);
            dt_bestArrival = DateTime.MinValue;
            ts_nextPauseTime = TimeSpan.FromSeconds(0);
            ts_remainingTime = TimeSpan.FromSeconds(0);
            ts_timebuffer = TimeSpan.FromSeconds(0);
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