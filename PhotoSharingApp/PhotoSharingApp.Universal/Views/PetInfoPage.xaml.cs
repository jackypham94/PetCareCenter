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
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Serialization;
using PhotoSharingApp.Universal.Services;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PetInfoPage : BasePage
    {
        private static ReturnUser CurrentUser { get; set; }
        private ReturnPet Pet { get; set; }
        private int _thumbnailImageSideLength;
        public PetInfoPage()
        {
            this.InitializeComponent();
            UpdateThumbnailSize();
            SizeChanged += PetInfoPage_SizeChanged;
        }

        private void PetInfoPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateThumbnailSize();
        }

        public int ThumbnailImageSideLength
        {
            get { return _thumbnailImageSideLength; }
            set
            {
                if (value != _thumbnailImageSideLength)
                {
                    _thumbnailImageSideLength = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private void UpdateThumbnailSize()
        {
            if (PageRoot.ActualWidth > 1300)
            {
                ThumbnailImageSideLength = 500;
            }
            else
            {
                ThumbnailImageSideLength = 300;
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            Authentication authentication = new Authentication();
            authentication.GetCurrentUser();
            CurrentUser = authentication.CurrentUser;
            if (e.Parameter != null && CurrentUser !=null)
            {
                var args = SerializationHelper.Deserialize<ReturnPet>(e.Parameter as string);
                try
                {
                    InitializePetInfo(CurrentUser,args.Id).Wait();
                    GetPetCareType().Wait();
                    PetCareTypeComboBox.ItemsSource = PetCareType;
                    PetCareTypeComboBox.SelectedIndex = 0;
                    ImagePath.Source = new BitmapImage(new Uri(Pet.ImagePath));
                    NameTextBlock.Text = Pet.Name;
                    AgeTextBlock.Text = Pet.Age.ToString();
                    GenderTextBlock.Text = Pet.Gender == 0 ? "Male" : "Female";
                    StatusTextBlock.Text = Pet.Status;
                    TypeTextBlock.Text = Pet.PetCategory.Name;
                    if (Pet.IsDepositing)
                    {
                        DepositGrid.Visibility = Visibility.Visible;
                        DepositPetButton.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        DepositGrid.Visibility = Visibility.Collapsed;
                        DepositPetButton.Visibility = Visibility.Visible;
                    }
                }
                catch (Exception ex)
                {
                    if (ex is AggregateException || ex is TaskCanceledException)
                    {
                        var dialog = new MessageDialog("Can not connect to server!", "Message");
                        await dialog.ShowAsync();
                    }
                }
            }
        }
        public async Task InitializePetInfo(ReturnUser user, int id)
        {
            using (var client = new HttpClient())
            {
                var resourceLoader = ResourceLoader.GetForCurrentView();
                string serverUrl = resourceLoader.GetString("ServerURL");
                client.BaseAddress = new Uri(serverUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // New code:
                String apiUrl = "/api/Pets/" + id;
                HttpResponseMessage response = await client.PostAsJsonAsync(apiUrl, user).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    Pet = await response.Content.ReadAsAsync<ReturnPet>();
                }
            }

        }

        private List<PetCareType> PetCareType { get; set; }
        public async Task GetPetCareType()
        {
            using (var client = new HttpClient())
            {
                var resourceLoader = ResourceLoader.GetForCurrentView();
                string serverUrl = resourceLoader.GetString("ServerURL");
                client.BaseAddress = new Uri(serverUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromMilliseconds(2000);

                String apiUrl = "/api/PetCareTypes";
                HttpResponseMessage response = await client.GetAsync(apiUrl).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    PetCareType = await response.Content.ReadAsAsync<List<PetCareType>>();
                }
            }
        }

        private async void DepositPetButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new MessageDialog("Deposited!", "Welldone");
            await dialog.ShowAsync();
        }
    }
}
