using System;
using System.Windows;

namespace R34.Downloader.UI.Windows.Helpers
{
    internal static class AppThemeHelper
    {
        public static void ChangeTheme(Uri themeUri)
        {
            //System.Windows.Application.Current.Resources.Clear();
            System.Windows.Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = themeUri });
        }
    }
}
