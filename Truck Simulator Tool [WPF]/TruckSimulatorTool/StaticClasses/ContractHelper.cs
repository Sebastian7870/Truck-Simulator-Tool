using Newtonsoft.Json;
using System.IO;
using System.Timers;
using System.Windows;
using Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Json;

namespace Truck_Simulator_Tool__WPF_.TruckSimulatorTool.StaticClasses
{
    public static class ContractHelper
    {
        private static Timer timer_autoBackupContract = new Timer(60000);
        
        private static bool contractOnStartLoaded = false;
        public static bool ContractOnStartLoaded
        {
            get { return contractOnStartLoaded; }
            set { contractOnStartLoaded = value; }
        }

        private static bool sdkActive = false;
        public static bool SDKActive
        {
            get { return sdkActive; }
            set { sdkActive = value; }
        }

        private static bool onJob = false;
        public static bool OnJob
        {
            get { return onJob; }
            set { onJob = value; }
        }

        private static string truckId;
        public static string TruckId
        {
            get { return truckId; }
            set { truckId = value; }
        }


        private static ContractJson contractJson = new ContractJson();
        public static ContractJson ContractJson
        {
            get
            {
                if (contractJson == null)
                {
                    return null;
                }
                else
                {
                    return contractJson;
                }
            }
            set { contractJson = value; }
        }

        private static string Auto_FileFormat
        {
            get { return $@"\{ContractJson.Game.ToString()}_AutoSaveContract_{ContractJson.CitySource} - {ContractJson.CityDestination}___{ContractJson.Income + ContractJson.Mass}.json"; }
        }


        public static void AutoLoadIfStartup()
        {
            if (!ContractOnStartLoaded)
            {
                if (SettingsHelper.SettingsJson.AntiKickAutoStart && OnJob && truckId != string.Empty)
                {
                    try
                    {
                        ContractJson = JsonConvert.DeserializeObject<ContractJson>(File.ReadAllText($"{StaticValues.SoftwarePath}{StaticValues.ContractsPath}{Auto_FileFormat}"));
                        ContractOnStartLoaded = true;
                    }
                    catch
                    {// started contract without this application
                        ContractOnStartLoaded = true;
                        MessageBox.Show("Es scheint, dass Sie den derzetigen Auftrag ohne diese Software begonnen haben. Bitte beachten Sie, dass Auftragsdaten wie [gefahrene KM] und  [Durchschnittsgeschwindigkeit] erst ab jetzt berechnet werden können.", "Auftragsdaten können Abweichen!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    ContractOnStartLoaded = true;
                }
            }
        }

        public static void StartBackupper()
        {
            timer_autoBackupContract.Elapsed += Timer_autoBackupContract_Elapsed;
            timer_autoBackupContract.Start();
        }


        public static void AutoSave()
        {
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

        private static void Timer_autoBackupContract_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (SettingsHelper.SettingsJson.ContractAutoSaveActive)
            {
                if (SDKActive && OnJob)
                    AutoSave();
            }
        }
    }
}
