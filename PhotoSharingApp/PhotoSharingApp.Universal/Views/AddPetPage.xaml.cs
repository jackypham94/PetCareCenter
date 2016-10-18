using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using PhotoSharingApp.Universal.Facades;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Services;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddPetPage : Page
    {
        private readonly INavigationFacade _navigationFacade = new NavigationFacade();
        private AppEnvironment environment = new AppEnvironment();
        private List<ReturnPetCategory> PetCategory { get; set; }
        private static ReturnUser CurrentUser { get; set; }
        public AddPetPage()
        {
            this.InitializeComponent();
            InputPane.GetForCurrentView().Showing += ItemDetailPage_Showing;
            InputPane.GetForCurrentView().Hiding += ItemDetailPage_Hiding;
        }

        private void ItemDetailPage_Hiding(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            LayoutRoot.Margin = new Thickness(0);
            args.EnsuredFocusedElementInView = true;
        }

        private void ItemDetailPage_Showing(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            LayoutRoot.Margin = new Thickness(0, 0, 0, args.OccludedRect.Height);
            args.EnsuredFocusedElementInView = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Authentication authentication = new Authentication();
            authentication.GetCurrentUser();
            CurrentUser = authentication.CurrentUser;
            if (CurrentUser == null)
            {
                NoConnectionGrid.Visibility = Visibility.Collapsed;
                AuthenticationButton.Visibility = Visibility.Visible;
                MainScrollViewer.Visibility = Visibility.Collapsed;
            }
            else
            {
                AuthenticationButton.Visibility = Visibility.Collapsed;
                MainScrollViewer.Visibility = Visibility.Visible;
                InitializeData();
            }

        }


        private void InitializeData()
        {
            try
            {
                GetListCategory().Wait();
                CategoriesComboBox.ItemsSource = PetCategory;
                CategoriesComboBox.SelectedIndex = 0;
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

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            ReturnPet newPet = new ReturnPet();
            newPet.Id = -1;
            newPet.Name = NameTextBox.Text.Trim();
            newPet.Age = int.Parse(AgeTextBox.Text.Trim());
            newPet.Gender = GenderComboBox.SelectedIndex;
            newPet.Status = StatusComboBox.SelectionBoxItem.ToString();
            newPet.PetCategory = PetCategory[CategoriesComboBox.SelectedIndex];

            bool check = CheckPet(newPet);
            try
            {
                if (!check) return;
                NoConnectionGrid.Visibility = Visibility.Collapsed;
                newPet.user = CurrentUser;
                RegisterNewPet(newPet).Wait();
                Frame.Navigate(typeof(MyPetPage));
            }
            catch (Exception ex)
            {
                if (ex is AggregateException || ex is TaskCanceledException)
                {
                    NoConnectionGrid.Visibility = Visibility.Visible;
                    MainScrollViewer.Visibility = Visibility.Collapsed;
                }
            }


        }

        private bool CheckPet(ReturnPet pet)
        {
            var check = true;
            //check name
            if (pet.Name.Length == 0)
            {
                ErrorNameTextBlock.Text = "Please enter pet's name";
                ErrorNameTextBlock.Visibility = Visibility.Visible;
                check = false;
            }

            //check age
            if (pet.Age == null)
            {
                ErrorAgeTextBlock.Text = "Please enter pet's name";
                ErrorAgeTextBlock.Visibility = Visibility.Visible;
                check = false;
            }
            return check;
        }

        public async Task RegisterNewPet(ReturnPet newPet)
        {
            using (var client = new HttpClient())
            {
                var resourceLoader = ResourceLoader.GetForCurrentView();
                string serverUrl = resourceLoader.GetString("ServerURL");
                client.BaseAddress = new Uri(serverUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromMilliseconds(2000);

                HttpResponseMessage response = await client.PutAsJsonAsync("/api/Pets/add", newPet).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {

                }
            }
        }

        private void NameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            NameTextBox.SelectAll();
            ErrorNameTextBlock.Visibility = Visibility.Collapsed;
        }

        private void AgeTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AgeTextBox.SelectAll();
            ErrorAgeTextBlock.Visibility = Visibility.Collapsed;
        }

        private void AuthenticationButton_Click(object sender, RoutedEventArgs e)
        {
            _navigationFacade.NavigateToSignInPage();
        }

        private async void PhotoPickerButton_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker();
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file
                var dialog = new MessageDialog("Picked photo: " + file.Name, "Message");
                await dialog.ShowAsync();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MyPetPage));
        }
    }
}
