using System;
using System.Globalization;
using Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Methods;
using Truck_Simulator_Tool__WPF_.TruckSimulatorTool.StaticClasses;

namespace Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Classes
{
    public static class Unit
    {
        #region "UnitTexts"
        public static string USpeed
        {
            get
            {
                if (CalcData._game == CalcData.game.ets2)
                    return "km/h";
                else if (CalcData._game == CalcData.game.ats)
                    return "mph";
                else //unkown
                    return "km/h";
            }
        }

        public static string UDistance
        {
            get
            {
                if (CalcData._game == CalcData.game.ets2)
                    return "km";
                else if (CalcData._game == CalcData.game.ats)
                    return "mi";
                else //unkown
                    return "km";
            }
        }

        public static string UCurrency
        {
            get
            {
                if (CalcData._game == CalcData.game.ets2)
                    return "€";
                else if (CalcData._game == CalcData.game.ats)
                    return "$";
                else //unkown
                    return "€";
            }
        }

        public static string UMass
        {
            get
            {
                if (CalcData._game == CalcData.game.ets2)
                    return "t";
                else if (CalcData._game == CalcData.game.ats)
                    return "lb";
                else //unkown
                    return "t";
            }
        }

        public static string UFluid
        {
            get
            {
                if (CalcData._game == CalcData.game.ets2)
                    return "l";
                else if (CalcData._game == CalcData.game.ats)
                    return "gal";
                else //unkown
                    return "l";
            }
        }

        public static string UMoneyDistance
        {
            get
            {
                if (CalcData._game == CalcData.game.ets2)
                    return "€/km";
                else if (CalcData._game == CalcData.game.ats)
                    return "$/km";
                else //unkown
                    return "€/km";
            }
        }

        public static string UAverageFuelConsumption
        {
            get
            {
                if (CalcData._game == CalcData.game.ets2)
                    return "l/100km";
                else if (CalcData._game == CalcData.game.ats)
                    return "mpg";
                else //unkown
                    return "l/100km";
            }
        }

        public static CultureInfo UCultureInfo
        {
            get
            {
                if (CalcData._game == CalcData.game.ets2)
                    return new CultureInfo("de-DE");
                else if (CalcData._game == CalcData.game.ats)
                    return new CultureInfo("en-US");
                else //unkown
                    return new CultureInfo("de-DE");
            }
        }
        #endregion

        #region "UnitValues"
        public static double navigationDistanceC
        {
            get
            {
                if (CalcData._game == CalcData.game.ets2)
                    return CalcData._Data.ets2.truck.navigationEstimatedDistance / 1000;
                else if (CalcData._game == CalcData.game.ats)
                    return ConverterHelper.ConvertKMtoMI(CalcData._Data.ets2.truck.navigationEstimatedDistance / 1000);
                else //unkown
                    return CalcData._Data.ets2.truck.navigationEstimatedDistance / 1000;
            }
        }

        public static double plannedDistanceKM
        {
            get
            {
                if (CalcData._game == CalcData.game.ets2)
                    return CalcData._Data.ets2.job.cargo.plannedDistanceKM;
                else if (CalcData._game == CalcData.game.ats)
                    return ConverterHelper.ConvertKMtoMI(CalcData._Data.ets2.job.cargo.plannedDistanceKM);
                else //unkown
                    return CalcData._Data.ets2.job.cargo.plannedDistanceKM;
            }
        }

        public static double truckSpeed
        {
            get
            {
                if (CalcData._game == CalcData.game.ets2)
                    return Math.Abs(CalcData._Data.ets2.truck.speed);
                else if (CalcData._game == CalcData.game.ats)
                    return ConverterHelper.ConvertKMtoMI(Math.Abs(CalcData._Data.ets2.truck.speed));
                else //unkown
                    return Math.Abs(CalcData._Data.ets2.truck.speed);
            }
        }

        public static double jobInfoMassC
        {
            get
            {
                if (CalcData._game == CalcData.game.ets2)
                    return CalcData._Data.ets2.job.cargo.totalMass / 1000;
                else if (CalcData._game == CalcData.game.ats)
                    return ConverterHelper.ConvertTtoLB(CalcData._Data.ets2.job.cargo.totalMass / 1000);
                else //unkown
                    return CalcData._Data.ets2.job.cargo.totalMass / 1000;
            }
        }

        public static double fuelAverageConsumptionC
        {
            get
            {
                if (CalcData._game == CalcData.game.ets2)
                    return CalcData._Data.ets2.truck.fuelAverageConsumption * 100;
                else if (CalcData._game == CalcData.game.ats)
                    return ConverterHelper.ConvertEUAverageFueltoAMAverageFuel(CalcData._Data.ets2.truck.fuelAverageConsumption * 100);
                else //unkown
                    return CalcData._Data.ets2.truck.fuelAverageConsumption * 100;
            }
        }

        public static double fuelCurrent
        {
            get
            {
                if (CalcData._game == CalcData.game.ets2)
                    return CalcData._Data.ets2.truck.fuel;
                else if (CalcData._game == CalcData.game.ats)
                    return ConverterHelper.ConvertLtoGAL(CalcData._Data.ets2.truck.fuel);
                else //unkown
                    return CalcData._Data.ets2.truck.fuel;
            }
        }

        public static double fuelCapacity
        {
            get
            {
                if (CalcData._game == CalcData.game.ets2)
                    return CalcData._Data.ets2.truck.fuelCapacity;
                else if (CalcData._game == CalcData.game.ats)
                    return ConverterHelper.ConvertLtoGAL(CalcData._Data.ets2.truck.fuelCapacity);
                else //unkown
                    return CalcData._Data.ets2.truck.fuelCapacity;
            }
        }

        public static double fuelRange
        {
            get
            {
                if (CalcData._game == CalcData.game.ets2)
                    return CalcData._Data.ets2.truck.fuelRange;
                else if (CalcData._game == CalcData.game.ats)
                    return ConverterHelper.ConvertKMtoMI(CalcData._Data.ets2.truck.fuelRange);
                else //unkown
                    return CalcData._Data.ets2.truck.fuelRange;
            }
        }

        public static double currentOdometer
        {
            get
            {
                if (CalcData._game == CalcData.game.ets2)
                    return CalcData._Data.ets2.truck.odometer;
                else if (CalcData._game == CalcData.game.ats)
                    return ConverterHelper.ConvertKMtoMI(CalcData._Data.ets2.truck.odometer);
                else //unkown
                    return CalcData._Data.ets2.truck.odometer;
            }
        }

        public static string ingameTime //not used: when timeZones are activated ingame the time does not match with the ingame time.
        {
            get
            {
                return DateTime.MinValue.Add(TimeSpan.FromSeconds(CalcData._Data.ets2.game.gameTime)).ToString("ddd H:mm", Unit.UCultureInfo);
            }

        }
        #endregion
    }
}
