
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Microsoft.Practices.ServiceLocation;
using PhotoSharingApp.Universal.Unity;
using PhotoSharingApp.Universal.ViewModels;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using PhotoSharingApp.Universal.Facades;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Serialization;
using PhotoSharingApp.Universal.Services;
using User = Windows.System.User;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CartPage : BasePage
    {
        //Authentication authentication = new Authentication();
        //private List<ReturnBuyingDetail> BuyingDetails { get; set; }
        //private static ReturnUser CurrentUser { get; set; }
        private readonly INavigationFacade _navigationFacade = new NavigationFacade();
        private int _thumbnailImageSideLength;
        private CartViewModel _viewModel;
        private static ReturnUser CurrentUser { get; set; }

        public CartPage()
        {
            this.InitializeComponent();
            UpdateThumbnailSize();
            SizeChanged += CartPage_SizeChanged;
            //CartListView.Loaded += OnCartListLoaded;
        }


        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Authentication authentication = new Authentication();
            authentication.GetCurrentUser();
            CurrentUser = authentication.CurrentUser;
            if (CurrentUser == null)
            {
                AuthenticationButton.Visibility = Visibility.Visible;
                MainScrollViewer.Visibility = Visibility.Collapsed;
                NoConnectionGrid.Visibility = Visibility.Collapsed;
                CheckoutPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                AuthenticationButton.Visibility = Visibility.Collapsed;
                MainScrollViewer.Visibility = Visibility.Visible;
                NoConnectionGrid.Visibility = Visibility.Collapsed;
                CheckoutPanel.Visibility = Visibility.Visible;
                var loadData = e.NavigationMode != NavigationMode.Back;
                _viewModel = ServiceLocator.Current.GetInstance<CartViewModel>(loadData);
                DataContext = _viewModel;

                if (loadData)
                {
                    await _viewModel.LoadState();
                    TotalTextBlock.Text = _viewModel.TotalPrice.ToString(CultureInfo.InvariantCulture);
                }
                if (!_viewModel.IsConnect)
                {
                    NoConnectionGrid.Visibility = Visibility.Visible;
                    MainScrollViewer.Visibility = Visibility.Collapsed;
                    CheckoutPanel.Visibility = Visibility.Collapsed;
                }

                bool isSignIn = e.Parameter != null && (bool)e.Parameter;
                if (isSignIn)
                {
                    Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1);
                }
            }
        }

        private void CartPage_SizeChanged(object sender, SizeChangedEventArgs e)
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
                ThumbnailImageSideLength = 150;
            }
            else
            {
                ThumbnailImageSideLength = 100;
            }
        }

        public async Task UpdateUserInfo(CreateNewUser newUser)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var resourceLoader = ResourceLoader.GetForCurrentView();
                    string serverUrl = resourceLoader.GetString("ServerURL");
                    client.BaseAddress = new Uri(serverUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.Timeout = TimeSpan.FromMilliseconds(2000);

                    await client.PutAsJsonAsync("api/Users/", newUser).ConfigureAwait(false);

                }
            }
            catch (Exception ex)
            {
                if (ex is TaskCanceledException || ex is AggregateException)
                {
                    var dialog = new MessageDialog("Can not connect to server!", "Message");
                    await dialog.ShowAsync();
                }
            }

        }

        public async Task CheckOut(CheckoutInfo checkout, UserInfo userInfo)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var resourceLoader = ResourceLoader.GetForCurrentView();
                    string serverUrl = resourceLoader.GetString("ServerURL");
                    client.BaseAddress = new Uri(serverUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.Timeout = TimeSpan.FromMilliseconds(2000);

                    HttpResponseMessage response = await client.PutAsJsonAsync("api/Checkout/", checkout).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        BuyingDetail = await response.Content.ReadAsAsync<List<ReturnBuyingDetail>>();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is TaskCanceledException || ex is AggregateException)
                {
                    var dialog = new MessageDialog("Can not connect to server!", "Message");
                    await dialog.ShowAsync();
                }
            }

        }

        private List<ReturnBuyingDetail> BuyingDetail { get; set; }

        private async void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            var data = _viewModel.Cart;
            if (!(data?.Count > 0))
            {
                var dialog = new MessageDialog("Buying something before checkout!", "Empty cart");
                await dialog.ShowAsync();
                return;
            }
            var newUser = new CreateNewUser();
            var userInfo = _viewModel.UserInfo;
            var user = _viewModel.CurrentUser;
            newUser.Name = NameTextBox.Text.Trim();
            newUser.Username = userInfo.Username;
            newUser.Password = user.Password;
            newUser.Email = userInfo.Email;
            newUser.Address = AddressTextBox.Text.Trim();
            newUser.Phone = PhoneTextBox.Text.Trim();
            newUser.Gender = userInfo.Gender;
            newUser.NewPassword = user.Password;
            UpdateUserInfo(newUser).Wait();

            var checkoutInfo = new CheckoutInfo
            {
                Username = user.Username,
                Password = user.Password,
                PlanDate = DeliveryDatePicker.Date.DateTime
            };

            CheckOut(checkoutInfo, userInfo).Wait();
            var bill = new Bill
            {
                ReturnBuyingDetail = BuyingDetail,
                PlanDate = DeliveryDatePicker.Date.DateTime,
                Total = Double.Parse(TotalTextBlock.Text.Trim()),
                UserInfo = userInfo
            };
            _navigationFacade.NavigateToBillPage(bill);
        }

        private void AuthenticationButton_Click(object sender, RoutedEventArgs e)
        {
            _navigationFacade.NavigateToSignInPage();
        }

        private void EditInfoButton_Click(object sender, RoutedEventArgs e)
        {
            var newUser = new CreateNewUser();
            var userInfo = _viewModel.UserInfo;
            var user = _viewModel.CurrentUser;
            newUser.Name = NameTextBox.Text.Trim();
            newUser.Username = userInfo.Username;
            newUser.Password = user.Password;
            newUser.Email = userInfo.Email;
            newUser.Address = AddressTextBox.Text.Trim();
            newUser.Phone = PhoneTextBox.Text.Trim();
            newUser.Gender = userInfo.Gender;
            newUser.NewPassword = user.Password;
            UpdateUserInfo(newUser).Wait();
            _navigationFacade.NavigateToProfilePage();
        }
    }
}
