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
    public sealed partial class PetInfoPage : Page
    {
        private static ReturnUser CurrentUser { get; set; }
        private ReturnPet Pet { get; set; }
        public PetInfoPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
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

                    NameTextBlock.Text = Pet.Name;
                    NoConnectionGrid.Visibility = Visibility.Collapsed;

                }
                catch (Exception ex)
                {
                    if (ex is AggregateException || ex is TaskCanceledException)
                    {
                        NoConnectionGrid.Visibility = Visibility.Visible;
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
    }
}
