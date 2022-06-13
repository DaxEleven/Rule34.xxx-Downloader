using R34Downloader.Logic;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace R34Downloader
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Transfer.IsAPI = true;
            toolStripStatusLabel1.Text = "Welcome!";
            toolStripStatusLabel2.Text = "0 / 0";

            if (Properties.Settings.Default.Path != null || Properties.Settings.Default.Path != "")
            {
                folderBrowserDialog1.SelectedPath = Properties.Settings.Default.Path;
            }

            if (!CheckForInternetConnection("https://rule34.xxx"))
            {
                if (MessageBox.Show("You are offline please check your internet connection", "Unable to connect to the Rule34.xxx", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
                {
                    Form1_Load(sender, e);
                }
                else
                {
                    Close();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e) // Search Button
        {
            try
            {
                toolStripStatusLabel1.Text = "Searching...";
                string request = textBox1.Text.Replace(' ', '+').Replace("*", "%2a");
                if (Transfer.IsAPI)
                {
                    int countContent = R34API.GetCountContent(request);
                    if (countContent > 0)
                    {
                        toolStripStatusLabel1.Text = "Search completed";
                        if (MessageBox.Show(countContent + " results found. Open in a browser?", "Searching results", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        {
                            Process.Start("https://rule34.xxx/index.php?page=post&s=list&tags=" + request);
                        }
                    }
                    else
                    {
                        toolStripStatusLabel1.Text = "Search completed";
                        MessageBox.Show("Nobody here but us chickens!", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else // If parsing method
                {
                    if (R34Parser.IsSomethingFound(request))
                    {
                        int countContent = R34Parser.GetCountContent(request, R34Parser.GetMaxPid(request));
                        if (countContent > 0)
                        {
                            toolStripStatusLabel1.Text = "Search completed";
                            if (MessageBox.Show(countContent + " results found. Open in a browser?", "Searching results", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                            {
                                Process.Start("https://rule34.xxx/index.php?page=post&s=list&tags=" + request);
                            }
                        }
                        else
                        {
                            toolStripStatusLabel1.Text = "Search completed";
                            MessageBox.Show("Unable to search this deep in temporarily (error on site)", "Search error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        toolStripStatusLabel1.Text = "Search completed";
                        MessageBox.Show("Nobody here but us chickens!", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception exp)
            {
                toolStripStatusLabel1.Text = "Search error";
                MessageBox.Show(exp.Message, "Search error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void button2_Click(object sender, EventArgs e) // Download Button
        {
            try
            {
                string request = textBox1.Text.Replace(' ', '+').Replace("*", "%2a");
                if (Transfer.IsAPI)
                {
                    int countContent = R34API.GetCountContent(request);
                    if (countContent > 0)
                    {
                        if (folderBrowserDialog1.ShowDialog() != DialogResult.Cancel)
                        {
                            Properties.Settings.Default.Path = folderBrowserDialog1.SelectedPath;
                            Properties.Settings.Default.Save();

                            DownloadingForm dlf = new DownloadingForm(countContent);
                            dlf.ShowDialog();

                            if (Transfer.Limit > 0)
                            {
                                toolStripStatusLabel1.Text = "Downloading content...";
                                toolStripProgressBar1.Maximum = Transfer.Limit;

                                var progress = new Progress<int>(s => toolStripProgressBar1.Value = s);
                                var progress2 = new Progress<int>(s => toolStripStatusLabel2.Text = s + " / " + Transfer.Limit);
                                await Task.Factory.StartNew(() => R34API.DownloadContent(request, folderBrowserDialog1.SelectedPath, progress, progress2, Transfer.Limit), TaskCreationOptions.LongRunning);

                                toolStripStatusLabel1.Text = "Download completed";
                                if (MessageBox.Show("Download completed! Open the folder?", "Download completed", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                                {
                                    Process.Start(folderBrowserDialog1.SelectedPath);
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Nobody here but us chickens!", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else // If parsing method
                {
                    if (R34Parser.IsSomethingFound(request))
                    {
                        if (folderBrowserDialog1.ShowDialog() != DialogResult.Cancel)
                        {
                            Properties.Settings.Default.Path = folderBrowserDialog1.SelectedPath;
                            Properties.Settings.Default.Save();

                            int countContent = R34Parser.GetCountContent(request, R34Parser.GetMaxPid(request));
                            DownloadingForm dlf;
                            if (countContent > 0)
                            {
                                dlf = new DownloadingForm(countContent);
                            }
                            else
                            {
                                dlf = new DownloadingForm(500000);
                            }
                            dlf.ShowDialog();

                            if (Transfer.Limit > 0)
                            {
                                toolStripStatusLabel1.Text = "Downloading content...";
                                toolStripProgressBar1.Maximum = Transfer.Limit;

                                var progress = new Progress<int>(s => toolStripProgressBar1.Value = s);
                                var progress2 = new Progress<int>(s => toolStripStatusLabel2.Text = s + " / " + Transfer.Limit);
                                await Task.Factory.StartNew(() => R34Parser.DownloadPages(request, folderBrowserDialog1.SelectedPath, progress, progress2, Transfer.Limit), TaskCreationOptions.LongRunning);

                                toolStripStatusLabel1.Text = "Download completed";
                                if (MessageBox.Show("Download completed. Open the folder?", "Download completed", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                                {
                                    Process.Start(folderBrowserDialog1.SelectedPath);
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Nobody here but us chickens!", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception exp)
            {
                toolStripStatusLabel1.Text = "Download error";
                MessageBox.Show(exp.Message, "Download error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e) // About Button
        {
            MessageBox.Show("The author has nothing to do with the rule34.xxx\nAuthor: r34dlnew\nVersion: 1.0.2", "About Rule34.xxx Downloader", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button4_Click(object sender, EventArgs e) // Help Button
        {
            string searchHelpMessage = "You can use:\n'*' all,\n(' ' or '+') union,\n'-' remove;\n\nFor example:\n > \"rainbow *\" - search for all tags starting with \"rainbow\"\n      rainbow_dash_(mlp)\n      rainbow_fur\n      rainbow_tail\n\n > \"mercy pharah animated\" - posts where there is \"mercy\", \"pharah\" and \"animated\" at the same time\n     \"fallout+elizabeth\"\n\n > \"tomb_raider -dickgirl -zoophilia\" - posts where there is \"tomb_raider\", but no \"dickgirl\" and \"zoophilia\"";
            MessageBox.Show(searchHelpMessage, "Search help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pictureBox2_Click(object sender, EventArgs e) // Settings Button
        {
            SettingsForm sf = new SettingsForm();
            sf.ShowDialog();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) // Link to rule34.xxx
        {
            Process.Start("https://rule34.xxx");
        }

        public static bool CheckForInternetConnection(string siteName)
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead(siteName))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
