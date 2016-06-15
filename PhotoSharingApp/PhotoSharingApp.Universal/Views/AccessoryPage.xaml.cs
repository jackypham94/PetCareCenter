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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AccessoryPage : Page
    {
        private ReturnAccessory Acessories { get; set; }
        private static ReturnUser User { get; set; }
        public AccessoryPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                var args = SerializationHelper.Deserialize<ReturnAccessory>(e.Parameter as string);
                try
                {
                    InitializeAccessoriesDetails(args.Id).Wait();

                    Image.Source = new BitmapImage(new Uri(Acessories.ImagePath));
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

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            DeserelizeDataFromJson("user");
            int quantity = Int32.Parse(QuantityTextBox.Text.Trim());
            AddToCart(User, Acessories, quantity);
        }


        private async void AddToCart(ReturnUser user, ReturnAccessory accessory, int quantity)
        {
            //request POST to api
            using (var client = new HttpClient())
            {
                var resourceLoader = ResourceLoader.GetForCurrentView();
                client.BaseAddress = new Uri(resourceLoader.GetString("ServerURL"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromMilliseconds(2000);
                AddToCart cart = new AddToCart()
                {
                    Username = user.Username,
                    Password = user.Password,
                    AccessoryId = accessory.Id,
                    Quantity = quantity
                };

                try
                {
                    HttpResponseMessage response = await client.PutAsJsonAsync("api/UserBuyingDetail/Add", cart);
                    if (response.IsSuccessStatusCode)
                    {
                        var dialog = new MessageDialog(
                        "You have been add ID: " + accessory.Id + " Name: " + accessory.Name,
                        "Congratulation");
                        await dialog.ShowAsync();
                    }

                }
                catch (HttpRequestException)
                {
                    var dialog = new MessageDialog("Can not connect to server!", "Message");
                    await dialog.ShowAsync();
                }

            }
        }

        public void DeserelizeDataFromJson(string fileName)
        {
            User = new ReturnUser();
            var folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var filePath = folder.Path + @"\" + fileName + ".json";
            using (StreamReader file = File.OpenText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                User = (ReturnUser)serializer.Deserialize(file, typeof(ReturnUser));
                User.Password = Base64Decode(User.Password);
            }
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
