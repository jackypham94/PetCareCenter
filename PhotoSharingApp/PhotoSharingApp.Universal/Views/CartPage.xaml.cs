
using Microsoft.Practices.ServiceLocation;
using PhotoSharingApp.Universal.Unity;
using PhotoSharingApp.Universal.ViewModels;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

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

        private int _thumbnailImageSideLength;
        private CartViewModel _viewModel;
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

            var loadData = e.NavigationMode != NavigationMode.Back;
            _viewModel = ServiceLocator.Current.GetInstance<CartViewModel>(loadData);
            DataContext = _viewModel;

            NoConnectionGrid.Visibility = Visibility.Collapsed;
            ScrollGrid.Visibility = Visibility.Visible;
            if (loadData)
            {
                await _viewModel.LoadState();
            }
            if (!_viewModel.IsConnect)
            {
                NoConnectionGrid.Visibility = Visibility.Visible;
                ScrollGrid.Visibility = Visibility.Collapsed;
            }

            bool isSignIn = e.Parameter != null && (bool)e.Parameter;
            if (isSignIn)
            {
                Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1);
            }

            //_viewModel.StartHeroImageSlideShow();
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

        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {

        }

        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    base.OnNavigatedTo(e);
        //    authentication.GetCurrentUser();
        //    CurrentUser = authentication.CurrentUser;
        //    try
        //    {
        //        if (CurrentUser != null)
        //        {
        //            InitCartDetail(CurrentUser).Wait();
        //            CartListView.ItemsSource = BuyingDetails;
        //            NoConnectionGrid.Visibility = Visibility.Collapsed;
        //        }
        //        else
        //        {
        //            // Navigate to Login page
        //            ErrorTextBlock.Text = "You must login first!";
        //            NoConnectionGrid.Visibility = Visibility.Visible;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex is AggregateException || ex is TimeoutException)
        //        {
        //            NoConnectionGrid.Visibility = Visibility.Visible;
        //        }
        //    }
        //}



        //private async void DeleteCartItemButton_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        DeleteCartItem(CurrentUser, BuyingDetails[CartListView.SelectedIndex].Accessory, 0);
        //        BuyingDetails.RemoveAt(CartListView.SelectedIndex);
        //    }
        //    catch (HttpRequestException)
        //    {
        //        var dialog = new MessageDialog("Can not connect to server!", "Message");
        //        await dialog.ShowAsync();
        //    }
        //}

        //private async void DeleteCartItem(ReturnUser user, ReturnAccessory accessory, int quantity)
        //{
        //    //request POST to api
        //    using (var client = new HttpClient())
        //    {
        //        var resourceLoader = ResourceLoader.GetForCurrentView();
        //        client.BaseAddress = new Uri(resourceLoader.GetString("ServerURL"));
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        client.Timeout = TimeSpan.FromMilliseconds(2000);

        //        AddToCart cart = new AddToCart()
        //        {
        //            Username = user.Username,
        //            Password = user.Password,
        //            AccessoryId = accessory.Id,
        //            Quantity = quantity
        //        };

        //        HttpResponseMessage response = await client.PutAsJsonAsync("api/UserBuyingDetail/Edit", cart);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var dialog = new MessageDialog(
        //            "You have been deleted ID: " + accessory.Id + " Name: " + accessory.Name,
        //            "Congratulation");
        //            await dialog.ShowAsync();
        //        }

        //    }
        //}
    }
}
