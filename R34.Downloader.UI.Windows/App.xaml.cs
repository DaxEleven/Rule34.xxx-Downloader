using System;
using System.Windows;

namespace R34.Downloader.UI.Windows
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void App_OnDeactivated(object sender, EventArgs e)
        {
            foreach (Window window in Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    ((MainWindow)window).SearchPopup.IsOpen = false;
                }
            }
        }
    }
}
