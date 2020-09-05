using Newtonsoft.Json;
using System.IO;
using System.Windows;
using Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Json;

namespace Truck_Simulator_Tool__WPF_.TruckSimulatorTool.StaticClasses
{
    public static class SettingsHelper
    {
        private static string fileName
        {
            get { return "config.json"; }
        }

        private static SettingsJson settingsJson = new SettingsJson();
        public static SettingsJson SettingsJson
        {
            get
            {
                if (settingsJson == null)
                {
                    LoadCreateSettings();
                    return settingsJson;
                }
                else
                {
                    return settingsJson;
                }
            }
            set { settingsJson = value; }
        }

        private static bool SettingsFileExists()
        {
            if (File.Exists($"{StaticValues.SoftwarePath}{fileName}"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void LoadCreateSettings()
        {
            if (SettingsFileExists())
            {
                try
                {
                    LoadSettingsFile();
                }
                catch
                {
                    MessageBox.Show("Die Datei wurde wahrscheinlich beschädigt. Die Einstellugen mussten zurückgesetzt werden.", $"\"{fileName}\" konnte nicht geladen werden!", MessageBoxButton.OK, MessageBoxImage.Error);
                    DeleteSettingsFile();
                    CreateSettingsFile();
                }
            }
            else
            {
                CreateSettingsFile();
            }
        }

        public static void SaveSettings()
        {
            try
            {
                string json = JsonConvert.SerializeObject(SettingsJson);
                File.WriteAllText($"{StaticValues.SoftwarePath}{fileName}", json);
            }
            catch
            {
                MessageBox.Show("Dieser Fehler sollte nicht vorkommen. Bitte informieren Sie den Autor. Die Einstellungen wurden zurückgesetzt", "Fehler beim Speichern der Einstellungen!", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadCreateSettings();
            }
        }


        private static void LoadSettingsFile()
        {
            SettingsJson = JsonConvert.DeserializeObject<SettingsJson>(File.ReadAllText($"{StaticValues.SoftwarePath}{fileName}"));
        }

        private static void CreateSettingsFile()
        {
            SettingsJson settingsJson = new SettingsJson();
            settingsJson.ContractAutoSaveActive = true;
            settingsJson.AntiKickAutoStart = true;
            settingsJson.TSTServerAutoStart = true;
            settingsJson.TimeScaleValue = 19;
            settingsJson.BackgroundPath = string.Empty;

            string json = JsonConvert.SerializeObject(settingsJson);
            File.WriteAllText($"{StaticValues.SoftwarePath}{fileName}", json);
        }

        private static void DeleteSettingsFile()
        {
            File.Delete($"{StaticValues.SoftwarePath}{fileName}");
        }
    }
}
