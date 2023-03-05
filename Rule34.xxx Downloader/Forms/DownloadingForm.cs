using R34Downloader.Models;
using System;
using System.Windows.Forms;

namespace R34Downloader.Forms
{
    /// <summary>
    /// Downloading form.
    /// </summary>
    public partial class DownloadingForm : Form
    {
        #region Fields

        private readonly ushort _maxCount;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the DownloadingForm class.
        /// </summary>
        /// <param name="maxCount">Maximum amount of content.</param>
        public DownloadingForm(ushort maxCount)
        {
            InitializeComponent();
            _maxCount = maxCount;
        }

        #endregion

        #region Handlers

        private void DownloadingForm_Load(object sender, EventArgs e)
        {
            trackBar1.Maximum = _maxCount;
            numericUpDown1.Maximum = _maxCount;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown1.Value = trackBar1.Value;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            trackBar1.Value = Convert.ToInt32(numericUpDown1.Value);
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                ((CheckBox)sender).Text = ((CheckBox)sender).Text.Remove(((CheckBox)sender).Text.Length - 1);
            }
            else
            {
                ((CheckBox)sender).Text += "*";
            }

            label4.Visible = !checkBox1.Checked || !checkBox2.Checked || !checkBox3.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SettingsModel.Limit = Convert.ToUInt16(trackBar1.Value);
            SettingsModel.Images = checkBox1.Checked;
            SettingsModel.Gif = checkBox2.Checked;
            SettingsModel.Video = checkBox3.Checked;

            Close();
        }

        #endregion
    }
}
