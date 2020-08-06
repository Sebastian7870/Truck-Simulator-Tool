namespace Truck_Simulator_Tool
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1_settings = new System.Windows.Forms.Button();
            this.label1_paused = new System.Windows.Forms.Label();
            this.label2_timescale = new System.Windows.Forms.Label();
            this.timer1_calculate = new System.Windows.Forms.Timer(this.components);
            this.label3_currentarrival = new System.Windows.Forms.Label();
            this.label4_currentbestarrival = new System.Windows.Forms.Label();
            this.label5_jobinfo = new System.Windows.Forms.Label();
            this.pictureBox1_distance = new System.Windows.Forms.PictureBox();
            this.label6_timebuffer = new System.Windows.Forms.Label();
            this.label7_remainingtime = new System.Windows.Forms.Label();
            this.label8_nextpausetime = new System.Windows.Forms.Label();
            this.label9_estimatedtime = new System.Windows.Forms.Label();
            this.label10_sourcedata = new System.Windows.Forms.Label();
            this.label11_destinationdata = new System.Windows.Forms.Label();
            this.label12_progresspercentage = new System.Windows.Forms.Label();
            this.label13_remainingdistance = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label14_datetimetime = new System.Windows.Forms.Label();
            this.label15_datetimedate = new System.Windows.Forms.Label();
            this.pictureBox2_cargodamage = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1_distance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2_cargodamage)).BeginInit();
            this.SuspendLayout();
            // 
            // button1_settings
            // 
            resources.ApplyResources(this.button1_settings, "button1_settings");
            this.button1_settings.Name = "button1_settings";
            this.button1_settings.UseVisualStyleBackColor = true;
            this.button1_settings.Click += new System.EventHandler(this.button1_settings_Click);
            // 
            // label1_paused
            // 
            this.label1_paused.BackColor = System.Drawing.Color.Brown;
            resources.ApplyResources(this.label1_paused, "label1_paused");
            this.label1_paused.ForeColor = System.Drawing.Color.Gainsboro;
            this.label1_paused.Name = "label1_paused";
            // 
            // label2_timescale
            // 
            resources.ApplyResources(this.label2_timescale, "label2_timescale");
            this.label2_timescale.Name = "label2_timescale";
            // 
            // timer1_calculate
            // 
            this.timer1_calculate.Interval = 1000;
            this.timer1_calculate.Tick += new System.EventHandler(this.timer1_calculate_Tick);
            // 
            // label3_currentarrival
            // 
            resources.ApplyResources(this.label3_currentarrival, "label3_currentarrival");
            this.label3_currentarrival.Name = "label3_currentarrival";
            // 
            // label4_currentbestarrival
            // 
            resources.ApplyResources(this.label4_currentbestarrival, "label4_currentbestarrival");
            this.label4_currentbestarrival.Name = "label4_currentbestarrival";
            // 
            // label5_jobinfo
            // 
            resources.ApplyResources(this.label5_jobinfo, "label5_jobinfo");
            this.label5_jobinfo.Name = "label5_jobinfo";
            // 
            // pictureBox1_distance
            // 
            this.pictureBox1_distance.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.pictureBox1_distance, "pictureBox1_distance");
            this.pictureBox1_distance.Name = "pictureBox1_distance";
            this.pictureBox1_distance.TabStop = false;
            // 
            // label6_timebuffer
            // 
            resources.ApplyResources(this.label6_timebuffer, "label6_timebuffer");
            this.label6_timebuffer.Name = "label6_timebuffer";
            // 
            // label7_remainingtime
            // 
            resources.ApplyResources(this.label7_remainingtime, "label7_remainingtime");
            this.label7_remainingtime.Name = "label7_remainingtime";
            // 
            // label8_nextpausetime
            // 
            resources.ApplyResources(this.label8_nextpausetime, "label8_nextpausetime");
            this.label8_nextpausetime.Name = "label8_nextpausetime";
            // 
            // label9_estimatedtime
            // 
            resources.ApplyResources(this.label9_estimatedtime, "label9_estimatedtime");
            this.label9_estimatedtime.Name = "label9_estimatedtime";
            // 
            // label10_sourcedata
            // 
            resources.ApplyResources(this.label10_sourcedata, "label10_sourcedata");
            this.label10_sourcedata.Name = "label10_sourcedata";
            // 
            // label11_destinationdata
            // 
            resources.ApplyResources(this.label11_destinationdata, "label11_destinationdata");
            this.label11_destinationdata.Name = "label11_destinationdata";
            // 
            // label12_progresspercentage
            // 
            this.label12_progresspercentage.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.label12_progresspercentage, "label12_progresspercentage");
            this.label12_progresspercentage.Name = "label12_progresspercentage";
            // 
            // label13_remainingdistance
            // 
            this.label13_remainingdistance.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.label13_remainingdistance, "label13_remainingdistance");
            this.label13_remainingdistance.Name = "label13_remainingdistance";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Name = "panel1";
            // 
            // label14_datetimetime
            // 
            this.label14_datetimetime.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            resources.ApplyResources(this.label14_datetimetime, "label14_datetimetime");
            this.label14_datetimetime.ForeColor = System.Drawing.Color.Gainsboro;
            this.label14_datetimetime.Name = "label14_datetimetime";
            // 
            // label15_datetimedate
            // 
            this.label15_datetimedate.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            resources.ApplyResources(this.label15_datetimedate, "label15_datetimedate");
            this.label15_datetimedate.ForeColor = System.Drawing.Color.Gainsboro;
            this.label15_datetimedate.Name = "label15_datetimedate";
            // 
            // pictureBox2_cargodamage
            // 
            this.pictureBox2_cargodamage.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.pictureBox2_cargodamage, "pictureBox2_cargodamage");
            this.pictureBox2_cargodamage.Name = "pictureBox2_cargodamage";
            this.pictureBox2_cargodamage.TabStop = false;
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.Controls.Add(this.pictureBox2_cargodamage);
            this.Controls.Add(this.label15_datetimedate);
            this.Controls.Add(this.label14_datetimetime);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label13_remainingdistance);
            this.Controls.Add(this.label12_progresspercentage);
            this.Controls.Add(this.label11_destinationdata);
            this.Controls.Add(this.label10_sourcedata);
            this.Controls.Add(this.label9_estimatedtime);
            this.Controls.Add(this.label8_nextpausetime);
            this.Controls.Add(this.label7_remainingtime);
            this.Controls.Add(this.label6_timebuffer);
            this.Controls.Add(this.pictureBox1_distance);
            this.Controls.Add(this.label5_jobinfo);
            this.Controls.Add(this.label4_currentbestarrival);
            this.Controls.Add(this.label3_currentarrival);
            this.Controls.Add(this.label2_timescale);
            this.Controls.Add(this.label1_paused);
            this.Controls.Add(this.button1_settings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1_distance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2_cargodamage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1_settings;
        private System.Windows.Forms.Label label1_paused;
        private System.Windows.Forms.Label label2_timescale;
        private System.Windows.Forms.Timer timer1_calculate;
        private System.Windows.Forms.Label label3_currentarrival;
        private System.Windows.Forms.Label label4_currentbestarrival;
        private System.Windows.Forms.Label label5_jobinfo;
        private System.Windows.Forms.PictureBox pictureBox1_distance;
        private System.Windows.Forms.Label label6_timebuffer;
        private System.Windows.Forms.Label label7_remainingtime;
        private System.Windows.Forms.Label label8_nextpausetime;
        private System.Windows.Forms.Label label9_estimatedtime;
        private System.Windows.Forms.Label label10_sourcedata;
        private System.Windows.Forms.Label label11_destinationdata;
        private System.Windows.Forms.Label label12_progresspercentage;
        private System.Windows.Forms.Label label13_remainingdistance;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label14_datetimetime;
        private System.Windows.Forms.Label label15_datetimedate;
        private System.Windows.Forms.PictureBox pictureBox2_cargodamage;
    }
}

