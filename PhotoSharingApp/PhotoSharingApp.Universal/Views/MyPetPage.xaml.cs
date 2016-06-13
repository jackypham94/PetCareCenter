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
using PhotoSharingApp.Universal.Facades;
using PhotoSharingApp.Universal.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MyPetPage : Page
    {
        private readonly INavigationFacade _navigationFacade = new NavigationFacade();
        private List<ReturnPet> ListPet { get; set; }
        private static ReturnUser User { get; set; }
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
                DeserelizeDataFromJson("user");
                if (User != null)
                {
                    InitPetList(User).Wait();
                    PetListView.ItemsSource = ListPet;
                    //CategoriesComboBox.ItemsSource = PetCategory;
                    //CategoriesComboBox.SelectedIndex = 0;
                    NoConnectionGrid.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                if (ex is TimeoutException || ex is AggregateException)
                {
                    NoConnectionGrid.Visibility = Visibility.Visible;
                }

            }
        }

        public async Task InitPetList(ReturnUser user)
        {
            using (var client = new HttpClient())
            {
                var resourceLoader = ResourceLoader.GetForCurrentView();
                string serverUrl = resourceLoader.GetString("ServerURL");
                client.BaseAddress = new Uri(serverUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromMilliseconds(2000);

                HttpResponseMessage response = await client.PostAsJsonAsync("/api/Pets", user).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    ListPet = await response.Content.ReadAsAsync<List<ReturnPet>>();
                }
            }
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
        private void AddNewPetButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddPetPage));
        }

        private void PetListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _navigationFacade.NavigateToPetDetail(ListPet[PetListView.SelectedIndex]);
        }
    }
}
