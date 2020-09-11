using Microsoft.Win32;
using System.Windows;
using Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Json;
using Truck_Simulator_Tool__WPF_.TruckSimulatorTool.StaticClasses;

namespace Truck_Simulator_Tool__WPF_
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            SettingsHelper.LoadCreateSettings();
            SetDataFromJson();
        }
        private bool SettingsChanged
        {
            get
            {
                if (SettingsHelper.SettingsJson.ContractAutoSaveActive != checkBox_contractAutoSave.IsChecked || SettingsHelper.SettingsJson.AntiKickAutoStart != checkBox_antiKickAutoStart.IsChecked || SettingsHelper.SettingsJson.TSTServerAutoStart != checkBox_tstServerAutoStart.IsChecked || SettingsHelper.SettingsJson.TimeScaleValue != integerUpDown_timeScale.Value || SettingsHelper.SettingsJson.BackgroundPath != textBox_imageFilePath.Text)
                    return true;
                else
                    return false;
            }
        }

        private void SetDataFromJson()
        {
            checkBox_contractAutoSave.IsChecked = SettingsHelper.SettingsJson.ContractAutoSaveActive;
            checkBox_antiKickAutoStart.IsChecked = SettingsHelper.SettingsJson.AntiKickAutoStart;
            checkBox_antiKickMessage.IsChecked = SettingsHelper.SettingsJson.AntiKickMessage;
            checkBox_tstServerAutoStart.IsChecked = SettingsHelper.SettingsJson.TSTServerAutoStart;
            integerUpDown_timeScale.Value = SettingsHelper.SettingsJson.TimeScaleValue;
            textBox_imageFilePath.Text = SettingsHelper.SettingsJson.BackgroundPath;
        }

        private void SaveJson()
        {
            SettingsJson settingsJson = new SettingsJson();
            settingsJson.ContractAutoSaveActive = checkBox_contractAutoSave.IsChecked ?? true;
            settingsJson.AntiKickAutoStart = checkBox_antiKickAutoStart.IsChecked ?? true;
            settingsJson.AntiKickMessage = checkBox_antiKickMessage.IsChecked ?? false;
            settingsJson.TSTServerAutoStart = checkBox_tstServerAutoStart.IsChecked ?? true;
            settingsJson.TimeScaleValue = integerUpDown_timeScale.Value ?? 19;
            settingsJson.BackgroundPath = textBox_imageFilePath.Text;

            SettingsHelper.SettingsJson = settingsJson;
            SettingsHelper.SaveSettings();
        }

        private void ResetJson()
        {
            checkBox_contractAutoSave.IsChecked = true;
            checkBox_antiKickAutoStart.IsChecked = true;
            checkBox_antiKickMessage.IsChecked = false;
            checkBox_tstServerAutoStart.IsChecked = true;
            integerUpDown_timeScale.Value = 19;
            //textBox_imageFilePath.Text = string.Empty;
        }


        private void button_Save_Click(object sender, RoutedEventArgs e)
        {
            SaveJson();
        }

        private void button_cancel_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsChanged)
            {
                var messageBoxResult = MessageBox.Show("Nicht gespeicherte Änderungen werden verworfen.", "Möchten Sie die Einstellungen speichern?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    SaveJson();
                    this.Close();
                }
                else if (messageBoxResult == MessageBoxResult.No)
                {
                    this.Close();
                }
                else
                {
                    // do nothing (only close message)
                }
            }
            else
            {
                this.Close();
            }
        }

        private void button_reset_Click(object sender, RoutedEventArgs e)
        {
            ResetJson();
        }

        private void button_imageBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Alle Dateien (*.jpg; *.png; *.bmp)|*.jpg; *.png; *.bmp";
            if (fileDialog.ShowDialog() == true)
            {
                textBox_imageFilePath.Text = fileDialog.FileName;
            }
        }

        private void button_imageDelete_Click(object sender, RoutedEventArgs e)
        {
            textBox_imageFilePath.Text = string.Empty;
        }

        private void button_timeScaleInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Durch diesen Wert wird die Ingame-Zeit mit der realen Zeit dividiert. Um korrekte Auftragsdaten zu erhalten, ändern Sie diesen Wert entsprechned der Region. Sie können die derzeitige Zeitskalierung der Region unten rechts im Hauptfenster erkennen. Folgende Werte gelten in den Spielen: \n\nATS  : highways = 20   Städte = 3\nETS2: Autobahnen = 19   AutobahnenUK = 15   Städte = 3", "Informationen über den Zeitskalierungswert", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
