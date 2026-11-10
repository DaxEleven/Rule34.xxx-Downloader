using R34Downloader.Models;
using R34Downloader.Services;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
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

        private async void Form1_Load(object sender, EventArgs e)
        {
            SettingsModel.Load();

            SettingsModel.Images = true;
            SettingsModel.Gif = true;
            SettingsModel.Video = true;

            toolStripStatusLabel1.Text = "Welcome!";
            toolStripStatusLabel2.Text = "0 / 0";

            if (!string.IsNullOrEmpty(SettingsModel.DownloadPath))
            {
                folderBrowserDialog1.SelectedPath = SettingsModel.DownloadPath;
            }

            var connectivity = await CheckConnectivityAsync();
            while (connectivity == ConnectivityStatus.Offline)
            {
                if (MessageBox.Show("You are offline, please check your internet connection",
                        "Failed to connect to Rule34.xxx",
                        MessageBoxButtons.RetryCancel,
                        MessageBoxIcon.Error) != DialogResult.Retry)
                {
                    Close();

                    return;
                }

                connectivity = await CheckConnectivityAsync();
            }

            if (connectivity == ConnectivityStatus.SiteUnavailable)
            {
                MessageBox.Show("The internet connection works, but rule34.xxx is not reachable (the site may be down or blocked by your provider).\r\n\r\n" +
                                "Searching and downloading may not work until the site is available.",
                    "Rule34.xxx is not reachable",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            if (!SettingsModel.HasApiCredentials)
            {
                toolStripStatusLabel1.Text = "API key is not set";

                var result = MessageBox.Show("To use the API you need your own rule34.xxx API key (User ID + API key).\r\n\r\nOpen the settings and enter it now?",
                    "API key is not set",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    ShowSettings();
                }
            }
        }

        private async void button1_Click(object sender, EventArgs e) // Search Button
        {
            var request = textBox1.Text;

            if (string.IsNullOrWhiteSpace(request))
            {
                MessageBox.Show("Enter tags to search.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return;
            }

            SetBusy(true);

            try
            {
                toolStripStatusLabel1.Text = "Searching...";
                toolStripStatusLabel2.Text = "0 / 0";

                if (!EnsureApiCredentials())
                {
                    toolStripStatusLabel1.Text = "Search cancelled";

                    return;
                }

                var countContent = await Task.Run(() => R34ApiService.GetContentCount(request));

                ShowSearchResult(countContent, request);
            }
            catch (R34Exception exception)
            {
                toolStripStatusLabel1.Text = "Search error";
                ShowServiceError(exception, "Search error");
            }
            catch (Exception exception)
            {
                toolStripStatusLabel1.Text = "Search error";
                MessageBox.Show(exception.Message, "Search error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void button2_Click(object sender, EventArgs e) // Download Button
        {
            var request = textBox1.Text;

            if (string.IsNullOrWhiteSpace(request))
            {
                MessageBox.Show("Enter tags to download.", "Download", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!EnsureApiCredentials())
            {
                return;
            }

            SetBusy(true);

            try
            {
                toolStripStatusLabel1.Text = "Searching...";
                toolStripStatusLabel2.Text = "0 / 0";

                var countContent = await Task.Run(() => R34ApiService.GetContentCount(request));

                if (countContent <= 0)
                {
                    toolStripStatusLabel1.Text = "Nothing found";
                    MessageBox.Show("Nobody here but us chickens!", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return;
                }

                if (folderBrowserDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    toolStripStatusLabel1.Text = "Download cancelled";

                    return;
                }

                SettingsModel.DownloadPath = folderBrowserDialog1.SelectedPath;
                SettingsModel.Save();

                SettingsModel.Limit = 0;

                var maxCount = (ushort)Math.Min(countContent, SettingsModel.MaxLimit);

                using (var downloadingForm = new DownloadingForm(maxCount))
                {
                    downloadingForm.ShowDialog(this);
                }

                if (SettingsModel.Limit <= 0)
                {
                    toolStripStatusLabel1.Text = "Download cancelled";

                    return;
                }

                DownloadControlService.Reset();
                buttonStop.Enabled = true;

                toolStripStatusLabel1.Text = "Downloading content...";
                toolStripProgressBar1.Maximum = SettingsModel.Limit;
                toolStripProgressBar1.Value = 0;

                var progress = new Progress<int>(s => toolStripProgressBar1.Value = s);
                var progress2 = new Progress<int>(s => toolStripStatusLabel2.Text = s + " / " + SettingsModel.Limit);
                IProgress<string> statusProgress = new Progress<string>(s => toolStripStatusLabel1.Text = s);

                RetryPolicy.OnRetryMessage = statusProgress.Report;
                DownloadService.OnFileProgress = (downloadedBytes, totalBytes) =>
                {
                    if (totalBytes > 0)
                    {
                        var percent = (int)(downloadedBytes * 100 / totalBytes.Value);
                        statusProgress.Report($"Downloading content... {percent}%");
                    }
                };

                var path = folderBrowserDialog1.SelectedPath;
                var limit = SettingsModel.Limit;

                var failed = await Task.Run(() => R34ApiService.DownloadContent(path, request, limit, progress, progress2));

                var stopped = DownloadControlService.StopRequested;
                DownloadControlService.Reset();

                toolStripStatusLabel1.Text = stopped ? "Download stopped" : "Download completed";

                var message = stopped
                    ? failed == 0
                        ? "Download stopped. Open the folder?"
                        : $"Download stopped with {failed} error(s). Open the folder?"
                    : failed == 0
                        ? "Download completed. Open the folder?"
                        : $"Download completed with {failed} error(s). Open the folder?";

                var caption = stopped ? "Download stopped" : "Download completed";

                if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    Process.Start(path);
                }
            }
            catch (R34Exception exception)
            {
                toolStripStatusLabel1.Text = "Download error";
                ShowServiceError(exception, "Download error");
            }
            catch (Exception exception)
            {
                toolStripStatusLabel1.Text = "Download error";
                MessageBox.Show(exception.Message, "Download error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                DownloadService.OnFileProgress = null;
                RetryPolicy.OnRetryMessage = null;
                buttonStop.Enabled = false;
                SetBusy(false);
            }
        }

        private void buttonStop_Click(object sender, EventArgs e) // Stop Button
        {
            DownloadControlService.RequestStop();
            buttonStop.Enabled = false;
            DownloadService.OnFileProgress = null;
            toolStripStatusLabel1.Text = "Stopping after the current file...";
        }

        private void button3_Click(object sender, EventArgs e) // About Button
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            MessageBox.Show("The application is not affiliated with rule34.xxx or its administration\n" +
                            "Author: Dax Eleven\n" +
                            "Copyright © 2017 Dax Eleven\n" +
                            $"Version: {version.Major}.{version.Minor}.{version.Build}",
                "About Rule34.xxx Downloader",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void button4_Click(object sender, EventArgs e) // Help Button
        {
            const string searchHelpMessage =
                "The syntax is the same as on the site.\n\n" +
                "tag1 tag2                 posts with both tags\n" +
                "-tag1                     posts without tag1\n" +
                "( tag1 ~ tag2 )           posts with tag1 or tag2 (keep the braces and the spaces!)\n" +
                "rainbow*                  tags that start with \"rainbow\"\n" +
                "ta*1                      tags that start with \"ta\" and end with \"1\"\n" +
                "night~                    fuzzy search for \"night\" (similar tags)\n" +
                "user:bob                  posts uploaded by the user \"bob\"\n" +
                "md5:<hash>                posts with this MD5 hash\n" +
                "parent:1234               posts with the post 1234 as a parent\n" +
                "sourcedomains:example.com posts with a source from example.com\n\n" +
                "rating:safe               rating: safe, questionable or explicit\n" +
                "-rating:explicit          exclude a rating\n" +
                "score:>=10                by score (it is updated once a day)\n" +
                "width:>=1000 height:>1000 by size\n" +
                "aspectratio:16:9          by aspect ratio\n" +
                "sort:score:desc           sorting: id, score, user, height, width,\n" +
                "                          parent, source, updated (asc or desc),\n" +
                "                          sort:random\n\n" +
                "Different meta-tags can be combined: rating:questionable parent:100.\n" +
                "The same meta-tag twice does not work: rating:safe rating:questionable.\n\n" +
                "Examples:\n" +
                "  mercy pharah animated   - all three tags\n" +
                "  tomb_raider -sunglasses - tomb_raider without sunglasses\n" +
                "  ( cat_ears ~ cat_tail ) - either tag\n" +
                "  rainbow* -rating:safe   - safe rainbow tags";

            MessageBox.Show(searchHelpMessage, "Search help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pictureBox2_Click(object sender, EventArgs e) // Settings Button
        {
            ShowSettings();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) // Link to rule34.xxx
        {
            Process.Start("https://rule34.xxx");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) // Link to GitHub page
        {
            Process.Start("https://github.com/DaxEleven/Rule34.xxx-Downloader");
        }

        #endregion

        #region Helpers

        private void ShowSearchResult(int countContent, string request)
        {
            toolStripStatusLabel1.Text = "Search completed";

            if (countContent <= 0)
            {
                MessageBox.Show("Nobody here but us chickens!", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return;
            }

            if (MessageBox.Show(countContent + " results found. Open in a browser?", "Searching results", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                Process.Start(BuildWebUrl(request));
            }
        }

        private static bool EnsureApiCredentials()
        {
            while (!SettingsModel.HasApiCredentials)
            {
                var result = MessageBox.Show("The application works through the official rule34.xxx API and needs a personal API key (User ID + API key).\r\n\r\n" +
                                             "Open the settings and enter it?",
                    "API key is not set",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Information);

                if (result != DialogResult.Yes)
                {
                    return false;
                }

                ShowSettings();
            }

            return true;
        }

        private static void ShowSettings()
        {
            using (var settingsForm = new SettingsForm())
            {
                settingsForm.ShowDialog();
            }
        }

        private static void ShowServiceError(R34Exception exception, string caption)
        {
            var result = MessageBox.Show(exception.Message + "\r\n\r\nOpen the settings?",
                caption,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                ShowSettings();
            }
        }

        private void SetBusy(bool busy)
        {
            button1.Enabled = !busy;
            button2.Enabled = !busy;
            textBox1.Enabled = !busy;
            pictureBox2.Enabled = !busy;
            UseWaitCursor = busy;
        }

        private static string BuildWebUrl(string tags)
        {
            var normalized = string.Join(" ", tags.Split(new[] { ' ', '+' }, StringSplitOptions.RemoveEmptyEntries));

            return "https://rule34.xxx/index.php?page=post&s=list&tags=" + Uri.EscapeDataString(normalized);
        }

        private enum ConnectivityStatus
        {
            Online,
            SiteUnavailable,
            Offline
        }

        private static async Task<ConnectivityStatus> CheckConnectivityAsync()
        {
            if (await AnyRespondsAsync("https://api.rule34.xxx/", "https://rule34.xxx/"))
            {
                return ConnectivityStatus.Online;
            }

            if (await AnyRespondsAsync("https://www.google.com/generate_204", "https://1.1.1.1/"))
            {
                return ConnectivityStatus.SiteUnavailable;
            }

            return ConnectivityStatus.Offline;
        }

        private static async Task<bool> AnyRespondsAsync(params string[] urls)
        {
            var probes = new Task<bool>[urls.Length];

            for (var index = 0; index < urls.Length; index++)
            {
                probes[index] = RespondsAsync(urls[index]);
            }

            var results = await Task.WhenAll(probes);

            foreach (var result in results)
            {
                if (result)
                {
                    return true;
                }
            }

            return false;
        }

        private static async Task<bool> RespondsAsync(string url)
        {
            try
            {
                using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(8) })
                using (await client.GetAsync(url))
                {
                    return true;
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
