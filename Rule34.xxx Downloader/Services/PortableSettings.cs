using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace R34Downloader.Services
{
    /// <summary>
    /// Stores the application settings in a single XML file next to the executable,
    /// so the application stays portable: no installer, no registry and no profile folders.
    /// If the application folder is read-only (for example, Program Files),
    /// %LOCALAPPDATA%\Rule34.xxx Downloader is used instead.
    /// </summary>
    internal static class PortableSettings
    {
        #region Fields

        /// <summary>
        /// Name of the settings file.
        /// </summary>
        private const string FileName = "R34Downloader.settings.xml";

        /// <summary>
        /// Folder in %LOCALAPPDATA% that is used when the application folder is read-only.
        /// </summary>
        private const string FallbackFolderName = "Rule34.xxx Downloader";

        /// <summary>
        /// Company folder used by the previous versions of the application
        /// (their settings lived in %LOCALAPPDATA%\Dax Eleven\...\user.config).
        /// </summary>
        private const string PreviousCompanyFolderName = "Dax Eleven";

        private static string _filePath;

        #endregion

        #region Properties

        /// <summary>
        /// Folder that was used during the previous download.
        /// </summary>
        public static string DownloadPath { get; set; } = string.Empty;

        /// <summary>
        /// Id of the user account that owns the API key.
        /// </summary>
        public static string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Personal API key of the current user.
        /// </summary>
        public static string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Full path of the settings file.
        /// </summary>
        private static string FilePath => _filePath ?? (_filePath = ResolveFilePath());

        #endregion

        #region Methods

        /// <summary>
        /// Reads the settings file. A missing or damaged file only means default values.
        /// </summary>
        public static void Load()
        {
            DownloadPath = string.Empty;
            UserId = string.Empty;
            ApiKey = string.Empty;

            var path = FilePath;

            if (!File.Exists(path))
            {
                MigrateFromPreviousVersion();
                return;
            }

            try
            {
                var root = XDocument.Load(path).Root;

                if (root == null)
                {
                    return;
                }

                DownloadPath = ReadValue(root, "DownloadPath");
                UserId = ReadValue(root, "UserId");
                ApiKey = ReadValue(root, "ApiKey");
            }
            catch (Exception)
            {
                // A broken or unreadable settings file must not block the application.
            }
        }

        /// <summary>
        /// Writes the settings file.
        /// </summary>
        /// <exception cref="InvalidOperationException">The file cannot be written.</exception>
        public static void Save()
        {
            var path = FilePath;
            var temporaryPath = path + ".tmp";

            var document = new XDocument(new XDeclaration("1.0", "utf-8", null), new XElement("settings",
                new XElement("DownloadPath", DownloadPath ?? string.Empty),
                new XElement("UserId", UserId ?? string.Empty),
                new XElement("ApiKey", ApiKey ?? string.Empty)));

            try
            {
                document.Save(temporaryPath);
                File.Copy(temporaryPath, path, true);
            }
            catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException)
            {
                throw new InvalidOperationException("Could not save the settings to " + path + ": " + exception.Message, exception);
            }
            finally
            {
                TryDelete(temporaryPath);
            }
        }

        #endregion

        #region Helpers

        private static string ResolveFilePath()
        {
            var applicationFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!string.IsNullOrEmpty(applicationFolder) && IsWritable(applicationFolder))
            {
                return Path.Combine(applicationFolder, FileName);
            }

            var fallbackFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), FallbackFolderName);

            Directory.CreateDirectory(fallbackFolder);

            return Path.Combine(fallbackFolder, FileName);
        }

        private static bool IsWritable(string folder)
        {
            var probePath = Path.Combine(folder, Guid.NewGuid().ToString("N") + ".tmp");

            try
            {
                using (new FileStream(probePath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 1, FileOptions.DeleteOnClose))
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void MigrateFromPreviousVersion()
        {
            try
            {
                var previousFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), PreviousCompanyFolderName);
                if (!Directory.Exists(previousFolder))
                {
                    return;
                }

                foreach (var configFile in Directory.GetFiles(previousFolder, "user.config", SearchOption.AllDirectories))
                {
                    var root = XDocument.Load(configFile).Root;
                    if (root == null)
                    {
                        continue;
                    }

                    var downloadPath = ReadSetting(root, "Path");
                    if (downloadPath.Length == 0 || !Directory.Exists(downloadPath))
                    {
                        continue;
                    }

                    DownloadPath = downloadPath;
                    UserId = ReadSetting(root, "UserId");
                    ApiKey = ReadSetting(root, "ApiKey");
                    Save();

                    return;
                }
            }
            catch (Exception)
            {
                // The migration is a convenience only, never a reason to fail the start.
            }
        }

        private static string ReadValue(XElement root, string name)
        {
            var element = root.Element(name);

            return element == null ? string.Empty : element.Value.Trim();
        }

        private static string ReadSetting(XElement root, string name)
        {
            var setting = root.Descendants("setting").FirstOrDefault(x => string.Equals((string)x.Attribute("name"), name, StringComparison.OrdinalIgnoreCase));

            var value = setting?.Element("value")?.Value;

            return value == null ? string.Empty : value.Trim();
        }

        private static void TryDelete(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        #endregion
    }
}
