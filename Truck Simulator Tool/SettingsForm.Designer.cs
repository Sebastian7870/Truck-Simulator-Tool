namespace Truck_Simulator_Tool
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.button_Save = new System.Windows.Forms.Button();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.button_Resetsettings = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label_Text1 = new System.Windows.Forms.Label();
            this.checkBox_Sett_AutoSave = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button_Help_ManualTimescale = new System.Windows.Forms.Button();
            this.numericUpDown_SetTimescale = new System.Windows.Forms.NumericUpDown();
            this.label_Text2 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.textBox_BackgroundfilePath = new System.Windows.Forms.TextBox();
            this.button_Browse = new System.Windows.Forms.Button();
            this.button_DeleteBackgroundFilepath = new System.Windows.Forms.Button();
            this.label_Text3 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label_Text4 = new System.Windows.Forms.Label();
            this.checkBox_Sett_AntiKick = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_SetTimescale)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_Save
            // 
            this.button_Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_Save.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_Save.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Save.Location = new System.Drawing.Point(6, 262);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(107, 23);
            this.button_Save.TabIndex = 5;
            this.button_Save.Text = "Speichern";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.button1_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_Cancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Cancel.Location = new System.Drawing.Point(119, 262);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(107, 23);
            this.button_Cancel.TabIndex = 6;
            this.button_Cancel.Text = "Abbrechen";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // button_Resetsettings
            // 
            this.button_Resetsettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_Resetsettings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_Resetsettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Resetsettings.Location = new System.Drawing.Point(288, 262);
            this.button_Resetsettings.Name = "button_Resetsettings";
            this.button_Resetsettings.Size = new System.Drawing.Size(107, 23);
            this.button_Resetsettings.TabIndex = 7;
            this.button_Resetsettings.Text = "Zurücksetzen";
            this.button_Resetsettings.UseVisualStyleBackColor = true;
            this.button_Resetsettings.Click += new System.EventHandler(this.button_resetsettings_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label_Text1);
            this.panel1.Controls.Add(this.checkBox_Sett_AutoSave);
            this.panel1.Location = new System.Drawing.Point(0, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(411, 49);
            this.panel1.TabIndex = 4;
            // 
            // label_Text1
            // 
            this.label_Text1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Text1.Location = new System.Drawing.Point(7, 4);
            this.label_Text1.Name = "label_Text1";
            this.label_Text1.Size = new System.Drawing.Size(280, 43);
            this.label_Text1.TabIndex = 4;
            this.label_Text1.Text = "Auftragsdaten automatisch speichern \r\n(stark empfohlen)";
            // 
            // checkBox_Sett_AutoSave
            // 
            this.checkBox_Sett_AutoSave.AutoSize = true;
            this.checkBox_Sett_AutoSave.Checked = true;
            this.checkBox_Sett_AutoSave.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_Sett_AutoSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkBox_Sett_AutoSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_Sett_AutoSave.Location = new System.Drawing.Point(380, 16);
            this.checkBox_Sett_AutoSave.Name = "checkBox_Sett_AutoSave";
            this.checkBox_Sett_AutoSave.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBox_Sett_AutoSave.Size = new System.Drawing.Size(15, 14);
            this.checkBox_Sett_AutoSave.TabIndex = 1;
            this.checkBox_Sett_AutoSave.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button_Help_ManualTimescale);
            this.panel2.Controls.Add(this.numericUpDown_SetTimescale);
            this.panel2.Controls.Add(this.label_Text2);
            this.panel2.Location = new System.Drawing.Point(-4, 100);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(415, 49);
            this.panel2.TabIndex = 5;
            // 
            // button_Help_ManualTimescale
            // 
            this.button_Help_ManualTimescale.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Help_ManualTimescale.Location = new System.Drawing.Point(292, 13);
            this.button_Help_ManualTimescale.Name = "button_Help_ManualTimescale";
            this.button_Help_ManualTimescale.Size = new System.Drawing.Size(24, 22);
            this.button_Help_ManualTimescale.TabIndex = 5;
            this.button_Help_ManualTimescale.Text = "?";
            this.button_Help_ManualTimescale.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button_Help_ManualTimescale.UseVisualStyleBackColor = true;
            this.button_Help_ManualTimescale.Click += new System.EventHandler(this.button_Help_ManualTimescale_Click);
            // 
            // numericUpDown_SetTimescale
            // 
            this.numericUpDown_SetTimescale.DecimalPlaces = 2;
            this.numericUpDown_SetTimescale.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown_SetTimescale.Location = new System.Drawing.Point(319, 13);
            this.numericUpDown_SetTimescale.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDown_SetTimescale.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.numericUpDown_SetTimescale.Name = "numericUpDown_SetTimescale";
            this.numericUpDown_SetTimescale.Size = new System.Drawing.Size(76, 22);
            this.numericUpDown_SetTimescale.TabIndex = 3;
            this.numericUpDown_SetTimescale.Value = new decimal(new int[] {
            19,
            0,
            0,
            0});
            // 
            // label_Text2
            // 
            this.label_Text2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Text2.Location = new System.Drawing.Point(7, 15);
            this.label_Text2.Name = "label_Text2";
            this.label_Text2.Size = new System.Drawing.Size(280, 20);
            this.label_Text2.TabIndex = 4;
            this.label_Text2.Text = "Zeitskalierung";
            this.label_Text2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Alle Dateien (*.jpg; *.png; *.bmp)|*.jpg; *.png; *.bmp";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // textBox_BackgroundfilePath
            // 
            this.textBox_BackgroundfilePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_BackgroundfilePath.Location = new System.Drawing.Point(6, 180);
            this.textBox_BackgroundfilePath.Name = "textBox_BackgroundfilePath";
            this.textBox_BackgroundfilePath.ReadOnly = true;
            this.textBox_BackgroundfilePath.Size = new System.Drawing.Size(257, 22);
            this.textBox_BackgroundfilePath.TabIndex = 8;
            // 
            // button_Browse
            // 
            this.button_Browse.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_Browse.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Browse.Location = new System.Drawing.Point(269, 178);
            this.button_Browse.Name = "button_Browse";
            this.button_Browse.Size = new System.Drawing.Size(101, 24);
            this.button_Browse.TabIndex = 4;
            this.button_Browse.Text = "durchsuchen";
            this.button_Browse.UseVisualStyleBackColor = true;
            this.button_Browse.Click += new System.EventHandler(this.button_Browse_Click);
            // 
            // button_DeleteBackgroundFilepath
            // 
            this.button_DeleteBackgroundFilepath.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_DeleteBackgroundFilepath.Image = global::Truck_Simulator_Tool.Properties.Resources.Delete;
            this.button_DeleteBackgroundFilepath.Location = new System.Drawing.Point(371, 178);
            this.button_DeleteBackgroundFilepath.Name = "button_DeleteBackgroundFilepath";
            this.button_DeleteBackgroundFilepath.Size = new System.Drawing.Size(24, 24);
            this.button_DeleteBackgroundFilepath.TabIndex = 10;
            this.button_DeleteBackgroundFilepath.TabStop = false;
            this.button_DeleteBackgroundFilepath.UseVisualStyleBackColor = true;
            this.button_DeleteBackgroundFilepath.Click += new System.EventHandler(this.button_DeleteBackgroundFilepath_Click);
            // 
            // label_Text3
            // 
            this.label_Text3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Text3.Location = new System.Drawing.Point(6, 159);
            this.label_Text3.Name = "label_Text3";
            this.label_Text3.Size = new System.Drawing.Size(277, 16);
            this.label_Text3.TabIndex = 11;
            this.label_Text3.Text = "Hintergrundbild (beta)";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label_Text4);
            this.panel3.Controls.Add(this.checkBox_Sett_AntiKick);
            this.panel3.Location = new System.Drawing.Point(0, 51);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(411, 49);
            this.panel3.TabIndex = 5;
            // 
            // label_Text4
            // 
            this.label_Text4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Text4.Location = new System.Drawing.Point(7, 4);
            this.label_Text4.Name = "label_Text4";
            this.label_Text4.Size = new System.Drawing.Size(280, 43);
            this.label_Text4.TabIndex = 4;
            this.label_Text4.Text = "AntiKick standartmäßig einschalten:";
            this.label_Text4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // checkBox_Sett_AntiKick
            // 
            this.checkBox_Sett_AntiKick.AutoSize = true;
            this.checkBox_Sett_AntiKick.Checked = true;
            this.checkBox_Sett_AntiKick.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_Sett_AntiKick.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkBox_Sett_AntiKick.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox_Sett_AntiKick.Location = new System.Drawing.Point(380, 16);
            this.checkBox_Sett_AntiKick.Name = "checkBox_Sett_AntiKick";
            this.checkBox_Sett_AntiKick.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBox_Sett_AntiKick.Size = new System.Drawing.Size(15, 14);
            this.checkBox_Sett_AntiKick.TabIndex = 1;
            this.checkBox_Sett_AntiKick.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(404, 297);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.label_Text3);
            this.Controls.Add(this.button_DeleteBackgroundFilepath);
            this.Controls.Add(this.button_Browse);
            this.Controls.Add(this.textBox_BackgroundfilePath);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button_Save);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_Resetsettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowInTaskbar = false;
            this.Text = "Einstellungen";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsForm_FormClosing);
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_SetTimescale)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Save;
        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.Button button_Resetsettings;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label_Text1;
        private System.Windows.Forms.CheckBox checkBox_Sett_AutoSave;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.NumericUpDown numericUpDown_SetTimescale;
        private System.Windows.Forms.Label label_Text2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox textBox_BackgroundfilePath;
        private System.Windows.Forms.Button button_Browse;
        private System.Windows.Forms.Button button_DeleteBackgroundFilepath;
        private System.Windows.Forms.Label label_Text3;
        private System.Windows.Forms.Button button_Help_ManualTimescale;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label_Text4;
        private System.Windows.Forms.CheckBox checkBox_Sett_AntiKick;
    }
}