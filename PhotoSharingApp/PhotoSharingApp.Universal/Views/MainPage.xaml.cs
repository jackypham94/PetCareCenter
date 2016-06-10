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
        //private MainPageViewModel _viewModel;
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
                Category1ItemsControl.ItemsSource = AccessoryCombinations[0].ListOfAccessory;
                Category1TextBlock.Text = AccessoryCombinations[0].Category.CategoryName;
                Category2ItemsControl.ItemsSource = AccessoryCombinations[1].ListOfAccessory;
                Category2TextBlock.Text = AccessoryCombinations[1].Category.CategoryName;
                Category3ItemsControl.ItemsSource = AccessoryCombinations[2].ListOfAccessory;
                Category3TextBlock.Text = AccessoryCombinations[2].Category.CategoryName;
                //Category4ItemsControl.ItemsSource = AccessoryCombinations[3].ListOfAccessory;
                //Category4TextBlock.Text = AccessoryCombinations[3].Category.CategoryName;
                ImagesFlipView.ItemsSource = AccessoryCombinations[0].ListOfAccessory;
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

        private void ShowCategory1Button_Click(object sender, RoutedEventArgs e)
        {
            int id = AccessoryCombinations[0].Category.Id;
            Frame.Navigate(typeof(Categories), id);
        }

        private void ShowCategory2Button_Click(object sender, RoutedEventArgs e)
        {
            int id = AccessoryCombinations[1].Category.Id;
            Frame.Navigate(typeof(Categories), id);
        }

        private void ShowCategory3Button_Click(object sender, RoutedEventArgs e)
        {
            int id = AccessoryCombinations[2].Category.Id;
            Frame.Navigate(typeof(Categories), id);
        }

        //private void ShowCategory4Button_Click(object sender, RoutedEventArgs e)
        //{
        //    int id = AccessoryCombinations[3].Category.Id;
        //    Frame.Navigate(typeof(Categories), id);
        //}
    }
}
