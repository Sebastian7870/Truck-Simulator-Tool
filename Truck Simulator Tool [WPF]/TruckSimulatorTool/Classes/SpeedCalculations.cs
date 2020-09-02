using SCSSdkClient.Object;
using Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Methods;

namespace Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Classes
{
    public class SpeedCalculations
    {
        public double timerCounter { get; set; }

        private double speedSummary;
        private double currentAverageSpeed;
        private double currentBestAverageSpeed;
        private double drivenDistance;
        private double distanceSummary;


        // Speed Summary
        public void SetSpeedSummary(double speed, double multiplier)
        {
            speedSummary += multiplier * speed;
        }
        public double GetSpeedSummary()
        {
            return speedSummary;
        }

        // Current AverageSpeed
        public double GetCurrentAverageSpeed()
        {
            this.currentAverageSpeed = this.speedSummary / this.timerCounter;
            return this.currentAverageSpeed;
        }

        // Current Best AverageSpeed
        public void SetCurrentBestAverageSpeed(double navigationDistance, double navigationTime)
        {
            this.currentBestAverageSpeed = navigationDistance / (navigationTime / 3600);
        }
        public double GetCurrentBestAverageSpeed()
        {
            return this.currentBestAverageSpeed;
        }

        // DrivenDistance
        public void SetDrivenDistance(double speed, double multiplier, double timeScale)
        {
            this.drivenDistance += timeScale *  multiplier * (speed / 3600);
        }
        public double GetDrivenDistance()
        {
            return this.drivenDistance;
        }

        // DistanceSummary
        public void SetDistanceSummary(double navigationDistance)
        {
            this.distanceSummary = this.GetDrivenDistance() + navigationDistance;
        }
        public double GetDistanceSummary()
        {
            return this.distanceSummary;
        }
    }
}
