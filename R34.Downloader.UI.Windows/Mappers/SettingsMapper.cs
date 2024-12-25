using R34.Downloader.Domain.Models;
using R34.Downloader.UI.Windows.ViewModels;

namespace R34.Downloader.UI.Windows.Mappers
{
    internal static class SettingsMapper
    {
        internal static Settings ToSettings(this MainWindowViewModel mainWindowViewModel)
        {
            return new Settings
            {
                DownloadContentInMaxQuality = mainWindowViewModel.DownloadContentInMaxQuality,
                IncludeImages = mainWindowViewModel.IncludeImages,
                IncludeVideo = mainWindowViewModel.IncludeVideo,
                IncludeGifs = mainWindowViewModel.IncludeGifs,
                SavePath = mainWindowViewModel.SavePath,
                Quantity = (ushort)mainWindowViewModel.Quantity,
                SearchPhrase = mainWindowViewModel.SearchPhrase
            };
        }
    }
}
