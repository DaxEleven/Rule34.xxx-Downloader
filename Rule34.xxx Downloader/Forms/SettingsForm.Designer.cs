namespace R34Downloader.Forms
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBoxAria2 = new System.Windows.Forms.GroupBox();
            this.checkBoxUseAria2 = new System.Windows.Forms.CheckBox();
            this.textBoxAria2ServerUrl = new System.Windows.Forms.TextBox();
            this.textBoxAria2SecretToken = new System.Windows.Forms.TextBox();
            this.labelAria2ServerUrl = new System.Windows.Forms.Label();
            this.labelAria2SecretToken = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBoxAria2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(231, 81);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " Download method ";
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(8, 47);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(183, 20);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.Text = "Parsing (if API not working)";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.radioButton_MouseClick);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(8, 21);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(146, 20);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "API (recommended)";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.radioButton_MouseClick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(169, 250);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 13;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBoxAria2
            // 
            this.groupBoxAria2.Controls.Add(this.checkBoxUseAria2);
            this.groupBoxAria2.Controls.Add(this.textBoxAria2ServerUrl);
            this.groupBoxAria2.Controls.Add(this.textBoxAria2SecretToken);
            this.groupBoxAria2.Controls.Add(this.labelAria2ServerUrl);
            this.groupBoxAria2.Controls.Add(this.labelAria2SecretToken);
            this.groupBoxAria2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBoxAria2.Location = new System.Drawing.Point(12, 100);
            this.groupBoxAria2.Name = "groupBoxAria2";
            this.groupBoxAria2.Size = new System.Drawing.Size(360, 130);
            this.groupBoxAria2.TabIndex = 19;
            this.groupBoxAria2.TabStop = false;
            this.groupBoxAria2.Text = " Aria2 Settings ";
            // 
            // checkBoxUseAria2
            // 
            this.checkBoxUseAria2.AutoSize = true;
            this.checkBoxUseAria2.Location = new System.Drawing.Point(8, 21);
            this.checkBoxUseAria2.Name = "checkBoxUseAria2";
            this.checkBoxUseAria2.Size = new System.Drawing.Size(74, 20);
            this.checkBoxUseAria2.TabIndex = 0;
            this.checkBoxUseAria2.Text = "Use Aria2";
            this.checkBoxUseAria2.UseVisualStyleBackColor = true;
            this.checkBoxUseAria2.CheckedChanged += new System.EventHandler(this.checkBoxUseAria2_CheckedChanged);
            // 
            // textBoxAria2ServerUrl
            // 
            this.textBoxAria2ServerUrl.Location = new System.Drawing.Point(150, 50);
            this.textBoxAria2ServerUrl.Name = "textBoxAria2ServerUrl";
            this.textBoxAria2ServerUrl.Size = new System.Drawing.Size(200, 22);
            this.textBoxAria2ServerUrl.TabIndex = 1;
            // 
            // textBoxAria2SecretToken
            // 
            this.textBoxAria2SecretToken.Location = new System.Drawing.Point(150, 80);
            this.textBoxAria2SecretToken.Name = "textBoxAria2SecretToken";
            this.textBoxAria2SecretToken.Size = new System.Drawing.Size(200, 22);
            this.textBoxAria2SecretToken.TabIndex = 2;
            // 
            // labelAria2ServerUrl
            // 
            this.labelAria2ServerUrl.AutoSize = true;
            this.labelAria2ServerUrl.Location = new System.Drawing.Point(8, 53);
            this.labelAria2ServerUrl.Name = "labelAria2ServerUrl";
            this.labelAria2ServerUrl.Size = new System.Drawing.Size(94, 16);
            this.labelAria2ServerUrl.TabIndex = 3;
            this.labelAria2ServerUrl.Text = "Aria2 Server URL:";
            // 
            // labelAria2SecretToken
            // 
            this.labelAria2SecretToken.AutoSize = true;
            this.labelAria2SecretToken.Location = new System.Drawing.Point(8, 83);
            this.labelAria2SecretToken.Name = "labelAria2SecretToken";
            this.labelAria2SecretToken.Size = new System.Drawing.Size(94, 16);
            this.labelAria2SecretToken.TabIndex = 4;
            this.labelAria2SecretToken.Text = "Aria2 Secret Token:";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(229)))), ((int)(((byte)(164)))));
            this.ClientSize = new System.Drawing.Size(400, 300);
            this.Controls.Add(this.groupBoxAria2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxAria2.ResumeLayout(false);
            this.groupBoxAria2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBoxAria2;
        private System.Windows.Forms.CheckBox checkBoxUseAria2;
        private System.Windows.Forms.TextBox textBoxAria2ServerUrl;
        private System.Windows.Forms.TextBox textBoxAria2SecretToken;
        private System.Windows.Forms.Label labelAria2ServerUrl;
        private System.Windows.Forms.Label labelAria2SecretToken;
    }
}