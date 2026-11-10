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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.buttonTestKey = new System.Windows.Forms.Button();
            this.linkLabelGetKey = new System.Windows.Forms.LinkLabel();
            this.labelCredentialsHint = new System.Windows.Forms.Label();
            this.textBoxCredentials = new System.Windows.Forms.TextBox();
            this.labelCredentials = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelStatus);
            this.groupBox2.Controls.Add(this.buttonTestKey);
            this.groupBox2.Controls.Add(this.linkLabelGetKey);
            this.groupBox2.Controls.Add(this.labelCredentialsHint);
            this.groupBox2.Controls.Add(this.textBoxCredentials);
            this.groupBox2.Controls.Add(this.labelCredentials);
            this.groupBox2.Font = new System.Drawing.Font("Verdana", 9F);
            this.groupBox2.Location = new System.Drawing.Point(10, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(494, 99);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Rule34.xxx API";
            // 
            // labelStatus
            // 
            this.labelStatus.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.labelStatus.Location = new System.Drawing.Point(12, 119);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(408, 17);
            this.labelStatus.TabIndex = 7;
            // 
            // buttonTestKey
            // 
            this.buttonTestKey.Font = new System.Drawing.Font("Verdana", 9F);
            this.buttonTestKey.Location = new System.Drawing.Point(386, 61);
            this.buttonTestKey.Name = "buttonTestKey";
            this.buttonTestKey.Size = new System.Drawing.Size(99, 27);
            this.buttonTestKey.TabIndex = 3;
            this.buttonTestKey.Text = "Test key";
            this.buttonTestKey.UseVisualStyleBackColor = true;
            this.buttonTestKey.Click += new System.EventHandler(this.buttonTestKey_Click);
            // 
            // linkLabelGetKey
            // 
            this.linkLabelGetKey.AutoSize = true;
            this.linkLabelGetKey.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabelGetKey.Location = new System.Drawing.Point(88, 69);
            this.linkLabelGetKey.Name = "linkLabelGetKey";
            this.linkLabelGetKey.Size = new System.Drawing.Size(80, 14);
            this.linkLabelGetKey.TabIndex = 4;
            this.linkLabelGetKey.TabStop = true;
            this.linkLabelGetKey.Text = "Get API key";
            this.linkLabelGetKey.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelGetKey_LinkClicked);
            // 
            // labelCredentialsHint
            // 
            this.labelCredentialsHint.Font = new System.Drawing.Font("Verdana", 8F);
            this.labelCredentialsHint.Location = new System.Drawing.Point(88, 44);
            this.labelCredentialsHint.Name = "labelCredentialsHint";
            this.labelCredentialsHint.Size = new System.Drawing.Size(220, 16);
            this.labelCredentialsHint.TabIndex = 2;
            this.labelCredentialsHint.Text = "Example: &&api_key=...&&user_id=...";
            // 
            // textBoxCredentials
            // 
            this.textBoxCredentials.Font = new System.Drawing.Font("Verdana", 9F);
            this.textBoxCredentials.Location = new System.Drawing.Point(91, 18);
            this.textBoxCredentials.MaxLength = 256;
            this.textBoxCredentials.Name = "textBoxCredentials";
            this.textBoxCredentials.Size = new System.Drawing.Size(392, 22);
            this.textBoxCredentials.TabIndex = 2;
            // 
            // labelCredentials
            // 
            this.labelCredentials.AutoSize = true;
            this.labelCredentials.Location = new System.Drawing.Point(6, 21);
            this.labelCredentials.Name = "labelCredentials";
            this.labelCredentials.Size = new System.Drawing.Size(84, 14);
            this.labelCredentials.TabIndex = 0;
            this.labelCredentials.Text = "Credentials:";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Verdana", 10F);
            this.button1.Location = new System.Drawing.Point(395, 115);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(110, 37);
            this.button1.TabIndex = 1;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(229)))), ((int)(((byte)(164)))));
            this.ClientSize = new System.Drawing.Size(514, 161);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelCredentials;
        private System.Windows.Forms.TextBox textBoxCredentials;
        private System.Windows.Forms.Label labelCredentialsHint;
        private System.Windows.Forms.LinkLabel linkLabelGetKey;
        private System.Windows.Forms.Button buttonTestKey;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Button button1;
    }
}
