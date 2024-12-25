using Microsoft.Win32;
using R34.Downloader.Application.Services;
using R34.Downloader.Domain.Models;
using R34.Downloader.Domain.Models.Api;
using R34.Downloader.UI.Windows.Mappers;
using R34.Downloader.UI.Windows.Models;
using R34.Downloader.UI.Windows.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;
using R34.Downloader.UI.Windows.Helpers;

namespace R34.Downloader.UI.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        private static readonly HttpClient HttpClient;

        private readonly MainWindowViewModel _viewModel;

        private readonly XmlSerializer _xmlSerializer;

        private WebsiteService _websiteService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        static MainWindow()
        {
            var socketsHandler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(1),
                AutomaticDecompression = DecompressionMethods.All
            };

            HttpClient = new HttpClient(socketsHandler)
            {
                DefaultRequestVersion = HttpVersion.Version20
            };
        }

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            var appsUseLightTheme = (int?)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 1);
            AppThemeHelper.ChangeTheme(appsUseLightTheme == 0
                ? new Uri("Resources/Themes/Dark.xaml", UriKind.Relative)
                : new Uri("Resources/Themes/Light.xaml", UriKind.Relative));

            InitializeComponent();
            _viewModel = GetDefaultDataContext();
            DataContext = _viewModel;

            _xmlSerializer = new XmlSerializer(typeof(PostsModel));

            _ = CreateWebsiteServiceAsync();

            Observable.FromEventPattern<TextChangedEventHandler, TextChangedEventArgs>(
                    x => SearchTextBox.TextChanged += x,
                    x => SearchTextBox.TextChanged -= x)
                .Select(_ => SearchTextBox.Text.TrimStart())
                .Sample(TimeSpan.FromMilliseconds(500))
                .DistinctUntilChanged()
                .Select(SearchComboBoxAutocompleteAsync)
                .Subscribe();

            Observable.FromEventPattern<TextChangedEventHandler, TextChangedEventArgs>(
                    x => SearchTextBox.TextChanged += x,
                    x => SearchTextBox.TextChanged -= x)
                .Select(_ => SearchTextBox.Text.Trim())
                .Sample(TimeSpan.FromSeconds(1))
                .DistinctUntilChanged()
                .Select(MaxQuantityLabelUpdateAsync)
                .Subscribe();
        }

        #endregion

        #region Controls

        #region SearchTextBox + SearchListBox + SearchPopup

        private void SearchTextBox_OnGotFocus(object sender, RoutedEventArgs e) => UpdateVisibilityOfSearchPopup();
        private void SearchTextBox_OnLostFocus(object sender, RoutedEventArgs e) => UpdateVisibilityOfSearchPopup();
        private void SearchTextBox_OnMouseDown(object sender, MouseButtonEventArgs e) => UpdateVisibilityOfSearchPopup();

        private TagAutocompleteModel[] _autocompleteResult;

        private void SearchListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTag = (TagAutocompleteModel)SearchListBox.SelectedItem;
            if (selectedTag != null)
            {
                var lastSpaceIndex = SearchTextBox.Text.LastIndexOf(' ');
                SearchTextBox.Text = lastSpaceIndex != -1 ? $"{SearchTextBox.Text[..(lastSpaceIndex + 1)]}{selectedTag.Value} " : $"{selectedTag.Value} ";
            }
        }

        private Task SearchComboBoxAutocompleteAsync(string searchText)
        {
            if (!string.IsNullOrEmpty(searchText))
            {
                var lastTag = searchText.Split(' ').Last()
                    .TrimStart('-');

                _autocompleteResult = Array.Empty<TagAutocompleteModel>();

                SearchListBox.Dispatcher.InvokeAsync(async () =>
                {
                    if (lastTag != string.Empty)
                    {
                        _autocompleteResult = await _websiteService.GetAutocompleteResultAsync(new SearchPhrase(lastTag));
                        if (_autocompleteResult.Length == 1 && lastTag == _autocompleteResult[0].Value)
                        {
                            _autocompleteResult = Array.Empty<TagAutocompleteModel>();
                        }
                    }

                    SearchListBox.ItemsSource = _autocompleteResult;
                    UpdateVisibilityOfSearchPopup();
                });
            }

            return Task.CompletedTask;
        }
        
        private void UpdateVisibilityOfSearchPopup()
        {
            SearchPopup.IsOpen = SearchTextBox.IsFocused && _autocompleteResult != null && _autocompleteResult.Length != 0;
        }

        #endregion

        #region MaxQuantityLabel

        private Task MaxQuantityLabelUpdateAsync(string searchText)
        {
            if (!string.IsNullOrEmpty(searchText))
            {
                _viewModel.SearchPhrase = new SearchPhrase(searchText);

                MaxQuantityLabel.Dispatcher.InvokeAsync(async () =>
                {
                    _viewModel.MaxQuantity = await _websiteService.GetContentCountAsync(_viewModel.SearchPhrase);
                });
            }

            return Task.CompletedTask;
        }

        #endregion

        #region OpenInBrowserButton

        private void OpenInBrowserButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = $"https://rule34.xxx/index.php?page=post&s=list&tags={_viewModel.SearchPhrase}"
            });
        }

        #endregion

        #region HelpButton

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = "https://rule34.xxx/index.php?page=help&topic=cheatsheet"
            });
        }

        #endregion

        #region OpenButton

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", _viewModel.SavePath);
        }

        #endregion

        #region SelectButton

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            var openFolderDialog = new OpenFolderDialog
            {
                Title = "Select the folder for downloaded content",
                InitialDirectory = _viewModel.SavePath
            };

            if (openFolderDialog.ShowDialog() == true)
            {
                _viewModel.SavePath = openFolderDialog.FolderName;
            }
        }

        #endregion

        #region DownloadButton

        private Task _downloadTask;
        private CancellationTokenSource _downloadTaskCancellationToken;

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (_downloadTask != null)
            {
                await _downloadTaskCancellationToken.CancelAsync();

                DownloadedNumberLabel.Content = "0";
                SkippedNumberLabel.Content = "0";
                DownloadProgressBar.Value = default;

                return;
            }

            var progress = new Progress<DownloadStatus>(x =>
            {
                DownloadedNumberLabel.Content = x.Downloaded.ToString();
                SkippedNumberLabel.Content = x.Skipped.ToString();
                DownloadProgressBar.Value = x.Downloaded + x.Skipped;
            });

            _downloadTaskCancellationToken = new CancellationTokenSource();
            _downloadTask = _websiteService.DownloadContentAsync(_viewModel.ToSettings(), progress, _downloadTaskCancellationToken.Token);

            DownloadButton.Content = "Stop";

            try
            {
                await _downloadTask;

                DownloadButton.Content = "Download";
            }
            catch (Exception)
            {
                _downloadTaskCancellationToken.Dispose();
                _downloadTask.Dispose();
                _downloadTask = null;

                DownloadButton.Content = "Download";
            }
        }

        #endregion

        #region AboutLabel

        private void AboutLabel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = "https://github.com/DaxEleven/Rule34.xxx-Downloader/releases"
            });
        }

        #endregion

        #endregion

        #region Helpers

        private Task CreateWebsiteServiceAsync()
        {
            return Task.Run(async () =>
            {
                _websiteService = await WebsiteServiceFactory.GetWebsiteServiceAsync(HttpClient, _xmlSerializer);
                if (_websiteService == null)
                {
                    throw new ArgumentNullException();
                }

                if (_websiteService is not WebsiteApiService)
                {
                    throw new ArgumentNullException();
                }
            });
        }

        #region Settings

        private const string AppSettingsPath = "R34.Settings.json";

        private static MainWindowViewModel GetDefaultDataContext()
        {
            var viewModel = new MainWindowViewModel
            {
                IncludeImages = true,
                IncludeGifs = true,
                IncludeVideo = true,
                SavePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (File.Exists(AppSettingsPath))
            {
                try
                {
                    var appSettings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(AppSettingsPath));
                    if (appSettings != null)
                    {
                        if (!string.IsNullOrEmpty(appSettings.SavePath))
                        {
                            viewModel.SavePath = appSettings.SavePath;
                        }
                    }
                }
                catch (Exception)
                {
                    // Ignored
                }
            }

            return viewModel;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var appSettings = new AppSettings
            {
                SavePath = _viewModel.SavePath
            };

            File.WriteAllText(AppSettingsPath, JsonSerializer.Serialize(appSettings, new JsonSerializerOptions { WriteIndented = true }));
        }



        #endregion

        #endregion


    }
}
