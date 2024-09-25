using R34Downloader.Models;
using System;
using System.Windows.Forms;

namespace R34Downloader.Forms
{
    /// <summary>
    /// Settings form.
    /// </summary>
    public partial class SettingsForm : Form
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the SettingsForm class.
        /// </summary>
        public SettingsForm()
        {
            InitializeComponent();
        }

        #endregion

        #region Handlers

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            if (SettingsModel.IsApi)
            {
                radioButton1.Checked = true;
            }
            else
            {
                radioButton2.Checked = true;
            }

            textBoxAria2ServerUrl.Text = SettingsModel.Aria2ServerUrl;
            textBoxAria2SecretToken.Text = SettingsModel.Aria2SecretToken;
            checkBoxUseAria2.Checked = SettingsModel.UseAria2;

            ToggleAria2Fields(SettingsModel.UseAria2);
        }

        private void radioButton_MouseClick(object sender, MouseEventArgs e)
        {
            SettingsModel.IsApi = radioButton1.Checked;
        }

        private void checkBoxUseAria2_CheckedChanged(object sender, EventArgs e)
        {
            SettingsModel.UseAria2 = checkBoxUseAria2.Checked;
            ToggleAria2Fields(checkBoxUseAria2.Checked);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SettingsModel.Aria2ServerUrl = textBoxAria2ServerUrl.Text;
            SettingsModel.Aria2SecretToken = textBoxAria2SecretToken.Text;
            SettingsModel.UseAria2 = checkBoxUseAria2.Checked;
            Close();
        }

        private void ToggleAria2Fields(bool isVisible)
        {
            textBoxAria2ServerUrl.Visible = isVisible;
            textBoxAria2SecretToken.Visible = isVisible;
            labelAria2ServerUrl.Visible = isVisible;
            labelAria2SecretToken.Visible = isVisible;
        }

        #endregion
    }
}
