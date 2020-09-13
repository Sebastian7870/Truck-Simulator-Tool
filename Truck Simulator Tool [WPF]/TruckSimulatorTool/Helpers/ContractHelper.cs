using Microsoft.Win32;
using Newtonsoft.Json;
using System;
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
            get { return $@"\{ContractJson.Game}_AutoSaveContract_{ContractJson.CitySource} - {ContractJson.CityDestination}___{ContractJson.LastProfile}_{Math.Round(ContractJson.Income + ContractJson.Mass, 0)}.json"; }
        }

        private static string Manual_FileFormat
        {
            get { return $@"\{ContractJson.Game.ToString()}_ManualSaveContract_{ContractJson.CitySource} - {ContractJson.CityDestination}"; }
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
                        if (!contractOnStartLoaded && (CalcData._Data.ets2.truck.navigationEstimatedDistance / 1000) < (CalcData._Data.ets2.job.cargo.plannedDistanceKM - 5))
                            MessageBox.Show("Es scheint, dass Sie den derzetigen Auftrag ohne diese Software begonnen haben. Bitte beachten Sie, dass Auftragsdaten wie [gefahrene KM] und  [Durchschnittsgeschwindigkeit] erst ab jetzt berechnet werden können.", "Auftragsdaten können Abweichen!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    contractOnStartLoaded = true;
                }
            }
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
        /*public static void TryManualSave()
        {
            if (SettingsHelper.SettingsJson.ContractAutoSaveActive && CalcData._Data.ets2.game.connected && CalcData._Data.ets2.job.cargo.id != string.Empty && CalcData.DistanceDriven > 5 && Unit.navigationDistanceC > 5)
            {// SpeedSummary of 2.500 are driving 60 s with a speed of 50. (if interval is 100 ms)
                try
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "json|*.json";
                    saveFileDialog.InitialDirectory = $"{StaticValues.SoftwarePath}{StaticValues.ContractsPath}";
                    saveFileDialog.FileName = Manual_FileFormat;
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        string json = JsonConvert.SerializeObject(contractJson);
                        File.WriteAllText(saveFileDialog.FileName, json);
                    }
                }
                catch
                {
                    MessageBox.Show("Die Auftragsdaten konnten aufgrund eines Fehlers nicht gespeicher werden. Eventuell wurden Programmdateien beschädigt. Durch einen Neustart der Software können Sie das Problem beheben.", "Auftragsdaten nicht gespeichert!", MessageBoxButton.OK, MessageBoxImage.Error);
                    //Todo: add LogEntry
                }
            }
            else
            {
                MessageBox.Show("Es wurden keine oder nicht genügend Auftragsdaten gefunden. Beachten Sie, dass Sie eine mindestdistanz gefahren sein müssen und die Entfernung nicht geringer als 5 km / 3,1 mi sein darf.", "Keine Auftragsdaten gefunden!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }*/


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
        /*public static void ManualLoad()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "json|*.json";
                openFileDialog.InitialDirectory = $"{StaticValues.SoftwarePath}{StaticValues.ContractsPath}";
                if (openFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        contractJson = JsonConvert.DeserializeObject<ContractJson>(File.ReadAllText(openFileDialog.FileName));
                    }
                    catch
                    {
                        MessageBox.Show("Die Datei wurde entweder beschädigt oder es gab ein Update und kann deshalb nicht geladen werden.", "Fehler beim Laden der Datei!", MessageBoxButton.OK, MessageBoxImage.Error);
                        //Todo: log entry
                    }
                }
            }
            catch
            {

            }
        }*/


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
            //lastProfile will not be reseted
        }
    }
}
