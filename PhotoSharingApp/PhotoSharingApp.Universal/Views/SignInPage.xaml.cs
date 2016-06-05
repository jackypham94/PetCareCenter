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
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Microsoft.Practices.ServiceLocation;
using PhotoSharingApp.Universal.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using PhotoSharingApp.Universal.Facades;
using PhotoSharingApp.Universal.Models;

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// The page that allows the user to sign in.
    /// </summary>
    public sealed partial class SignInPage : Page
    {
        public SignInPage()
        {
            InitializeComponent();
            ViewModel = ServiceLocator.Current.GetInstance<SignInViewModel>();
            DataContext = ViewModel;

            //InputPane.GetForCurrentView().Showing += (s, args) =>
            //{
            //    //SignInImage.Visibility = Visibility.Collapsed;
            //    offSet = (int)args.OccludedRect.Height;
            //    args.EnsuredFocusedElementInView = true;
            //    var trans = new TranslateTransform();
            //    trans.Y = -(offSet/2);
            //    this.RenderTransform = trans;
            //};

            //InputPane.GetForCurrentView().Hiding += (s, args) =>
            //{
            //    //SignInImage.Visibility = Visibility.Visible;
            //    var trans = new TranslateTransform();
            //    trans.Y = 0;
            //    this.RenderTransform = trans;
            //    args.EnsuredFocusedElementInView = false;
            //};

            InputPane.GetForCurrentView().Showing += ItemDetailPage_Showing;
            InputPane.GetForCurrentView().Hiding += ItemDetailPage_Hiding;
        }

        void ItemDetailPage_Hiding(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            layoutRoot.Margin = new Thickness(0);
            args.EnsuredFocusedElementInView = true;
        }

        private void ItemDetailPage_Showing(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            layoutRoot.Margin = new Thickness(0, 0, 0, args.OccludedRect.Height);
            args.EnsuredFocusedElementInView = true;
        }

        /// <summary>
        /// Gets the ViewModel.
        /// </summary>
        public SignInViewModel ViewModel { get; }

        private async void LoginButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //_navigationFacade.NavigateToMainPage();
            //this.Frame.Navigate(typeof(MainPage));
            string username = UsernameTextBox.Text;
            string password = PassWordPasswordBox.Password;
            ReturnUser user = new ReturnUser();
            user.Password = password;
            user.Username = username;

            //request POST to api
            using (var client = new HttpClient())
            {
                var resourceLoader = ResourceLoader.GetForCurrentView();
                client.BaseAddress = new Uri(resourceLoader.GetString("ServerURL"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // New code:
                HttpResponseMessage response = await client.PostAsJsonAsync("api/Users/info", user);
                if (response.IsSuccessStatusCode)
                {
                    UserInfo info = await response.Content.ReadAsAsync<UserInfo>();
                    // To do: Login to home page
                    //_navigationFacade.NavigateToMainPage();
                    //_navigationFacade.RemoveBackStackFrames(1);
                    this.Frame.Navigate(typeof(MainPage));
                }
                else
                {
                    ErrorProviderTextBlock.Text = "Incorrect username or password!";
                    ErrorProviderTextBlock.Visibility = Visibility.Visible;
                }
            }


        }

        private void UsernameTextBox_GotFocus(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            UsernameTextBox.SelectAll();
            UsernamePath.Fill = new SolidColorBrush(Colors.Teal);
            ErrorProviderTextBlock.Visibility = Visibility.Collapsed;
        }

        private void UsernameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UsernamePath.Fill = new SolidColorBrush(Colors.Gray);
        }

        private void PassWordPasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PassWordPasswordBox.SelectAll();
            PasswordPath.Fill = new SolidColorBrush(Colors.Teal);
            ErrorProviderTextBlock.Visibility = Visibility.Collapsed;
        }

        private void PassWordPasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            PasswordPath.Fill = new SolidColorBrush(Colors.Gray);
        }

        private void LoginScrollViewer_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                FocusManager.TryMoveFocus(FocusNavigationDirection.Next);
                // Make sure to set the Handled to true, otherwise the RoutedEvent might fire twice
                e.Handled = true;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            bool isRegstered = e.Parameter != null && (bool)e.Parameter;
            if (isRegstered)
            {
                Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1);
                Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1);
            }
        }
    }
}