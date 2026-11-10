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
            _maxCount = maxCount < 1 ? (ushort)1 : maxCount;
        }

        #endregion

        #region Handlers

        private void DownloadingForm_Load(object sender, EventArgs e)
        {
            trackBar1.Maximum = _maxCount;
            textBoxCount.Text = trackBar1.Value.ToString();

            checkBox1.Checked = SettingsModel.Images;
            checkBox2.Checked = SettingsModel.Gif;
            checkBox3.Checked = SettingsModel.Video;

            label4.Visible = !checkBox1.Checked || !checkBox2.Checked || !checkBox3.Checked;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            var value = trackBar1.Value.ToString();

            if (textBoxCount.Text != value)
            {
                textBoxCount.Text = value;
            }
        }

        private void textBoxCount_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(textBoxCount.Text, out var value))
            {
                return;
            }

            var clamped = ClampCount(value);

            if (clamped != value)
            {
                textBoxCount.Text = clamped.ToString();
                textBoxCount.SelectionStart = textBoxCount.Text.Length;

                return;
            }

            if (trackBar1.Value != clamped)
            {
                trackBar1.Value = clamped;
            }
        }

        private void textBoxCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBoxCount_Leave(object sender, EventArgs e)
        {
            if (!int.TryParse(textBoxCount.Text, out var value))
            {
                value = trackBar1.Value;
            }

            textBoxCount.Text = ClampCount(value).ToString();
        }

        private int ClampCount(int value)
        {
            return Math.Min(Math.Max(value, 1), _maxCount);
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            var checkBox = (CheckBox)sender;
            var text = checkBox.Text.TrimEnd('*');

            checkBox.Text = checkBox.Checked ? text : text + "*";

            label4.Visible = !checkBox1.Checked || !checkBox2.Checked || !checkBox3.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var count = trackBar1.Value;

            if (int.TryParse(textBoxCount.Text, out var typed))
            {
                count = ClampCount(typed);
            }

            SettingsModel.Limit = (ushort)count;
            SettingsModel.Images = checkBox1.Checked;
            SettingsModel.Gif = checkBox2.Checked;
            SettingsModel.Video = checkBox3.Checked;

            Close();
        }

        #endregion
    }
}
