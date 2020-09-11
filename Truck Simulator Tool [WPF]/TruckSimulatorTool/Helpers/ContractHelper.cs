using Newtonsoft.Json;
using System.IO;
using System.Timers;
using System.Windows;
using Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Classes;
using Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Json;

namespace Truck_Simulator_Tool__WPF_.TruckSimulatorTool.StaticClasses
{
    public static class ContractHelper
    {
        #region "Variables"
        public static Timer timer_autoBackupContract = new Timer(2500);
        public static bool contractOnStartLoaded { get; set; }
        #endregion

        private static ContractJson contractJson = new ContractJson();
        public static ContractJson ContractJson
        {
            get
            {
                if (contractJson == null)
                    return null;
                else
                    return contractJson;
            }
            set { contractJson = value; }
        }

        private static string Auto_FileFormat
        {
            get { return $@"\{ContractJson.Game.ToString()}_AutoSaveContract_{ContractJson.CitySource} - {ContractJson.CityDestination}___{ContractJson.Income + ContractJson.Mass}.json"; }
        }


        public static void AutoLoadIfStartup()
        {
            if (!contractOnStartLoaded)
            {
                if (SettingsHelper.SettingsJson.AntiKickAutoStart && CalcData._Data.ets2.game.connected && CalcData._Data.ets2.job.cargo.id != string.Empty)
                {
                    try
                    {
                        ContractJson = JsonConvert.DeserializeObject<ContractJson>(File.ReadAllText($"{StaticValues.SoftwarePath}{StaticValues.ContractsPath}{Auto_FileFormat}"));
                        CalcData.LoadValues(ContractJson);
                        contractOnStartLoaded = true;
                    }
                    catch
                    {// started contract without this application
                        contractOnStartLoaded = true;
                        if ((CalcData._Data.ets2.truck.navigationEstimatedDistance / 1000) < (CalcData._Data.ets2.job.cargo.plannedDistanceKM - 5))
                            MessageBox.Show("Es scheint, dass Sie den derzetigen Auftrag ohne diese Software begonnen haben. Bitte beachten Sie, dass Auftragsdaten wie [gefahrene KM] und  [Durchschnittsgeschwindigkeit] erst ab jetzt berechnet werden können.", "Auftragsdaten können Abweichen!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    contractOnStartLoaded = true;
                }
            }
        }

        public static void StartBackupper()
        {
            timer_autoBackupContract.Elapsed += Timer_autoBackupContract_Elapsed;
            timer_autoBackupContract.Start();
        }

        public static void StopBackupper()
        {
            timer_autoBackupContract.Stop();
        }

        public static void TryAutoSave()
        {
            if (SettingsHelper.SettingsJson.ContractAutoSaveActive && CalcData._Data.ets2.game.connected && CalcData._Data.ets2.job.cargo.id != string.Empty && CalcData.SpeedSummary > 250 && Unit.navigationDistanceC > 5)
            {// SpeedSummary of 2.500 are driving 60 s with a speed of 50. (if interval is 100 ms)
                try
                {
                    string json = JsonConvert.SerializeObject(ContractJson);
                    File.WriteAllText($"{StaticValues.SoftwarePath}{StaticValues.ContractsPath}{Auto_FileFormat}", json);
                }
                catch
                {
                    MessageBox.Show("Die Auftragsdaten konnten aufgrund eines Fehlers nicht gespeicher werden. Eventuell wurden Programmdateien beschädigt. Durch einen Neustart der Software können Sie das Problem beheben.", "Auftragsdaten nicht gespeichert!", MessageBoxButton.OK, MessageBoxImage.Error);
                    //Todo: add LogEntry
                }
            }
        }

        public static void AutoDelete()
        {
            try
            {
                File.Delete($"{StaticValues.SoftwarePath}{StaticValues.ContractsPath}{Auto_FileFormat}");
            }
            catch
            {
                // Todo: add LogEntry (no error just file not found ==> Information)
            }
        }

        public static void ResetValues()
        {
            contractJson.CityDestination = string.Empty;
            contractJson.CitySource = string.Empty;
            contractJson.distanceDriven = 0;
            contractJson.distanceSummary = 0;
            contractJson.Income = 0;
            contractJson.Mass = 0;
            contractJson.OdometerStartValue = 0;
            contractJson.speedSummary = 0;
            contractJson.timerCounter = 0;
            contractJson.Game = string.Empty;
        }

        private static void Timer_autoBackupContract_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (SettingsHelper.SettingsJson.ContractAutoSaveActive)
                TryAutoSave();
        }
    }
}
