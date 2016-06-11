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
using PhotoSharingApp.Universal.Serialization;
using PhotoSharingApp.Universal.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CategoryPage : Windows.UI.Xaml.Controls.Page
    {
        private int _thumbnailImageSideLength;
        private readonly CategoryPageViewModel _viewModel;
        private ReturnAccessoryCombination Acessories { get; set; }
        public CategoryPage()
        {
            this.InitializeComponent();
            SizeChanged += CategoryPage_SizeChanged;
            _viewModel = ServiceLocator.Current.GetInstance<CategoryPageViewModel>();
            DataContext = _viewModel;
        }

        private void CategoryPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateThumbnailSize();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var args = SerializationHelper.Deserialize<ReturnAccessoryCombination>(e.Parameter as string);
            InitializeAccessoriesDetails(args.Category.Id).Wait();
            AccessoriesListView.ItemsSource = Acessories.ListOfAccessory;
            base.OnNavigatedTo(e);
            await _viewModel.LoadState();
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
        public int ThumbnailImageSideLength
        {
            get { return _thumbnailImageSideLength; }
            set
            {
                if (value != _thumbnailImageSideLength)
                {
                    _thumbnailImageSideLength = value;
                    //NotifyPropertyChanged();
                }
            }
        }
        private void UpdateThumbnailSize()
        {
            if (PageRoot.ActualWidth > 1300)
            {
                ThumbnailImageSideLength = 150;
            }
            else
            {
                ThumbnailImageSideLength = 100;
            }
        }
    }
}
