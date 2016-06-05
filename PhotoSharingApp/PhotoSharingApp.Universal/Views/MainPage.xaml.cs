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
using Microsoft.Practices.ServiceLocation;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<ReturnAccessoryCombination> AccessoryCombinations { get;  set; }
        public MainPage()
        {
            this.InitializeComponent();
            InitializeItem();
        }

        private void InitializeItem()
        {
            try
            {
                InitializeCategoryItems().Wait();
            }
            catch (AggregateException)
            {
                
            }
            DataContext = AccessoryCombinations;
            if (AccessoryCombinations == null)
            {
                NoConnectionGrid.Visibility = Visibility.Visible;
            }
            else 
            {
                NoConnectionGrid.Visibility = Visibility.Collapsed;
                CategoryListView.ItemsSource = AccessoryCombinations;
            }
        }
        public async Task InitializeCategoryItems()
        {
            using (var client = new HttpClient())
            {
                var resourceLoader = ResourceLoader.GetForCurrentView();
                string serverUrl = resourceLoader.GetString("ServerURL");
                client.BaseAddress = new Uri(serverUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // New code:
                HttpResponseMessage response = await client.GetAsync("/api/AccessoryCategoriesDisplay").ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    AccessoryCombinations = await response.Content.ReadAsAsync<List<ReturnAccessoryCombination>>();
                }
            }

        }

        //protected override async void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    base.OnNavigatedTo(e);
        //    await _viewModel.LoadState();
        //}

        private void ViewAllTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            int id = AccessoryCombinations[CategoryListView.SelectedIndex].Category.Id;
            Frame.Navigate(typeof(Categories), id);
        }


        private async void AcessoryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dialog = new ContentDialog()
            {
                //Title = AccessoryCombinations[CategoryListView.SelectedIndex].ListOfAccessory[],
                MaxWidth = this.ActualWidth // Required for Mobile!
            };

            //AcessoryListView
            dialog.PrimaryButtonText = "OK";
            //dialog.IsPrimaryButtonEnabled = false;
            //dialog.PrimaryButtonClick += delegate {
            //};

            var result = await dialog.ShowAsync();
        }
    }
}
