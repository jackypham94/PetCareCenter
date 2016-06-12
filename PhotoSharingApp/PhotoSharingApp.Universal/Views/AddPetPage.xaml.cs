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
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using PhotoSharingApp.Universal.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddPetPage : Page
    {
        private List<ReturnPetCategory> PetCategory { get; set; }
        private static ReturnUser User { get; set; }
        public AddPetPage()
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
                CategoriesComboBox.ItemsSource = PetCategory;
                CategoriesComboBox.SelectedIndex = 0;
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

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            ReturnPet newPet = new ReturnPet();
            newPet.Name = NameTextBox.Text.Trim();
            newPet.Age = int.Parse(AgeTextBox.Text.Trim());
            newPet.Gender = GenderComboBox.SelectedIndex;
            if (StatusComboBox.SelectedItem != null) newPet.Status = StatusComboBox.SelectedItem.ToString();
            newPet.PetCategory = PetCategory[CategoriesComboBox.SelectedIndex];

            bool check = checkPet(newPet);
            try
            {
                if (check)
                {
                    DeserelizeDataFromJson("user");
                    if (User != null)
                    {
                        NoConnectionGrid.Visibility = Visibility.Collapsed;
                        newPet.user = User;
                        RequestToApi(newPet).Wait();
                    }
                    else
                    {

                    }
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

        private bool checkPet(ReturnPet pet)
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

        public async Task RequestToApi(ReturnPet newPet)
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
                    Frame.Navigate(typeof (MyPetPage));
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
    }
}
