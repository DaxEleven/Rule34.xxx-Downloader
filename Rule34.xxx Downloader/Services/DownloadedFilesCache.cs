using System;
using System.Collections.Generic;
using System.IO;

namespace R34Downloader.Services
{
    /// <summary>
    /// In-memory cache of the files that already exist in the download folders.
    /// </summary>
    internal static class DownloadedFilesCache
    {
        #region Fields

        private static readonly object SyncRoot = new object();

        private static readonly Dictionary<string, HashSet<string>> FileNamesByFolder = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Methods

        /// <summary>
        /// Forgets everything that was cached. Called before a new download run, so files deleted or added between the runs are seen correctly.
        /// </summary>
        public static void Reset()
        {
            lock (SyncRoot)
            {
                FileNamesByFolder.Clear();
            }
        }

        /// <summary>
        /// True when a non-empty file with the same name already exists in the folder of the path.
        /// </summary>
        /// <param name="filePath">Path where the file would be saved.</param>
        /// <returns>True when the file is already on disk and can be skipped.</returns>
        public static bool Contains(string filePath)
        {
            if (!TrySplit(filePath, out var folder, out var fileName))
            {
                return false;
            }

            lock (SyncRoot)
            {
                return GetFileNames(folder).Contains(fileName);
            }
        }

        /// <summary>
        /// Remembers a file that has just been saved, so the next check does not need the disk.
        /// </summary>
        /// <param name="filePath">Path of the saved file.</param>
        public static void Register(string filePath)
        {
            if (!TrySplit(filePath, out var folder, out var fileName))
            {
                return;
            }

            lock (SyncRoot)
            {
                GetFileNames(folder).Add(fileName);
            }
        }

        #endregion

        #region Helpers

        private static bool TrySplit(string filePath, out string folder, out string fileName)
        {
            folder = string.Empty;
            fileName = string.Empty;

            if (string.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }

            try
            {
                var directory = Path.GetDirectoryName(filePath);

                folder = Path.GetFullPath(string.IsNullOrEmpty(directory) ? "." : directory);
                fileName = Path.GetFileName(filePath);
            }
            catch (Exception exception) when (exception is ArgumentException || exception is NotSupportedException || exception is PathTooLongException)
            {
                return false;
            }

            return fileName.Length > 0;
        }

        private static HashSet<string> GetFileNames(string folder)
        {
            if (FileNamesByFolder.TryGetValue(folder, out var fileNames))
            {
                return fileNames;
            }

            fileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                if (Directory.Exists(folder))
                {
                    foreach (var file in new DirectoryInfo(folder).EnumerateFiles())
                    {
                        // Empty files are leftovers and unfinished ".part" files belong to this very post: neither may be mistaken for a finished download
                        if (file.Length > 0 && !file.Name.EndsWith(DownloadService.TemporaryFileSuffix, StringComparison.OrdinalIgnoreCase))
                        {
                            fileNames.Add(file.Name);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // An unreadable folder must not break the download: nothing is cached, so every file of that folder is simply downloaded again
            }

            FileNamesByFolder[folder] = fileNames;

            return fileNames;
        }

        #endregion
    }
}
