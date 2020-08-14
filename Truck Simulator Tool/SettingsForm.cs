using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Forms;

namespace Truck_Simulator_Tool
{
    public partial class SettingsForm : Form
    {
        Settings settings = new Settings();
        string SoftwarePath = Application.StartupPath;
        bool ClosedByCross = true;


        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, System.EventArgs e)
        {
            LoadSettingsOrCreate();
        }


        // Load settings Or Create if not there
        void LoadSettingsOrCreate()
        {
            // Check for settings file
            if (File.Exists(SoftwarePath + @"\config.json"))
            { // Load Data if available
                settings = (JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SoftwarePath + @"\config.json")));
            }
            else
            {
                try
                {
                    settings.AutoSaveActive = true;
                    settings.AntiKickDefaultOn = true;
                    settings.ManualTimescaleValue = 19;
                    settings.BackgroundImageFilePath = "";

                    string sJson = JsonConvert.SerializeObject(settings);
                    File.WriteAllText((String.Format(SoftwarePath + @"\config.json")), sJson);
                }
                catch
                {
                    MessageBox.Show("Schwerwiegender Fehler gefunde! Bitte Autor kontaktieren.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (settings.AutoSaveActive == true)
            {// Auto Save Checked
                checkBox_Sett_AutoSave.Checked = true;
            }
            else
            {
                checkBox_Sett_AutoSave.Checked = false;
            }

            if (settings.AntiKickDefaultOn == true)
            {// AntiKick Checked
                checkBox_Sett_AntiKick.Checked = true;
            }
            else
            {
                checkBox_Sett_AntiKick.Checked = false;
            }


            textBox_BackgroundfilePath.Text = settings.BackgroundImageFilePath;
            numericUpDown_SetTimescale.Value = settings.ManualTimescaleValue;
        }


        //Save settings
        void SaveSettings()
        {
            try
            {
                if (checkBox_Sett_AutoSave.Checked == true)
                {// Set AutoSave to settings
                    settings.AutoSaveActive = true;
                }
                else
                {
                    settings.AutoSaveActive = false;
                }

                if (checkBox_Sett_AntiKick.Checked == true)
                {// AntiKick to settings
                    settings.AntiKickDefaultOn = true;
                }
                else
                {
                    settings.AntiKickDefaultOn = false;
                }

                settings.ManualTimescaleValue = numericUpDown_SetTimescale.Value;
                settings.BackgroundImageFilePath = textBox_BackgroundfilePath.Text;

                string sJson = JsonConvert.SerializeObject(settings);
                File.WriteAllText((String.Format(SoftwarePath + @"\config.json")), sJson);

                ClosedByCross = false;
                if (System.Windows.Forms.Application.OpenForms["MainForm"] != null)
                {
                    (System.Windows.Forms.Application.OpenForms["MainForm"] as MainForm).LoadSettingsOrCreate();
                }
            }
            catch
            {
                MessageBox.Show("Schwerwiegender Fehler gefunde! Bitte Autor kontaktieren.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ClosedByCross == true)
            {
                if (settings.AutoSaveActive == checkBox_Sett_AutoSave.Checked && settings.ManualTimescaleValue == numericUpDown_SetTimescale.Value && settings.BackgroundImageFilePath == textBox_BackgroundfilePath.Text)
                {// Check if something was changed

                }
                else
                {
                    if (MessageBox.Show("Möchten Sie die Änderungen speichern?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        SaveSettings();
                    }
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            if (settings.AutoSaveActive == checkBox_Sett_AutoSave.Checked && settings.ManualTimescaleValue == numericUpDown_SetTimescale.Value && settings.BackgroundImageFilePath == textBox_BackgroundfilePath.Text)
            {// Check if something was changed

            }
            else
            {
                if (MessageBox.Show("Möchten Sie die Änderungen speichern?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    SaveSettings();
                }
            }
            ClosedByCross = false;
            this.Close();
        }

        private void button_resetsettings_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Möchten Sie die Einstellungen zurücksetzen? Die Durchschnittliche Zeitskalierung ist dabei ausgeschlossen.", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                settings.AutoSaveActive = true;
                checkBox_Sett_AutoSave.Checked = true;

                settings.AntiKickDefaultOn = true;
                checkBox_Sett_AntiKick.Checked = true;

                settings.ManualTimescaleValue = 19;
                numericUpDown_SetTimescale.Value = 19;

                textBox_BackgroundfilePath.Text = "";
            }
        }

        private void button_Browse_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            textBox_BackgroundfilePath.Text = openFileDialog1.FileName;
        }

        private void button_DeleteBackgroundFilepath_Click(object sender, EventArgs e)
        {
            textBox_BackgroundfilePath.Text = "";
        }

        private void button_Help_ManualTimescale_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Durch diesen Wert wird die Ingame-Zeit dividiert. Um korrekte Auftragsdaten zu erhalten, ändern Sie diesen Wert entsprechned der Region. Sie können die derzeitige Zeitskalierung der Region unten rechts im Hauptfenster erkennen.\n(ETS2 Autobahn: 19, ETS2 UK Autobahn: 15, ETS2 Städte: 3)\n (ATS  highways: 20, ATS Städte: 3)", "Zeitskalierungshilfe", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }
    }
}
