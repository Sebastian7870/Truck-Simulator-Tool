using System.Globalization;
using Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Methods;
using Truck_Simulator_Tool__WPF_.TruckSimulatorTool.StaticClasses;

namespace Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Classes
{
    public static class Unit
    {
        #region "Variables"
        private static string _USpeed;
        public static string USpeed
        {
            get
            {
                if (_USpeed != null)
                    return _USpeed;
                else
                    return "km/h";
            }
        }

        private static string _UDistance;
        public static string UDistance
        {
            get
            {
                if (_UDistance != null)
                    return _UDistance;
                else
                    return "km";
            }
        }

        private static string _UCurrency;
        public static string UCurrency
        {
            get
            {
                if (_UCurrency != null)
                    return _UCurrency;
                else
                    return "€";
            }
        }

        private static string _UMass;
        public static string UMass
        {
            get
            {
                if (_UMass != null)
                    return _UMass;
                else
                    return "t";
            }
        }

        private static string _UFluid;
        public static string UFluid
        {
            get
            {
                if (_UFluid != null)
                    return _UFluid;
                else
                    return "l";
            }
        }

        private static string _UMoneyDistance;
        public static string UMoneyDistance
        {
            get
            {
                if (_UMoneyDistance != null)
                    return _UMoneyDistance;
                else
                    return "€/km";
            }
        }

        private static string _UAverageFuelConsumption;
        public static string UAverageFuelConsumption
        {
            get
            {
                if (_UAverageFuelConsumption != null)
                    return _UAverageFuelConsumption;
                else
                    return "l/100km";
            }
        }

        private static CultureInfo _UCultureInfo;
        public static CultureInfo UCultureInfo
        {
            get
            {
                if (_UCultureInfo != null)
                    return _UCultureInfo;
                else
                    return new CultureInfo("de-DE");
            }
        }
        #endregion

        #region "ETS2Units"
        public static void SetETS2Units()
        {
            SetETS2UnitsText();
            SetETS2UnitsValues();
        }

        private static void SetETS2UnitsText()
        {
            _USpeed = "km/h";
            _UDistance = "km";
            _UCurrency = "€";
            _UMass = "t";
            _UFluid = "l";
            _UMoneyDistance = "€/km";
            _UAverageFuelConsumption = "l/100km";
            _UCultureInfo = new CultureInfo("de-DE");
        }
        private static void SetETS2UnitsValues()
        {
            //nothing to convert yet
        }
        #endregion

        #region "ATSUnits"
        public static void SetATSUnits()
        {
            SetATSUnitsText();
            SetATSUnitsValues();
        }

        private static void SetATSUnitsText()
        {
            _USpeed = "mph";
            _UDistance = "mi";
            _UCurrency = "$";
            _UMass = "lb";
            _UFluid = "gal";
            _UMoneyDistance = "$/mi";
            _UAverageFuelConsumption = "mpg";
            _UCultureInfo = new CultureInfo("en-US");
        }
        private static void SetATSUnitsValues()
        {
            ConverterHelper.ConvertKMtoMI(CalcData.navigationDistanceC);
            ConverterHelper.ConvertKMtoMI(CalcData.plannedDistanceKM);
            ConverterHelper.ConvertKMtoMI(CalcData.truckSpeed);
            ConverterHelper.ConvertTtoLB(CalcData.jobInfoMassC);
            ConverterHelper.ConvertLtoGAL(CalcData.fuelCurrent);
            ConverterHelper.ConvertLtoGAL(CalcData.fuelCapacity);
            ConverterHelper.ConvertLtoGAL(CalcData.fuelRange);
        }
        #endregion
    }
}
