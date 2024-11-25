using System;
using Weathernews.Sensor;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WxBeaconApp
{
    /// <summary>
    /// サンプル用簡易表示ページ
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private WxBeacon2Watcher wxBeacon2Watcher = new WxBeacon2Watcher();

        public MainPage()
        {
            InitializeComponent();
            wxBeacon2Watcher.Received += WxBeacon2Watcher_Found;
        }

        private async void WxBeacon2Watcher_Found(object sender, WxBeacon2 beacon)
        {
            var latest = await beacon.GetLatestDataAsync();
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                textBlock.Text = latest.ToString();
            });
            beacon.Dispose();
            wxBeacon2Watcher.Dispose();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            wxBeacon2Watcher.Start();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            wxBeacon2Watcher.Stop();
        }
    }
}
