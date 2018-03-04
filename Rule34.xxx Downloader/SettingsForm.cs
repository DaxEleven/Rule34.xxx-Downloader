using R34Downloader.Logic;
using System;
using System.Windows.Forms;

namespace R34Downloader
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            if (Transfer.IsAPI)
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
            Transfer.IsAPI = radioButton1.Checked ? true : false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}