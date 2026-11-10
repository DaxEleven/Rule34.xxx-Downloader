using R34Downloader.Forms;
using R34Downloader.Models;
using R34Downloader.Services;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace R34Downloader
{
    internal static class Program
    {
        #region Fields

        private const int AttachParentProcess = -1;

        #endregion

        #region Methods

        [STAThread]
        private static void Main(string[] args)
        {
            if (TryRunCommand(args))
            {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += OnThreadException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            Application.Run(new MainForm());
        }

        #endregion

        #region Helpers

        private static bool TryRunCommand(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                return false;
            }

            switch (args[0].ToLowerInvariant())
            {
                case "--set-credentials":
                {
                    SetCredentials(args);

                    return true;
                }
                case "--test-key":
                {
                    TestKey();

                    return true;
                }
                default:
                {
                    return false;
                }
            }
        }

        private static void SetCredentials(string[] args)
        {
            AttachConsole(AttachParentProcess);

            if (args.Length != 2 || !SettingsModel.TryParseCredentials(args[1], out var userId, out var apiKey))
            {
                Console.Error.WriteLine("Usage: R34Downloader.exe --set-credentials \"&api_key=...&user_id=...\"");
                Console.Error.WriteLine("Copy the line from https://rule34.xxx/index.php?page=account&s=options");
                Environment.ExitCode = 2;

                return;
            }

            SettingsModel.Load();
            SettingsModel.UserId = userId;
            SettingsModel.ApiKey = apiKey;
            SettingsModel.Save();

            Console.WriteLine("Credentials saved to the user settings.");
        }

        private static void TestKey()
        {
            AttachConsole(AttachParentProcess);

            try
            {
                SettingsModel.Load();

                var count = R34ApiService.GetContentCount(string.Empty);
                Console.WriteLine($"API key is valid. Posts on the site: {count:N0}");
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine("API key check failed: " + exception.Message);
                Environment.ExitCode = 1;
            }
        }

        private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ShowUnexpectedError(e.Exception);
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ShowUnexpectedError(e.ExceptionObject as Exception);
        }

        private static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
        }

        private static void ShowUnexpectedError(Exception exception)
        {
            var message = exception == null ? "Unknown error." : exception.Message;

            MessageBox.Show("An unexpected error occurred:\r\n\r\n" + message,
                "Rule34.xxx Downloader",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AttachConsole(int processId);

        #endregion
    }
}
