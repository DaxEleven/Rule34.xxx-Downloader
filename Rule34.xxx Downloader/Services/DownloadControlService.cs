namespace R34Downloader.Services
{
    /// <summary>
    /// Controls a running download. Stopping is cooperative: the file that is
    /// already being transferred is completed first, then the loop stops.
    /// </summary>
    public static class DownloadControlService
    {
        #region Fields

        private static volatile bool _stopRequested;

        #endregion

        #region Properties

        /// <summary>
        /// True when the user asked to stop after the current file.
        /// </summary>
        public static bool StopRequested => _stopRequested;

        #endregion

        #region Methods

        /// <summary>
        /// Clears the stop flag before a new download starts.
        /// </summary>
        public static void Reset()
        {
            _stopRequested = false;
        }

        /// <summary>
        /// Requests a stop; the file in progress is finished first.
        /// </summary>
        public static void RequestStop()
        {
            _stopRequested = true;
        }

        #endregion
    }
}
