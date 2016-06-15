//-----------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// 
//  The MIT License (MIT)
// 
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//  ---------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using PhotoSharingApp.Universal.Commands;
using PhotoSharingApp.Universal.Facades;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Views;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using PhotoSharingApp.Universal.ComponentModel;
using PhotoSharingApp.Universal.Services;

namespace PhotoSharingApp.Universal.ViewModels
{
    /// <summary>
    /// ViewModel for Welcome screen.
    /// </summary>
    public class CartViewModel : ViewModelBase
    {
        public bool IsConnect { get; set; }
        private readonly INavigationFacade _navigationFacade;
        private readonly IDialogService _dialogService;

        public CartViewModel(INavigationFacade navigationFacade, IDialogService dialogService)
        {
            _navigationFacade = navigationFacade;
            _dialogService = dialogService;

            _authentication.GetCurrentUser();
            CurrentUser = _authentication.CurrentUser;
            InitUserInfo(CurrentUser).Wait();

            // Initialize commands
            DeleteCommand = new RelayCommand<ReturnBuyingDetail>(OnDeleteSelected);
            PhotoThumbnailSelectedCommand = new RelayCommand<ReturnAccessory>(OnPhotoThumbnailSelected);

        }

        public RelayCommand<ReturnBuyingDetail> DeleteCommand { get; private set; }

        public RelayCommand<ReturnAccessory> PhotoThumbnailSelectedCommand { get; private set; }

        public ReturnBuyingDetail SelectedBuyingDetail { get; set; }

        public ObservableCollection<ReturnBuyingDetail> Cart { get; set; } =
            new ObservableCollection<ReturnBuyingDetail>();

        /// <summary>
        /// Gets the user selected command.
        /// </summary>
        public RelayCommand<User> UserSelectedCommand { get; private set; }

        readonly Authentication _authentication = new Authentication();

        public ReturnUser CurrentUser { get; set; }

        public List<ReturnBuyingDetail> CartItem { get; private set; }

        public UserInfo UserInfo { get; private set; }

        public double TotalPrice { get; private set; }

        /// <summary>
        /// Loads the state.
        /// </summary>
        public override async Task LoadState()
        {
            await base.LoadState();
            try
            {
                Cart.Clear();

                // Load categories
                
                InitCartDetail(CurrentUser).Wait();

                if (CartItem?.Count > 0)
                {
                    var cartItem = CartItem;

                    foreach (var c in cartItem)
                    {
                        Cart.Add(c);

                        // For UI animation purposes, we wait a little until the next
                        // element is inserted.
                        await Task.Delay(200);
                    }
                }
                CountTotalPrice();

            }
            catch (ServiceException)
            {

            }
        }

        public void CountTotalPrice()
        {
            TotalPrice = 0;
            foreach (var c in Cart)
            {
                TotalPrice += (c.Accessory.Price * c.BuyingQuantity);
            }
        }

        public async Task InitCartDetail(ReturnUser user)
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

                    HttpResponseMessage response = await client.PostAsJsonAsync("/api/CartDetail", user).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        CartItem = await response.Content.ReadAsAsync<List<ReturnBuyingDetail>>();
                        IsConnect = true;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is TaskCanceledException || ex is AggregateException)
                {
                    IsConnect = false;
                }
            }

        }

        public async Task InitUserInfo(ReturnUser user)
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

                    HttpResponseMessage response = await client.PostAsJsonAsync("api/Users/info", user).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        UserInfo = await response.Content.ReadAsAsync<UserInfo>();
                        IsConnect = true;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is TaskCanceledException || ex is AggregateException)
                {
                    IsConnect = false;
                }
            }

        }

        private async void OnDeleteSelected(ReturnBuyingDetail buyingDetail)
        {
            var confirmationResult = await
                    _dialogService.ShowYesNoNotification("DeleteAccessory_Message",
                        "DeleteAccessory_Title");
            if (confirmationResult)
            {
                SelectedBuyingDetail = buyingDetail;
                DeleteCartItem(CurrentUser, SelectedBuyingDetail.Accessory, 0);
                _navigationFacade.NavigateToCartPage();
            }
        }

        private async void DeleteCartItem(ReturnUser user, ReturnAccessory accessory, int quantity)
        {
            //request POST to api
            using (var client = new HttpClient())
            {
                var resourceLoader = ResourceLoader.GetForCurrentView();
                client.BaseAddress = new Uri(resourceLoader.GetString("ServerURL"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromMilliseconds(2000);

                AddToCart cart = new AddToCart()
                {
                    Username = user.Username,
                    Password = user.Password,
                    AccessoryId = accessory.Id,
                    Quantity = quantity
                };

                HttpResponseMessage response = await client.PutAsJsonAsync("api/UserBuyingDetail/Edit", cart);
                if (response.IsSuccessStatusCode)
                {
                    //Ignore
                }

            }
        }


        private void OnPhotoThumbnailSelected(ReturnAccessory accessory)
        {
            //var categoryPreview = Cả.SingleOrDefault(c => c.ListOfAccessory.Contains(accessory));

            //if (categoryPreview != null)
            //{
            //    //_navigationFacade.NavigateToPhotoStream(categoryPreview, accessory);
            //    _navigationFacade.NavigateToAccessoryDetail(categoryPreview, accessory);
            //    //_navigationFacade.NavigateToRegisterPage();
            //}
        }

        private void OnUserSelected(User user)
        {
            _navigationFacade.NavigateToProfileView(user);
        }

    }
}