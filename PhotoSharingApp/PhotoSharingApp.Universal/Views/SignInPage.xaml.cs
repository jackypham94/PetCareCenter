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

using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Microsoft.Practices.ServiceLocation;
using PhotoSharingApp.Universal.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

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
            //InputPane.GetForCurrentView().Showing += Keyboard_OnHide;
            //InputPane.GetForCurrentView().Hiding += Keyboard_OnShow;
            var _offSet = 0;

            Windows.UI.ViewManagement.InputPane.GetForCurrentView().Showing += (s, args) =>
            {
                //SignInImage.Visibility = Visibility.Collapsed;
                _offSet = (int)args.OccludedRect.Height;
                args.EnsuredFocusedElementInView = true;
                var trans = new TranslateTransform();
                trans.Y = -(_offSet/2);
                this.RenderTransform = trans;
            };

            Windows.UI.ViewManagement.InputPane.GetForCurrentView().Hiding += (s, args) =>
            {
                //SignInImage.Visibility = Visibility.Visible;
                var trans = new TranslateTransform();
                trans.Y = 0;
                this.RenderTransform = trans;
                args.EnsuredFocusedElementInView = false;
            };
            ViewModel = ServiceLocator.Current.GetInstance<SignInViewModel>();
            DataContext = ViewModel;
        }

        /// <summary>
        /// Gets the ViewModel.
        /// </summary>
        public SignInViewModel ViewModel { get; }

        private void LoginButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //this.Frame.Navigate(typeof(MainPage));
            ErrorProviderTextBlock.Text = "Incorrect username or password!";
            ErrorProviderTextBlock.Visibility = Visibility.Visible;

        }
        //private void Keyboard_OnShow(InputPane sender, InputPaneVisibilityEventArgs args)
        //{
        //    this.LoginScrollViewer.Height = this.ActualHeight - args.OccludedRect.Height - 50;
        //}

        //private void Keyboard_OnHide(InputPane sender, InputPaneVisibilityEventArgs args)
        //{
        //    this.LoginScrollViewer.Height = this.ActualHeight;
        //}
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
    }
}