namespace Truck_Simulator_Tool
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_Save = new System.Windows.Forms.Button();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.button_resetsettings = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label_Sett_text1 = new System.Windows.Forms.Label();
            this.checkBox_Sett_AutoSave = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.numericUpDown_Sett_SetTimescale = new System.Windows.Forms.NumericUpDown();
            this.label_Sett_text2 = new System.Windows.Forms.Label();
            this.checkBox_Sett_UseAverageTimescale = new System.Windows.Forms.CheckBox();
            this.label_Sett_AverageTimescale = new System.Windows.Forms.Label();
            this.button_Sett_ResetAverageTimescaleValue = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.textBox_BackgroundfilePath = new System.Windows.Forms.TextBox();
            this.button_Browse = new System.Windows.Forms.Button();
            this.button_DeleteBackgroundFilepath = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Sett_SetTimescale)).BeginInit();
            this.SuspendLayout();
            // 
            // button_Save
            // 
            this.button_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_Save.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Save.Location = new System.Drawing.Point(6, 228);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(107, 23);
            this.button_Save.TabIndex = 0;
            this.button_Save.Text = "Speichern";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.button1_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_Cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Cancel.Location = new System.Drawing.Point(119, 228);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(107, 23);
            this.button_Cancel.TabIndex = 1;
            this.button_Cancel.Text = "Abbrechen";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // button_resetsettings
            // 
            this.button_resetsettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_resetsettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_resetsettings.Location = new System.Drawing.Point(288, 228);
            this.button_resetsettings.Name = "button_resetsettings";
            this.button_resetsettings.Size = new System.Drawing.Size(107, 23);
            this.button_resetsettings.TabIndex = 2;
            this.button_resetsettings.Text = "Zurücksetzen";
            this.button_resetsettings.UseVisualStyleBackColor = true;
            this.button_resetsettings.Click += new System.EventHandler(this.button_resetsettings_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label_Sett_text1);
            this.panel1.Controls.Add(this.checkBox_Sett_AutoSave);
            this.panel1.Location = new System.Drawing.Point(-4, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(415, 49);
            this.panel1.TabIndex = 4;
            // 
            // label_Sett_text1
            // 
            this.label_Sett_text1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Sett_text1.Location = new System.Drawing.Point(7, 4);
            this.label_Sett_text1.Name = "label_Sett_text1";
            this.label_Sett_text1.Size = new System.Drawing.Size(321, 43);
            this.label_Sett_text1.TabIndex = 4;
            this.label_Sett_text1.Text = "Auftragsdaten automatisch speichern \r\n(stark empfohlen)";
            // 
            // checkBox_Sett_AutoSave
            // 
            this.checkBox_Sett_AutoSave.AutoSize = true;
            this.checkBox_Sett_AutoSave.Checked = true;
            this.checkBox_Sett_AutoSave.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_Sett_AutoSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_Sett_AutoSave.Location = new System.Drawing.Point(384, 14);
            this.checkBox_Sett_AutoSave.Name = "checkBox_Sett_AutoSave";
            this.checkBox_Sett_AutoSave.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBox_Sett_AutoSave.Size = new System.Drawing.Size(15, 14);
            this.checkBox_Sett_AutoSave.TabIndex = 3;
            this.checkBox_Sett_AutoSave.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.numericUpDown_Sett_SetTimescale);
            this.panel2.Controls.Add(this.label_Sett_text2);
            this.panel2.Controls.Add(this.checkBox_Sett_UseAverageTimescale);
            this.panel2.Location = new System.Drawing.Point(-4, 67);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(415, 49);
            this.panel2.TabIndex = 5;
            // 
            // numericUpDown_Sett_SetTimescale
            // 
            this.numericUpDown_Sett_SetTimescale.DecimalPlaces = 2;
            this.numericUpDown_Sett_SetTimescale.Location = new System.Drawing.Point(293, 15);
            this.numericUpDown_Sett_SetTimescale.Maximum = new decimal(new int[] {
            19,
            0,
            0,
            0});
            this.numericUpDown_Sett_SetTimescale.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numericUpDown_Sett_SetTimescale.Name = "numericUpDown_Sett_SetTimescale";
            this.numericUpDown_Sett_SetTimescale.Size = new System.Drawing.Size(76, 20);
            this.numericUpDown_Sett_SetTimescale.TabIndex = 5;
            this.numericUpDown_Sett_SetTimescale.Value = new decimal(new int[] {
            19,
            0,
            0,
            0});
            // 
            // label_Sett_text2
            // 
            this.label_Sett_text2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Sett_text2.Location = new System.Drawing.Point(7, 4);
            this.label_Sett_text2.Name = "label_Sett_text2";
            this.label_Sett_text2.Size = new System.Drawing.Size(321, 43);
            this.label_Sett_text2.TabIndex = 4;
            this.label_Sett_text2.Text = "Benutze durchschnittliche Zeitskalierung\r\n(empfohlen)";
            // 
            // checkBox_Sett_UseAverageTimescale
            // 
            this.checkBox_Sett_UseAverageTimescale.AutoSize = true;
            this.checkBox_Sett_UseAverageTimescale.Checked = true;
            this.checkBox_Sett_UseAverageTimescale.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_Sett_UseAverageTimescale.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_Sett_UseAverageTimescale.Location = new System.Drawing.Point(384, 19);
            this.checkBox_Sett_UseAverageTimescale.Name = "checkBox_Sett_UseAverageTimescale";
            this.checkBox_Sett_UseAverageTimescale.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBox_Sett_UseAverageTimescale.Size = new System.Drawing.Size(15, 14);
            this.checkBox_Sett_UseAverageTimescale.TabIndex = 3;
            this.checkBox_Sett_UseAverageTimescale.UseVisualStyleBackColor = true;
            this.checkBox_Sett_UseAverageTimescale.CheckedChanged += new System.EventHandler(this.checkBox_Sett_UseAverageTimescale_CheckedChanged);
            // 
            // label_Sett_AverageTimescale
            // 
            this.label_Sett_AverageTimescale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_Sett_AverageTimescale.Location = new System.Drawing.Point(3, 201);
            this.label_Sett_AverageTimescale.Name = "label_Sett_AverageTimescale";
            this.label_Sett_AverageTimescale.Size = new System.Drawing.Size(280, 13);
            this.label_Sett_AverageTimescale.TabIndex = 6;
            this.label_Sett_AverageTimescale.Text = "durchschnittliche Zeitskalireung: 0";
            // 
            // button_Sett_ResetAverageTimescaleValue
            // 
            this.button_Sett_ResetAverageTimescaleValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_Sett_ResetAverageTimescaleValue.BackColor = System.Drawing.Color.Brown;
            this.button_Sett_ResetAverageTimescaleValue.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button_Sett_ResetAverageTimescaleValue.ForeColor = System.Drawing.SystemColors.Control;
            this.button_Sett_ResetAverageTimescaleValue.Location = new System.Drawing.Point(289, 196);
            this.button_Sett_ResetAverageTimescaleValue.Name = "button_Sett_ResetAverageTimescaleValue";
            this.button_Sett_ResetAverageTimescaleValue.Size = new System.Drawing.Size(107, 23);
            this.button_Sett_ResetAverageTimescaleValue.TabIndex = 7;
            this.button_Sett_ResetAverageTimescaleValue.Text = "Wert zurücksetzen";
            this.button_Sett_ResetAverageTimescaleValue.UseVisualStyleBackColor = false;
            this.button_Sett_ResetAverageTimescaleValue.Click += new System.EventHandler(this.button_Sett_ResetAverageTimescaleValue_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Alle Dateien (*.jpg; *.png; *.bmp)|*.jpg; *.png; *.bmp";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // textBox_BackgroundfilePath
            // 
            this.textBox_BackgroundfilePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_BackgroundfilePath.Location = new System.Drawing.Point(6, 144);
            this.textBox_BackgroundfilePath.Name = "textBox_BackgroundfilePath";
            this.textBox_BackgroundfilePath.ReadOnly = true;
            this.textBox_BackgroundfilePath.Size = new System.Drawing.Size(277, 20);
            this.textBox_BackgroundfilePath.TabIndex = 8;
            // 
            // button_Browse
            // 
            this.button_Browse.Location = new System.Drawing.Point(288, 144);
            this.button_Browse.Name = "button_Browse";
            this.button_Browse.Size = new System.Drawing.Size(82, 21);
            this.button_Browse.TabIndex = 9;
            this.button_Browse.Text = "durchsuchen";
            this.button_Browse.UseVisualStyleBackColor = true;
            this.button_Browse.Click += new System.EventHandler(this.button_Browse_Click);
            // 
            // button_DeleteBackgroundFilepath
            // 
            this.button_DeleteBackgroundFilepath.Image = global::Truck_Simulator_Tool.Properties.Resources.Delete;
            this.button_DeleteBackgroundFilepath.Location = new System.Drawing.Point(376, 145);
            this.button_DeleteBackgroundFilepath.Name = "button_DeleteBackgroundFilepath";
            this.button_DeleteBackgroundFilepath.Size = new System.Drawing.Size(20, 20);
            this.button_DeleteBackgroundFilepath.TabIndex = 10;
            this.button_DeleteBackgroundFilepath.UseVisualStyleBackColor = true;
            this.button_DeleteBackgroundFilepath.Click += new System.EventHandler(this.button_DeleteBackgroundFilepath_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 125);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(277, 16);
            this.label1.TabIndex = 11;
            this.label1.Text = "Hintergrundbild (beta)";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(404, 263);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_DeleteBackgroundFilepath);
            this.Controls.Add(this.button_Browse);
            this.Controls.Add(this.textBox_BackgroundfilePath);
            this.Controls.Add(this.button_Sett_ResetAverageTimescaleValue);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label_Sett_AverageTimescale);
            this.Controls.Add(this.button_Save);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_resetsettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form2";
            this.Text = "Einstellungen";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form2_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form2_FormClosed);
            this.Load += new System.EventHandler(this.Form2_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Sett_SetTimescale)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Save;
        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.Button button_resetsettings;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label_Sett_text1;
        private System.Windows.Forms.CheckBox checkBox_Sett_AutoSave;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.NumericUpDown numericUpDown_Sett_SetTimescale;
        private System.Windows.Forms.Label label_Sett_text2;
        private System.Windows.Forms.CheckBox checkBox_Sett_UseAverageTimescale;
        private System.Windows.Forms.Label label_Sett_AverageTimescale;
        private System.Windows.Forms.Button button_Sett_ResetAverageTimescaleValue;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox textBox_BackgroundfilePath;
        private System.Windows.Forms.Button button_Browse;
        private System.Windows.Forms.Button button_DeleteBackgroundFilepath;
        private System.Windows.Forms.Label label1;
    }
}