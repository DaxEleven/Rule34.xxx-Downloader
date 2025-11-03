using R34Downloader.Models;
using R34Downloader.Services;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace R34Downloader.Forms
{
    /// <summary>
    /// Main form.
    /// </summary>
    public partial class MainForm : Form
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the MainForm class.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        #endregion

        #region Handlers

        private void Form1_Load(object sender, EventArgs e)
        {
            SettingsModel.IsApi = false;
            toolStripStatusLabel1.Text = "Welcome!";
            toolStripStatusLabel2.Text = "0 / 0";

            if (!string.IsNullOrEmpty(Properties.Settings.Default.Path))
            {
                folderBrowserDialog1.SelectedPath = Properties.Settings.Default.Path;
            }

            if (!CheckForInternetConnection("https://rule34.xxx"))
            {
                if (MessageBox.Show("You are offline, please check your internet connection", "Failed to connect to Rule34.xxx", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
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
                var request = textBox1.Text.Replace(' ', '+').Replace("*", "%2a");
                if (SettingsModel.IsApi)
                {
                    var countContent = R34ApiService.GetContentCount(request);
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
                    if (R34HtmlService.IsSomethingFound(request))
                    {
                        var countContent = R34HtmlService.GetCountContent(request, R34HtmlService.GetMaxPid(request));
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
                var request = textBox1.Text.Replace(' ', '+').Replace("*", "%2a");
                if (SettingsModel.IsApi)
                {
                    var countContent = R34ApiService.GetContentCount(request);
                    if (countContent > 0)
                    {
                        if (folderBrowserDialog1.ShowDialog() != DialogResult.Cancel)
                        {
                            Properties.Settings.Default.Path = folderBrowserDialog1.SelectedPath;
                            Properties.Settings.Default.Save();

                            var downloadingForm = new DownloadingForm((ushort)countContent);
                            downloadingForm.ShowDialog();

                            if (SettingsModel.Limit > 0)
                            {
                                toolStripStatusLabel1.Text = "Downloading content...";
                                toolStripProgressBar1.Maximum = SettingsModel.Limit;

                                var progress = new Progress<int>(s => toolStripProgressBar1.Value = s);
                                var progress2 = new Progress<int>(s => toolStripStatusLabel2.Text = s + " / " + SettingsModel.Limit);
                                await Task.Factory.StartNew(() => R34ApiService.DownloadContent(folderBrowserDialog1.SelectedPath, request, SettingsModel.Limit, progress, progress2), TaskCreationOptions.LongRunning);

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
                    if (R34HtmlService.IsSomethingFound(request))
                    {
                        if (folderBrowserDialog1.ShowDialog() != DialogResult.Cancel)
                        {
                            Properties.Settings.Default.Path = folderBrowserDialog1.SelectedPath;
                            Properties.Settings.Default.Save();

                            var countContent = R34HtmlService.GetCountContent(request, R34HtmlService.GetMaxPid(request));
                            var downloadingForm = countContent > 0 ? new DownloadingForm((ushort)countContent) : new DownloadingForm(ushort.MaxValue);
                            downloadingForm.ShowDialog();

                            if (SettingsModel.Limit > 0)
                            {
                                toolStripStatusLabel1.Text = "Downloading content...";
                                toolStripProgressBar1.Maximum = SettingsModel.Limit;

                                var progress = new Progress<int>(s => toolStripProgressBar1.Value = s);
                                var progress2 = new Progress<int>(s => toolStripStatusLabel2.Text = s + " / " + SettingsModel.Limit);
                                await Task.Factory.StartNew(() => R34HtmlService.DownloadContent(folderBrowserDialog1.SelectedPath, request, SettingsModel.Limit, progress, progress2), TaskCreationOptions.LongRunning);

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
            MessageBox.Show("The author has nothing to do with the rule34.xxx\nAuthor: Dax Eleven\nVersion: 1.0.5", "About Rule34.xxx Downloader", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button4_Click(object sender, EventArgs e) // Help Button
        {
            const string searchHelpMessage = "You can use:\n'*' all,\n(' ' or '+') union,\n'-' remove;\n\nFor example:\n > \"rainbow *\" - search for all tags starting with \"rainbow\"\n      rainbow_dash_(mlp)\n      rainbow_fur\n      rainbow_tail\n\n > \"mercy pharah animated\" - posts where there is \"mercy\", \"pharah\" and \"animated\" at the same time\n     \"fallout+elizabeth\"\n\n > \"tomb_raider -dickgirl -zoophilia\" - posts where there is \"tomb_raider\", but no \"dickgirl\" and \"zoophilia\"";
            MessageBox.Show(searchHelpMessage, "Search help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pictureBox2_Click(object sender, EventArgs e) // Settings Button
        {
            var settingsForm = new SettingsForm();
            settingsForm.ShowDialog();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) // Link to rule34.xxx
        {
            Process.Start("https://rule34.xxx");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/DaxEleven/Rule34.xxx-Downloader");
        }

        #endregion

        #region Helpers

        private static bool CheckForInternetConnection(string address)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var responseTask = client.GetAsync(address);
                    var response = responseTask.GetAwaiter().GetResult();

                    return response.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
