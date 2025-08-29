using R34Downloader.Models;
using R34Downloader.Properties;
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
            if (!string.IsNullOrEmpty(SettingsModel.APICreds))
            {
                apicred.Text = SettingsModel.APICreds;
            }
        }

        private void radioButton_MouseClick(object sender, MouseEventArgs e)
        {
            SettingsModel.IsApi = radioButton1.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(apicred.Text))
            {
                SettingsModel.APICreds = apicred.Text;
                Properties.Settings.Default.APICreds = apicred.Text;
                Properties.Settings.Default.Save();
            }
            Close();
        }

        #endregion
    }
}
