﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
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
using PhotoSharingApp.Universal.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProfilePage : Page
    {
        /// <summary>
        /// The constructor
        /// </summary>
        public ProfilePage()
        {
            InitializeComponent();
            InitialData();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            CreateNewUser newUser = new CreateNewUser();
            newUser.Name = NameTextBox.Text.Trim();
            newUser.Username = UsernameTextBox.Text.Trim();
            //newUser.Password = PassWordPasswordBox.Password.Trim();
            newUser.Email = EmailTextBox.Text.Trim();
            newUser.Address = AddressTextBox.Text.Trim();
            newUser.Phone = PhoneTextBox.Text.Trim();
            newUser.Gender = GenderList.SelectedIndex;

            bool check = CheckInfo(newUser);

            if (check)
            {
                RequestPutToApi(newUser);
            }
        }

        public async void RequestPutToApi(CreateNewUser newUser)
        {
            //request POST to api
            using (var client = new HttpClient())
            {
                var resourceLoader = ResourceLoader.GetForCurrentView();
                client.BaseAddress = new Uri(resourceLoader.GetString("ServerURL"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // New code:
                try
                {
                    HttpResponseMessage response = await client.PutAsJsonAsync("api/Users/", newUser);
                    if (response.IsSuccessStatusCode)
                    {
                        //var isRegistered = true;
                        Frame.Navigate(typeof(ProfilePage));
                    }
                    else
                    {
                        ErrorProviderTextBlock.Text = "Username is already existed!";
                        ErrorProviderTextBlock.Visibility = Visibility.Visible;
                    }
                }
                catch (HttpRequestException)
                {
                    var dialog = new MessageDialog("Can not connect to server!", "Message");
                    //dialog.Commands.Add(new UICommand("Yes") { Id = 0 });
                    //dialog.Commands.Add(new UICommand("No") { Id = 1 });

                    await dialog.ShowAsync();
                }
            }
        }

        public async void RequestPostToApi(ReturnUser user)
        {
            //request POST to api
            using (var client = new HttpClient())
            {
                var resourceLoader = ResourceLoader.GetForCurrentView();
                client.BaseAddress = new Uri(resourceLoader.GetString("ServerURL"));
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // New code:

                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync("api/Users/info", user);
                    if (response.IsSuccessStatusCode)
                    {
                        UserInfo info = await response.Content.ReadAsAsync<UserInfo>();
                        NameTextBox.Text = info.Name;
                        UsernameTextBox.Text = info.Username;
                        EmailTextBox.Text = info.Email;
                        PhoneTextBox.Text = info.Phone;
                        AddressTextBox.Text = info.Address;
                        GenderList.SelectedIndex = (int)info.Gender;

                        // To do: Login to home page
                        //this.Frame.Navigate(typeof (MainPage));
                    }
                    else
                    {
                        ErrorProviderTextBlock.Text = "Please login to edit your profile";
                        ErrorProviderTextBlock.Visibility = Visibility.Visible;
                    }
                }
                catch (HttpRequestException)
                {
                    var dialog = new MessageDialog("Can not connect to server!", "Message");
                    await dialog.ShowAsync();
                }
            }
        }

        public async void InitialData()
        {
            var user = new ReturnUser();
            await DeserelizeDataFromJson("user", user);
            Base64Decode(user.Password);
            RequestPostToApi(user);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static async Task<ReturnUser> DeserelizeDataFromJson(string fileName, ReturnUser user)
        {
            try
            {
                var folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                var file = await folder.GetFileAsync(fileName + ".json");
                var data = await file.OpenReadAsync();

                using (StreamReader r = new StreamReader(data.AsStream()))
                {
                    string text = r.ReadToEnd();
                    user = JsonConvert.DeserializeObject<ReturnUser>(text);
                }
                return user;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private bool CheckInfo(CreateNewUser user)
        {
            Regex myRegex = null;
            Match m = null;
            bool check = true;

            //check name
            if (user.Name.Length == 0)
            {
                ErrorNameTextBlock.Text = "Please enter your name!";
                ErrorNameTextBlock.Visibility = Visibility.Visible;
                check = false;
            }

            //check username
            if (user.Username.Length == 0)
            {
                ErrorUsernameTextBlock.Text = "Please enter your username!";
                ErrorUsernameTextBlock.Visibility = Visibility.Visible;
                check = false;
            }

            //check password
            //if (user.Password.Length == 0)
            //{
            //    ErrorPasswordTextBlock.Text = "Please enter your password!";
            //    ErrorPasswordTextBlock.Visibility = Visibility.Visible;
            //    check = false;
            //}



            ////check password confirm
            //if (ConfirmPassWordPasswordBox.Password.Trim().Length == 0)
            //{
            //    ErrorConfirmPasswordTextBlock.Text = "Please confirm your password!";
            //    ErrorConfirmPasswordTextBlock.Visibility = Visibility.Visible;
            //    check = false;
            //}
            //else
            //{
            //    if (!ConfirmPassWordPasswordBox.Password.Trim().Equals(user.Password))
            //    {
            //        ErrorConfirmPasswordTextBlock.Text = "Password not match!";
            //        ErrorConfirmPasswordTextBlock.Visibility = Visibility.Visible;
            //        check = false;
            //    }
            //}

            //check email
            myRegex = new Regex(@"^\w+@\w+[.]\w+$");
            m = myRegex.Match(user.Email);
            if (user.Email.Length == 0)
            {
                ErrorEmailTextBlock.Text = "Please enter your email!";
                ErrorEmailTextBlock.Visibility = Visibility.Visible;
                check = false;
            }
            else
            {
                if (!m.Success)
                {
                    ErrorEmailTextBlock.Text = "Wrong email format!";
                    ErrorEmailTextBlock.Visibility = Visibility.Visible;
                    check = false;
                }
            }

            //check address
            if (user.Address.Length == 0)
            {
                ErrorAddressTextBlock.Text = "Please enter your address!";
                ErrorAddressTextBlock.Visibility = Visibility.Visible;
                check = false;
            }

            //check phone
            myRegex =
                new Regex(
                    @"\+(9[976]\d|8[987530]\d|6[987]\d|5[90]\d|42\d|3[875]\d|2[98654321]\d|9[8543210]|8[6421]|6[6543210]|5[87654321]|4[987654310]|3[9643210]|2[70]|7|1)\d{1,14}$");
            m = myRegex.Match(user.Phone);
            if (user.Phone.Length == 0)
            {
                ErrorPhoneTextBlock.Text = "Please enter your phone number!";
                ErrorPhoneTextBlock.Visibility = Visibility.Visible;
                check = false;
            }
            else
            {
                if (!m.Success)
                {
                    ErrorPhoneTextBlock.Text = "Wrong phone number format!\nPlease input phone number with area code.";
                    ErrorPhoneTextBlock.Visibility = Visibility.Visible;
                    check = false;
                }
            }

            //check gender
            //if (-1 == user.Gender)
            //{
            //    ErrorGenderTextBlock.Text = "Please choose your gender!";
            //    ErrorGenderTextBlock.Visibility = Visibility.Visible;
            //    check = false;
            //}

            return check;
        }

        private void ProfileScrollViewer_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                //if (FocusManager.GetFocusedElement() == NameTextBox) // Change the inputTextBox to your TextBox name
                //{
                //    FocusManager.TryMoveFocus(FocusNavigationDirection.Next);
                //    FocusManager.TryMoveFocus(FocusNavigationDirection.Next);
                //}
                //else
                //{
                //    FocusManager.TryMoveFocus(FocusNavigationDirection.Next);
                //}

                FocusManager.TryMoveFocus(FocusNavigationDirection.Next);
                // Make sure to set the Handled to true, otherwise the RoutedEvent might fire twice
                e.Handled = true;
            }
        }

        private void NameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            NameTextBox.SelectAll();
            ErrorNameTextBlock.Visibility = Visibility.Collapsed;
        }

        private void UsernameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            UsernameTextBox.SelectAll();
            ErrorUsernameTextBlock.Visibility = Visibility.Collapsed;
            ErrorProviderTextBlock.Visibility = Visibility.Collapsed;
        }

        //private void PassWordPasswordBox_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    PassWordPasswordBox.SelectAll();
        //    ErrorPasswordTextBlock.Visibility = Visibility.Collapsed;
        //}

        //private void ConfirmPassWordPasswordBox_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    ConfirmPassWordPasswordBox.SelectAll();
        //    ErrorConfirmPasswordTextBlock.Visibility = Visibility.Collapsed;
        //}

        private void EmailTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            EmailTextBox.SelectAll();
            ErrorEmailTextBlock.Visibility = Visibility.Collapsed;
        }

        private void AddressTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AddressTextBox.SelectAll();
            ErrorAddressTextBlock.Visibility = Visibility.Collapsed;
        }

        private void PhoneTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ErrorPhoneTextBlock.Visibility = Visibility.Collapsed;
        }
    }
}
