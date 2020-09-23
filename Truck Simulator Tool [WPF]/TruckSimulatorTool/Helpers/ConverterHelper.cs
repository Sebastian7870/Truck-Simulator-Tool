using System;

namespace Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Methods
{
    public static class ConverterHelper
    {
        public static double ConvertKMtoMI(double kilometer)
        {
            return kilometer / 1.609344;
        }

        public static double ConvertTtoLB(double tons)
        {
            return tons * 2240;
        }

        public static double ConvertLtoGAL(double liters)
        {
            return liters / 3.7886952;
        }

        public static double ConvertEUAverageFueltoAMAverageFuel(double averageFuelConsumption)
        {
            return averageFuelConsumption / 6.43242746591568;
        }

        public static string ConvertTimespanToCustomString(TimeSpan timeSpan)
        {// Only shows available values (00:02 ---> 2 min. (and nod 0 hrs. 2 min.))
            if (timeSpan.Days != 0)
            {
                return $"{(timeSpan.Days * 24) + timeSpan.Hours} Std. {timeSpan.Minutes} Min.";
            }
            else
            {
                if (timeSpan.Hours != 0)
                    return $"{timeSpan.Hours} Std. {timeSpan.Minutes} Min.";
                else
                    return $"{timeSpan.Minutes} Min.";
            }
        }
    }
}
