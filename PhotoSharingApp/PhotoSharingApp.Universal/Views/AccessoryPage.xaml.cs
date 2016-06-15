using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Serialization;
using PhotoSharingApp.Universal.Services;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AccessoryPage : BasePage
    {
        private ReturnAccessory Acessories { get; set; }
        private static ReturnUser CurrentUser { get; set; }
        readonly Authentication _authentication = new Authentication();
        private int _thumbnailImageSideLength;
        public AccessoryPage()
        {
            this.InitializeComponent();
            UpdateThumbnailSize();
            SizeChanged += AccessoryPage_SizeChanged;
            AuthenticationButton.Visibility = Visibility.Collapsed;
        }

        private void AccessoryPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateThumbnailSize();
        }

        public int ThumbnailImageSideLength
        {
            get { return _thumbnailImageSideLength; }
            set
            {
                if (value == _thumbnailImageSideLength) return;
                _thumbnailImageSideLength = value;
                NotifyPropertyChanged();
            }
        }
        private void UpdateThumbnailSize()
        {
            ThumbnailImageSideLength = PageRoot.ActualWidth > 1300 ? 500 : 300;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter == null) return;
            var args = SerializationHelper.Deserialize<ReturnAccessory>(e.Parameter as string);
            try
            {
                InitializeAccessoriesDetails(args.Id).Wait();

                ImagePath.Source = new BitmapImage(new Uri(Acessories.ImagePath));
                NameTextBlock.Text = Acessories.Name;
                SizeTextBlock.Text = Acessories.Size;
                ColorTextBlock.Text = Acessories.Color;
                StockQuantityTextBlock.Text = Acessories.StockQuantity.ToString();
                PriceTextBlock.Text = Acessories.Price.ToString(CultureInfo.InvariantCulture);
                NoConnectionGrid.Visibility = Visibility.Collapsed;

            }
            catch (Exception ex)
            {
                if (ex is TaskCanceledException || ex is AggregateException)
                {
                    NoConnectionGrid.Visibility = Visibility.Visible;
                }
            }
        }

        public async Task InitializeAccessoriesDetails(int id)
        {
            using (var client = new HttpClient())
            {
                var resourceLoader = ResourceLoader.GetForCurrentView();
                string serverUrl = resourceLoader.GetString("ServerURL");
                client.BaseAddress = new Uri(serverUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // New code:
                String apiUrl = "/api/Accessories/" + id;
                HttpResponseMessage response = await client.GetAsync(apiUrl).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    Acessories = await response.Content.ReadAsAsync<ReturnAccessory>();
                }
            }

        }

        private async void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            _authentication.GetCurrentUser();
            CurrentUser = _authentication.CurrentUser;
            if (CurrentUser != null)
            {
                int quantity = int.Parse(QuantityTextBox.Text.Trim());
                try
                {
                    AddToCart(CurrentUser, Acessories, quantity).Wait();
                    var dialog = new MessageDialog(
                    Acessories.Name + " has been added!!",
                    "Added");
                    await dialog.ShowAsync();
                }
                catch (Exception ex)
                {
                    if (ex is TaskCanceledException || ex is AggregateException)
                    {
                        var dialog = new MessageDialog("Can not connect to server!", "Message");
                        await dialog.ShowAsync();
                    }
                }
            }
            else
            {
                AuthenticationButton.Visibility = Visibility.Visible;
            }
        }


        private static async Task AddToCart(ReturnUser user, ReturnAccessory accessory, int quantity)
        {
            //request POST to api
            using (var client = new HttpClient())
            {
                var resourceLoader = ResourceLoader.GetForCurrentView();
                client.BaseAddress = new Uri(resourceLoader.GetString("ServerURL"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromMilliseconds(2000);
                AddToCart cart = new AddToCart
                {
                    Username = user.Username,
                    Password = user.Password,
                    AccessoryId = accessory.Id,
                    Quantity = quantity
                };

                HttpResponseMessage response = await client.PutAsJsonAsync("api/UserBuyingDetail/Add", cart).ConfigureAwait(false);
                //if (response.IsSuccessStatusCode)
                //{
                    
                //}
            }
        }

        private void AuthenticationButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SignInPage));
        }
    }
}
