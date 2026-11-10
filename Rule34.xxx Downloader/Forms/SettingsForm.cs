using R34Downloader.Models;
using R34Downloader.Services;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
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
            textBoxCredentials.Text = SettingsModel.CredentialsLine;

            ShowStatus(SettingsModel.HasApiCredentials ? "API key is set." : "API key is not set yet.", SettingsModel.HasApiCredentials);
        }

        private void linkLabelGetKey_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(R34ApiService.ApiKeyPageUrl);
        }

        private async void buttonTestKey_Click(object sender, EventArgs e)
        {
            if (!TryGetCredentials(out var userId, out var apiKey))
            {
                ShowStatus("Paste the credentials line from the account options page.", false);

                return;
            }

            buttonTestKey.Enabled = false;
            ShowStatus("Checking the key...", null);

            try
            {
                var count = await Task.Run(() => R34ApiService.GetContentCount(string.Empty, userId, apiKey));

                ShowStatus($"Key is valid. Posts on the site: {count:N0}.", true);
            }
            catch (Exception ex)
            {
                ShowStatus("The key was not accepted.", false);
                MessageBox.Show(ex.Message, "API check", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                buttonTestKey.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var parsed = TryGetCredentials(out var userId, out var apiKey);

            if (!parsed)
            {
                var result = MessageBox.Show(
                    "The credentials line is empty or invalid.\r\n\r\n" +
                    "Paste the line from the account options page, for example: &api_key=...&user_id=...\r\n\r\n" +
                    "Save the settings anyway?",
                    "API credentials are not set",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                {
                    return;
                }
            }

            SettingsModel.UserId = parsed ? userId : string.Empty;
            SettingsModel.ApiKey = parsed ? apiKey : string.Empty;
            SettingsModel.Save();

            Close();
        }

        #endregion

        #region Helpers

        private bool TryGetCredentials(out string userId, out string apiKey)
        {
            return SettingsModel.TryParseCredentials(textBoxCredentials.Text, out userId, out apiKey);
        }

        private void ShowStatus(string text, bool? success)
        {
            labelStatus.Text = text;
            labelStatus.ForeColor = success == true ? Color.ForestGreen : success == false ? Color.Firebrick : SystemColors.ControlText;
        }

        #endregion
    }
}
