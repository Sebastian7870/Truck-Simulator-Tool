using SCSSdkClient.Object;
using System.Globalization;
using Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Methods;

namespace Truck_Simulator_Tool__WPF_.TruckSimulatorTool.StaticClasses
{
    public static class GameValues
    {
        public static string UDistance = "km";
        public static string UCurrency = "€";
        public static string UMass = "t";
        public static string UFluid = "l";
        public static string UMoneyDistance = "€/km";
        public static string UAverageFuelConsumption = "l/100km";
        public static CultureInfo UCultureInfo = new CultureInfo("de-DE");

        public static double navigationDistance;
        public static double plannedDistanceKM;
        public static double truckSpeed;
        public static double jobInfoMass;
        public static double fuelCurrent;
        public static double fuelCapacity;
        public static double fuelRange;
        public static string ingameTime;

        public static void SetETS2Units(SCSTelemetry data)
        {
            SetETS2UnitsText();
            SetETS2UnitsValues(data);
        }

        private static void SetETS2UnitsText()
        {
            UDistance = "km";
            UCurrency = "€";
            UMass = "t";
            UFluid = "l";
            UMoneyDistance = "€/km";
            UAverageFuelConsumption = "l/100km";
            UCultureInfo = new CultureInfo("de-DE");
        }
        private static void SetETS2UnitsValues(SCSTelemetry data)
        {
            navigationDistance = data.NavigationValues.NavigationDistance / 1000;
            plannedDistanceKM = data.JobValues.PlannedDistanceKm;
            truckSpeed = data.TruckValues.CurrentValues.DashboardValues.Speed.Kph;
            jobInfoMass = data.JobValues.CargoValues.Mass / 1000;
            fuelCurrent = data.TruckValues.CurrentValues.DashboardValues.FuelValue.Amount;
            fuelCapacity = data.TruckValues.ConstantsValues.CapacityValues.Fuel;
            fuelRange = data.TruckValues.CurrentValues.DashboardValues.FuelValue.Range;
            ingameTime = $"{data.CommonValues.GameTime.Date.ToString("ddd H:mm", UCultureInfo)}";
        }


        public static void SetATSUnits(SCSTelemetry data)
        {
            SetATSUnitsText();
            SetATSUnitsValues(data);
        }

        private static void SetATSUnitsText()
        {
            UDistance = "mi";
            UCurrency = "$";
            UMass = "lb";
            UFluid = "gal";
            UMoneyDistance = "$/mi";
            UAverageFuelConsumption = "mpg";
            UCultureInfo = new CultureInfo("en-US");
        }
        private static void SetATSUnitsValues(SCSTelemetry data)
        {
            navigationDistance = ConverterMethods.ConvertKMtoMI(data.NavigationValues.NavigationDistance / 1000);
            plannedDistanceKM = ConverterMethods.ConvertKMtoMI(data.JobValues.PlannedDistanceKm);
            truckSpeed = data.TruckValues.CurrentValues.DashboardValues.Speed.Mph;
            jobInfoMass = ConverterMethods.ConvertTtoLB(data.JobValues.CargoValues.Mass);
            fuelCurrent = ConverterMethods.ConvertLtoGAL(data.TruckValues.CurrentValues.DashboardValues.FuelValue.Amount);
            fuelCapacity = ConverterMethods.ConvertLtoGAL(data.TruckValues.ConstantsValues.CapacityValues.Fuel);
            fuelRange = ConverterMethods.ConvertLtoGAL(data.TruckValues.CurrentValues.DashboardValues.FuelValue.Range);
            ingameTime = $"{data.CommonValues.GameTime.Date.ToString("ddd h:mm", UCultureInfo)}";
        }
    }
}
