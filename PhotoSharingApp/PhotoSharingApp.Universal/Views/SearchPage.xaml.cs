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
using PhotoSharingApp.Universal.Facades;
using PhotoSharingApp.Universal.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchPage : Page
    {
        private INavigationFacade _navigationFacade = new NavigationFacade();
        private List<ReturnAccessoryCombination> AccessoryCombinations { get; set; }
        private List<ReturnAccessory> Accessories { get; set; }
        public SearchPage()
        {
            this.InitializeComponent();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string nameSearch = SearchTextBox.Text.Trim();
            if (TypeComboBox.SelectedIndex == 0)
            {
                try
                {
                    SearchByAccessoryName(nameSearch).Wait();
                    AccessoryListView.ItemsSource = Accessories;
                    CategoryListView.ItemsSource = null;
                    if (Accessories == null)
                    {
                        SearchResultTextBlock.Text = "NOT FOUND";
                    }
                    else
                    {
                        SearchResultTextBlock.Text = "";
                    }
                    NoConnectionGrid.Visibility = Visibility.Collapsed;
                }
                catch (AggregateException)
                {
                    NoConnectionGrid.Visibility = Visibility.Visible;
                }

            }
            else
            {
                try
                {
                    SearchByCategoryName(nameSearch).Wait();
                    AccessoryListView.ItemsSource = null;
                    CategoryListView.ItemsSource = AccessoryCombinations;
                    if (AccessoryCombinations == null)
                    {
                        SearchResultTextBlock.Text = "NOT FOUND";
                    }
                    else
                    {
                        SearchResultTextBlock.Text = "";
                    }
                    NoConnectionGrid.Visibility = Visibility.Collapsed;
                }
                catch (AggregateException)
                {
                    NoConnectionGrid.Visibility = Visibility.Visible;
                }

            }
        }


        public async Task SearchByCategoryName(string name)
        {
            using (var client = new HttpClient())
            {
                var resourceLoader = ResourceLoader.GetForCurrentView();
                string serverUrl = resourceLoader.GetString("ServerURL");
                client.BaseAddress = new Uri(serverUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromMilliseconds(2000);

                String apiUrl = "/api/AccessoryCategoriesSearch/" + name;
                HttpResponseMessage response = await client.GetAsync(apiUrl).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    AccessoryCombinations = await response.Content.ReadAsAsync<List<ReturnAccessoryCombination>>();
                }
            }
        }

        public async Task SearchByAccessoryName(string name)
        {
            using (var client = new HttpClient())
            {
                var resourceLoader = ResourceLoader.GetForCurrentView();
                string serverUrl = resourceLoader.GetString("ServerURL");
                client.BaseAddress = new Uri(serverUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromMilliseconds(2000);

                String apiUrl = "/api/AccessoriesSearch/" + name;
                HttpResponseMessage response = await client.GetAsync(apiUrl).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    Accessories = await response.Content.ReadAsAsync<List<ReturnAccessory>>();
                }
            }
        }

        private void AccessoryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _navigationFacade.NavigateToAccessoryDetail(Accessories[AccessoryListView.SelectedIndex]);
        }

        private void CategoryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _navigationFacade.NavigateToCategoryPage(AccessoryCombinations[CategoryListView.SelectedIndex]);
        }
    }
}