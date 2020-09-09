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
        public static double fuelCurrent { get; set; }
        public static double fuelCapacity { get; set; }
        public static double fuelRange { get; set; }
        public static string ingameTime { get; set; }

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

        #endregion

        public static void SetGameValues(SCSTelemetry data)
        {// "C" behin value means "Converted" (=> no original RawDataValues from TelemetrySDK)
            #region "RawData_SetAndConvert"

            navigationDistanceC = data.NavigationValues.NavigationDistance / 1000;
            plannedDistanceKM = data.JobValues.PlannedDistanceKm;
            truckSpeed = Math.Abs(data.TruckValues.CurrentValues.DashboardValues.Speed.Kph);
            jobInfoMassC = data.JobValues.CargoValues.Mass / 1000;
            fuelCurrent = data.TruckValues.CurrentValues.DashboardValues.FuelValue.Amount;
            fuelCapacity = data.TruckValues.ConstantsValues.CapacityValues.Fuel;
            fuelRange = data.TruckValues.CurrentValues.DashboardValues.FuelValue.Range;
            ingameTime = $"{data.CommonValues.GameTime.Date.ToString("ddd H:mm", Unit.UCultureInfo)}";

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
