using R34.Downloader.UI.Windows.Helpers;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace R34.Downloader.UI.Windows.Converters
{
    /// <summary>
    /// Tag type to color converter.
    /// </summary>
    internal class TagTypeConverter : IValueConverter
    {
        #region Methods

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string tagType)
            {
                return tagType switch
                {
                    "metadata" => ResourceHelper.GetResource<SolidColorBrush>("SearchComboBoxItemFontColorMetadata"),
                    "copyright" => ResourceHelper.GetResource<SolidColorBrush>("SearchComboBoxItemFontColorCopyright"),
                    "character" => ResourceHelper.GetResource<SolidColorBrush>("SearchComboBoxItemFontColorCharacter"),
                    "artist" => ResourceHelper.GetResource<SolidColorBrush>("SearchComboBoxItemFontColorArtist"),
                    _ => ResourceHelper.GetResource<SolidColorBrush>("FontColor")
                };
            }

            return ResourceHelper.GetResource<SolidColorBrush>("FontColor");
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
