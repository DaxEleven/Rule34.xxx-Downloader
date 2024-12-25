using R34.Downloader.Domain.Models;

namespace R34.Downloader.UI.Windows.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Fields

        private uint _maxQuantity;

        private bool _includeImages, _includeGifs, _includeVideo;

        private bool _downloadContentInMaxQuality;

        private string _savePath;

        private uint _quantitySliderMaximum, _quantity;

        #endregion

        #region Properties

        /// <summary>
        /// Search tags.
        /// </summary>
        public SearchPhrase SearchPhrase { get; set; }

        /// <summary>
        /// Maximum amount of content searched.
        /// </summary>
        public uint MaxQuantity
        {
            get => _maxQuantity;
            set
            {
                SetField(ref _maxQuantity, value);
                QuantitySliderMaximum = value > ushort.MaxValue ? ushort.MaxValue : value;
            }
        }

        /// <summary>
        /// Whether to include images in downloaded content.
        /// </summary>
        public bool IncludeImages
        {
            get => _includeImages;
            set => SetField(ref _includeImages, value);
        }

        /// <summary>
        /// Whether to include gifs in downloaded content.
        /// </summary>
        public bool IncludeGifs
        {
            get => _includeGifs;
            set => SetField(ref _includeGifs, value);
        }

        /// <summary>
        /// Whether to include video in downloaded content.
        /// </summary>
        public bool IncludeVideo
        {
            get => _includeVideo;
            set => SetField(ref _includeVideo, value);
        }

        /// <summary>
        /// Download content in maximum quality.
        /// </summary>
        public bool DownloadContentInMaxQuality
        {
            get => _downloadContentInMaxQuality;
            set => SetField(ref _downloadContentInMaxQuality, value);
        }

        /// <summary>
        /// Content save path.
        /// </summary>
        public string SavePath
        {
            get => _savePath;
            set => SetField(ref _savePath, value);
        }

        /// <summary>
        /// Content save path.
        /// </summary>
        public uint QuantitySliderMaximum
        {
            get => _quantitySliderMaximum;
            set => SetField(ref _quantitySliderMaximum, value);
        }

        /// <summary>
        /// The amount of downloaded content.
        /// </summary>
        /// <remarks>Maximum 65535.</remarks>
        public uint Quantity
        {
            get => _quantity;
            set
            {
                if (value > MaxQuantity)
                {
                    value = MaxQuantity;
                }

                if (value > ushort.MaxValue)
                {
                    value = ushort.MaxValue;
                }

                if (value == 0)
                {
                    value = 1;
                }

                SetField(ref _quantity, value);
            }
        }

        #endregion
    }
}
