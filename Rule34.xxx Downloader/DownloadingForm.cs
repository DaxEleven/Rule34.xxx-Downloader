using R34Downloader.Logic;
using System;
using System.Windows.Forms;

namespace R34Downloader
{
    public partial class DownloadingForm : Form
    {
        private int maxPid = 0;

        public DownloadingForm(int maxPid)
        {
            InitializeComponent();
            this.maxPid = maxPid;
        }

        private void DownloadingForm_Load(object sender, EventArgs e)
        {
            if (maxPid < 500000)
            {
                trackBar1.Maximum = maxPid;
                numericUpDown1.Maximum = maxPid;
            }
            else
            {
                label2.Text = "500000 maximum";
            }
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
                ((CheckBox)sender).Text = ((CheckBox)sender).Text.Remove(((CheckBox)sender).Text.Length - 1);
            else
                ((CheckBox)sender).Text += "*";

            label4.Visible = (checkBox1.Checked && checkBox2.Checked && checkBox3.Checked) ? false : true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Transfer.Limit = trackBar1.Value;
            Transfer.Images = checkBox1.Checked;
            Transfer.Gif = checkBox2.Checked;
            Transfer.Video = checkBox3.Checked;
            Close();
        }
    }
}
