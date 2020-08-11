using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace Truck_Simulator_Tool
{
    public partial class Form2 : Form
    {
        Settings settings = new Settings();
        string SoftwarePath = Application.StartupPath;
        bool TimeScaleChanged = false;
        bool ClosedByCross = true;


        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, System.EventArgs e)
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
                    settings.AverageTimescaleActive = true;
                    settings.AverageTimescaleValue = 0;
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

            if (settings.AverageTimescaleActive == true)
            {// Average TimeScale Checked
                checkBox_Sett_UseAverageTimescale.Checked = true;
                numericUpDown_Sett_SetTimescale.Visible = false;
            }
            else
            {
                checkBox_Sett_UseAverageTimescale.Checked = false;
                numericUpDown_Sett_SetTimescale.Visible = true;
            }
            
            textBox_BackgroundfilePath.Text = settings.BackgroundImageFilePath;
            numericUpDown_Sett_SetTimescale.Value = settings.ManualTimescaleValue;
            label_Sett_AverageTimescale.Text = "durchschnittliche Zeitskalierung: " + settings.AverageTimescaleValue.ToString();
        }

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

                if (checkBox_Sett_UseAverageTimescale.Checked == true)
                {// Set AverageTimescale to settings
                    settings.AverageTimescaleActive = true;
                }
                else
                {
                    settings.AverageTimescaleActive = false;
                }
                settings.ManualTimescaleValue = numericUpDown_Sett_SetTimescale.Value;
                settings.BackgroundImageFilePath = textBox_BackgroundfilePath.Text;

                string sJson = JsonConvert.SerializeObject(settings);
                File.WriteAllText((String.Format(SoftwarePath + @"\config.json")), sJson);

                ClosedByCross = false;
                this.Close();
            }
            catch
            {
                MessageBox.Show("Schwerwiegender Fehler gefunde! Bitte Autor kontaktieren.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ClosedByCross == true)
            {
                if (settings.AutoSaveActive == checkBox_Sett_AutoSave.Checked && settings.AverageTimescaleActive == checkBox_Sett_UseAverageTimescale.Checked && TimeScaleChanged == false && settings.ManualTimescaleValue == numericUpDown_Sett_SetTimescale.Value && settings.BackgroundImageFilePath == textBox_BackgroundfilePath.Text)
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

        private void button_Sett_ResetAverageTimescaleValue_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Dieser Wert ist sehr bedeutsam und beim Zurücksetzen kann es zu unerwünschten Werten führen. Möchten Sie fortfahren?", "Warnung!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                if (MessageBox.Show("Sie bestätigen hiermit, dass Sie den Wert löschen möchten.", "Möchten Sie diese Datei wirklich löschen?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    settings.AverageTimescaleValue = 0;
                    label_Sett_AverageTimescale.Text = "durchschnittliche Zeitskalierung: " + settings.AverageTimescaleValue.ToString();
                    TimeScaleChanged = true;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveSettings();
            this.Close();
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            if (settings.AutoSaveActive == checkBox_Sett_AutoSave.Checked && settings.AverageTimescaleActive == checkBox_Sett_UseAverageTimescale.Checked && TimeScaleChanged == false && settings.ManualTimescaleValue == numericUpDown_Sett_SetTimescale.Value && settings.BackgroundImageFilePath == textBox_BackgroundfilePath.Text)
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
                settings.AverageTimescaleActive = true;
                settings.ManualTimescaleValue = 19;

                if (settings.AutoSaveActive == true)
                {// Auto Save Checked
                    checkBox_Sett_AutoSave.Checked = true;
                }
                else
                {
                    checkBox_Sett_AutoSave.Checked = false;
                }

                if (settings.AverageTimescaleActive == true)
                {// Average TimeScale Checked
                    checkBox_Sett_UseAverageTimescale.Checked = true;
                    numericUpDown_Sett_SetTimescale.Visible = false;
                }
                else
                {
                    checkBox_Sett_UseAverageTimescale.Checked = false;
                    numericUpDown_Sett_SetTimescale.Visible = true;
                }

                numericUpDown_Sett_SetTimescale.Value = settings.ManualTimescaleValue;
            }
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (System.Windows.Forms.Application.OpenForms["Form1"] != null)
            {
                (System.Windows.Forms.Application.OpenForms["Form1"] as Form1).LoadSettingsOrCreate();
            }
        }

        private void button_Browse_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                
            }
            catch
            {

            }
            textBox_BackgroundfilePath.Text = openFileDialog1.FileName;
        }

        private void button_DeleteBackgroundFilepath_Click(object sender, EventArgs e)
        {
            textBox_BackgroundfilePath.Text = "";
        }

        private void checkBox_Sett_UseAverageTimescale_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_Sett_UseAverageTimescale.Checked == true)
            {
                numericUpDown_Sett_SetTimescale.Visible = false;
            }
            else
            {
                numericUpDown_Sett_SetTimescale.Visible = true;
            }
        }
    }
}
