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
using PhotoSharingApp.Universal.Serialization;
using PhotoSharingApp.Universal.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Categories : Windows.UI.Xaml.Controls.Page
    {
        private ReturnAccessoryCombination Acessories { get; set; }
        public Categories()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                var args = SerializationHelper.Deserialize<ReturnAccessory>(e.Parameter as string);
                //var args = e.Parameter.ToString();
                int id = args.Id;
                string name = args.Name;
                try
                {
                    //InitializeAccessoriesDetails(id).Wait();
                    var dialog = new Windows.UI.Popups.MessageDialog(
                        "You have been clicked ID: " + id + " Name: " + name,
                        "Lorem Ipsum");
                    await dialog.ShowAsync();
                }
                catch (AggregateException)
                {
                }
                if (Acessories == null)
                {
                    NoConnectionGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    NoConnectionGrid.Visibility = Visibility.Collapsed;
                    CategoryListView.ItemsSource = Acessories.ListOfAccessory;
                    TitleTextBlock.Text = Acessories.Category.CategoryName.ToUpper();
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
                String apiUrl = "/api/AccessoryCategoriesFull/" + id;
                HttpResponseMessage response = await client.GetAsync(apiUrl).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    Acessories = await response.Content.ReadAsAsync<ReturnAccessoryCombination>();
                }
            }

        }
    }
}
