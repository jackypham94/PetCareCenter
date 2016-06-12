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
using PhotoSharingApp.Universal.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MyPetPage : Page
    {
        private List<ReturnPetCategory> PetCategory { get; set; }
        public MyPetPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            InitData();
        }


        private void InitData()
        {
            try
            {
                GetListCategory().Wait();
                //CategoriesComboBox.ItemsSource = PetCategory;
                //CategoriesComboBox.SelectedIndex = 0;
                NoConnectionGrid.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                if (ex is TimeoutException || ex is AggregateException)
                {
                    NoConnectionGrid.Visibility = Visibility.Visible;
                }

            }
        }

        public async Task GetListCategory()
        {
            using (var client = new HttpClient())
            {
                var resourceLoader = ResourceLoader.GetForCurrentView();
                string serverUrl = resourceLoader.GetString("ServerURL");
                client.BaseAddress = new Uri(serverUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromMilliseconds(2000);

                String apiUrl = "/api/PetCategories";
                HttpResponseMessage response = await client.GetAsync(apiUrl).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    PetCategory = await response.Content.ReadAsAsync<List<ReturnPetCategory>>();
                }
            }
        }
    }
}
