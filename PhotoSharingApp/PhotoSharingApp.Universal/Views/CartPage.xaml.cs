using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Services;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CartPage : Page
    {
        private List<ReturnBuyingDetail> BuyingDetails { get; set; }
        private static ReturnUser CurrentUser { get; set; }
        public CartPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Authentication authentication = new Authentication();
            authentication.GetCurrentUser();
            CurrentUser = authentication.CurrentUser;
            try
            {
                if (CurrentUser != null)
                {
                    InitCartDetail(CurrentUser).Wait();
                    CartListView.ItemsSource = BuyingDetails;
                    NoConnectionGrid.Visibility = Visibility.Collapsed;
                }
                else
                {
                    // Navigate to Login page
                    ErrorTextBlock.Text = "You must login first!";
                    NoConnectionGrid.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                if (ex is AggregateException || ex is TimeoutException)
                {
                    NoConnectionGrid.Visibility = Visibility.Visible;
                }
            }
        }

        public async Task InitCartDetail(ReturnUser user)
        {
            using (var client = new HttpClient())
            {
                var resourceLoader = ResourceLoader.GetForCurrentView();
                string serverUrl = resourceLoader.GetString("ServerURL");
                client.BaseAddress = new Uri(serverUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromMilliseconds(2000);

                HttpResponseMessage response = await client.PostAsJsonAsync("/api/CartDetail", user).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    BuyingDetails = await response.Content.ReadAsAsync<List<ReturnBuyingDetail>>();
                }
            }
        }
        
    }
}
