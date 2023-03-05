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
        }

        private void radioButton_MouseClick(object sender, MouseEventArgs e)
        {
            SettingsModel.IsApi = radioButton1.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion
    }
}
