using SCSSdkClient;
using SCSSdkClient.Object;
using System;
using Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Classes;

namespace Truck_Simulator_Tool__WPF_.TruckSimulatorTool.StaticClasses
{
    public static class CalcData
    {
        #region "Variables"
        public static double navigationDistanceC { get; set; }
        public static double plannedDistanceKM { get; set; }
        public static double truckSpeed { get; set; }
        public static double jobInfoMassC { get; set; }
        public static double fuelAverageConsumptionC { get; set; }
        public static double fuelCurrent { get; set; }
        public static double fuelCapacity { get; set; }
        public static double fuelRange { get; set; }
        public static string ingameTime { get; set; }
        public static float currentOdometer { get; set; }

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
            set { ts_timebuffer = value; }
        }

        private static TimeSpan ts_nextPauseTime;
        public static TimeSpan ts_NextPauseTime
        {
            get { return ts_nextPauseTime; }
            set { ts_nextPauseTime = value; }
        }

        private static TimeSpan ts_remainingTime;
        public static TimeSpan ts_RemainingTime
        {
            get { return ts_remainingTime; }
            set { ts_remainingTime = value; }
        }
        #endregion

        public static void SetGameValues(SCSTelemetry data)
        {// "C" behin value means "Converted" (=> no original RawDataValues from TelemetrySDK)
            #region "RawData_SetAndConvert"
            navigationDistanceC = data.NavigationValues.NavigationDistance / 1000;
            plannedDistanceKM = data.JobValues.PlannedDistanceKm;
            truckSpeed = Math.Abs(data.TruckValues.CurrentValues.DashboardValues.Speed.Kph);
            jobInfoMassC = data.JobValues.CargoValues.Mass / 1000;
            fuelAverageConsumptionC = data.TruckValues.CurrentValues.DashboardValues.FuelValue.AverageConsumption / 100;
            fuelCurrent = data.TruckValues.CurrentValues.DashboardValues.FuelValue.Amount;
            fuelCapacity = data.TruckValues.ConstantsValues.CapacityValues.Fuel;
            fuelRange = data.TruckValues.CurrentValues.DashboardValues.FuelValue.Range;
            ingameTime = $"{data.CommonValues.GameTime.Date.ToString("ddd H:mm", Unit.UCultureInfo)}";
            currentOdometer = data.TruckValues.CurrentValues.DashboardValues.Odometer;

            //do not change order: above values will be converted in Unit class
            switch (data.Game)
            {
                case SCSGame.Ets2:
                    {
                        Unit.SetETS2Units();
                        break;
                    }
                case SCSGame.Ats:
                    {
                        Unit.SetATSUnits();
                        break;
                    }
                default:
                    {
                        Unit.SetETS2Units();
                        break;
                    }
            }
            #endregion

            #region "Calculations"
            //timerCounter has to be set from outside (from SCSSdkTelemetry)
            if (!data.Paused && Math.Abs(data.TruckValues.CurrentValues.DashboardValues.Speed.Kph) > 5)
                speedSummary += truckSpeed * timerInvervalFactor;

            if (timerCounter > 0 && speedSummary > 0)
                speedCurrentAverage = speedSummary / (timerCounter * timerInvervalFactor);
            else
                speedCurrentAverage = 0;

            speedCurrentBestAverage = navigationDistanceC / (data.NavigationValues.NavigationTime / 3600);
            if (data.NavigationValues.NavigationDistance != 0)
            {
                distanceDriven = data.TruckValues.CurrentValues.DashboardValues.Odometer - ContractHelper.ContractJson.OdometerStartValue;
                distanceSummary = distanceDriven + navigationDistanceC;

                //CurrentBestArrival
                dt_currentBestArrival = DateTime.Now.AddSeconds(CalcData.navigationDistanceC / CalcData.speedCurrentBestAverage / SettingsHelper.SettingsJson.TimeScaleValue * 3600);
                ts_currentBestArrival = dt_currentBestArrival.Subtract(DateTime.Now);

                //BestArrival
                if (!CalcData.hasBestArrival)
                {
                    ts_bestArrival = TimeSpan.FromSeconds((int)CalcData.navigationDistanceC / CalcData.SpeedCurrentBestAverage / SettingsHelper.SettingsJson.TimeScaleValue * 3600);
                    dt_bestArrival = DateTime.Now.Add(ts_bestArrival);
                }
                ts_bestArrival = CalcData.dt_BestArrival - DateTime.Now;

                //CurrentArrival
                if (CalcData.SpeedCurrentAverage > 0)
                {
                    dt_currenArrival = DateTime.Now.AddSeconds(CalcData.navigationDistanceC / CalcData.SpeedCurrentAverage / SettingsHelper.SettingsJson.TimeScaleValue * 3600);
                    ts_currentArrival = dt_currenArrival.Subtract(DateTime.Now);
                }

                //nextPauseTime
                ts_nextPauseTime = TimeSpan.FromMinutes(data.CommonValues.NextRestStop.Value);

                //remainingTime
                ts_remainingTime = TimeSpan.FromMinutes(data.JobValues.RemainingDeliveryTime.Value);

                //timebuffer
                if (data.SpecialEventsValues.OnJob)
                {
                    TimeSpan ts_estimatedTime = TimeSpan.FromSeconds(data.NavigationValues.NavigationTime);
                    ts_timebuffer = CalcData.ts_RemainingTime - ts_estimatedTime;
                    if (CalcData.ts_NextPauseTime < ts_estimatedTime)
                    {
                        if (data.Game == SCSGame.Ets2)
                        {
                            double d = Math.Ceiling((ts_estimatedTime.TotalSeconds - CalcData.ts_NextPauseTime.TotalSeconds) / (11 * 3600));
                            ts_timebuffer = CalcData.ts_RemainingTime - (ts_estimatedTime.Add(TimeSpan.FromHours(d * 9)));
                        }
                        else
                        {
                            double d = Math.Ceiling((ts_estimatedTime.TotalSeconds - CalcData.ts_NextPauseTime.TotalSeconds) / (14 * 3600));
                            ts_timebuffer = CalcData.ts_RemainingTime - (ts_estimatedTime.Add(TimeSpan.FromHours(d * 10)));
                        }
                    }
                }
                else
                {
                    ts_timebuffer = TimeSpan.FromSeconds(0);
                }
            }
            else
            {
                ResetValues(false);
                ContractHelper.ContractJson.OdometerStartValue = data.TruckValues.CurrentValues.DashboardValues.Odometer;
            }
            #endregion
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
    }
}
