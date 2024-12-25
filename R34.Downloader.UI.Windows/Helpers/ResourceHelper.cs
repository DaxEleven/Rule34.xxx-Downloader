namespace R34.Downloader.UI.Windows.Helpers
{
    /// <summary>
    /// Application resource helper.
    /// </summary>
    internal static class ResourceHelper
    {
        #region Methods

        /// <summary>
        /// Returns the requested resource.
        /// </summary>
        /// <typeparam name="T">Resource type.</typeparam>
        /// <param name="resourceName">Resource name.</param>
        /// <returns>Resource.</returns>
        internal static T GetResource<T>(string resourceName) where T : class
        {
            return System.Windows.Application.Current.TryFindResource(resourceName) as T;
        }

        #endregion
    }
}
