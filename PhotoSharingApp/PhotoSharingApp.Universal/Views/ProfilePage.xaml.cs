using System;
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
        ReturnUser user = new ReturnUser();

        public ReturnUser User
        {
            get { return user; }
            set { user = value; }
        }

        /// <summary>
        /// The constructor
        /// </summary>
        public ProfilePage()
        {
            InitializeComponent();

            //initial data to view
            InitialData();
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CreateNewUser newUser = new CreateNewUser();
                newUser.Name = user.Username;
                newUser.Username = UsernameTextBox.Text.Trim();
                newUser.Password = user.Password;
                newUser.Email = EmailTextBox.Text.Trim();
                newUser.Address = AddressTextBox.Text.Trim();
                newUser.Phone = PhoneTextBox.Text.Trim();
                newUser.Gender = GenderList.SelectedIndex;
                if (ConfirmPassWordPasswordBox.Password.Trim().Length != 0 &&
                    NewPassWordPasswordBox.Password.Trim().Length != 0)
                {
                    newUser.NewPassword = NewPassWordPasswordBox.Password.Trim();
                }
                else
                {
                    newUser.NewPassword = user.Password;
                }

                bool check = CheckInfo(newUser);

                if (check)
                {
                    RequestPutToApi(newUser);
                }
            }
            catch (NullReferenceException)
            {
                var dialog = new MessageDialog("Please login to edit your profile!", "Message");
                await dialog.ShowAsync();
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
                        user.Password = newUser.NewPassword;
                        //encode user's new password
                        user.Password = Base64Encode(user.Password);

                        //write data to user.json file
                        await SerelizeDataToJson(user, "user");
                        Frame.Navigate(typeof(ProfilePage));
                    }
                    else
                    {
                        ErrorProviderTextBlock.Text = "Something went wrong!";
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
                        //show data to view
                        UserInfo info = await response.Content.ReadAsAsync<UserInfo>();
                        NameTextBox.Text = info.Name;
                        UsernameTextBox.Text = info.Username;
                        UsernameTextBox.IsEnabled = false;
                        EmailTextBox.Text = info.Email;
                        PhoneTextBox.Text = info.Phone;
                        AddressTextBox.Text = info.Address;
                        GenderList.SelectedIndex = (int)info.Gender;
                        //PassWordPasswordBox.Password = user.Password;
                    }
                    else
                    {
                        ErrorProviderTextBlock.Text = "Please login to edit your profile";
                        ErrorProviderTextBlock.Visibility = Visibility.Visible;
                    }
                }
                catch (Exception ex)
                {
                    if (ex is TimeoutException || ex is AggregateException)
                    {
                        var dialog = new MessageDialog("Cannot connect to server!", "Message");
                        await dialog.ShowAsync();
                    }
                    else
                    {
                        var dialog = new MessageDialog("Something happen in our end! Please try again later.", "Message");
                        await dialog.ShowAsync();
                    }
                }
            }
        }

        public async void InitialData()
        {
            user = await DeserelizeDataFromJson("user", user);  //read data from user.json file
            if (user != null)
            {
                user.Password = Base64Decode(user.Password);  //decode user's password
                RequestPostToApi(user);
            }
            else
            {
                ErrorProviderTextBlock.Text = "Please login to edit your profile";
                ErrorProviderTextBlock.Visibility = Visibility.Visible;
            }            
        }

        public static async Task<string> SerelizeDataToJson(ReturnUser user, string filename)
        {
            try
            {
                var folder = Windows.Storage.ApplicationData.Current.LocalFolder;
                var file =
                    await
                        folder.CreateFileAsync(filename + ".json",
                            Windows.Storage.CreationCollisionOption.ReplaceExisting);
                var data = await file.OpenStreamForWriteAsync();

                using (StreamWriter r = new StreamWriter(data))
                {
                    var serelizedfile = JsonConvert.SerializeObject(user);
                    r.Write(serelizedfile);

                }
                return filename;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
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

            //check password
            if (ConfirmPassWordPasswordBox.Password.Trim().Length != 0 && NewPassWordPasswordBox.Password.Trim().Length != 0 && PassWordPasswordBox.Password.Trim().Length == 0)
            {
                ErrorPasswordTextBlock.Text = "Please enter your current password!";
                ErrorPasswordTextBlock.Visibility = Visibility.Visible;
                check = false;
            }
            if (PassWordPasswordBox.Password.Trim().Length == 0)
            {
                ErrorPasswordTextBlock.Text = "Please enter your current password!";
                ErrorPasswordTextBlock.Visibility = Visibility.Visible;
                check = false;
            }
            else
            {
                if (!PassWordPasswordBox.Password.Trim().Equals(user.Password))
                {
                    ErrorPasswordTextBlock.Text = "This is not your current password!";
                    ErrorPasswordTextBlock.Visibility = Visibility.Visible;
                    check = false;
                }
            }
            

            //check new password
            if ((ConfirmPassWordPasswordBox.Password.Trim().Length != 0 && NewPassWordPasswordBox.Password.Trim().Length == 0))
            {
                ErrorNewPasswordTextBlock.Text = "Please enter your new password!";
                ErrorNewPasswordTextBlock.Visibility = Visibility.Visible;
                check = false;
            }



            //check password confirm
            if ((ConfirmPassWordPasswordBox.Password.Trim().Length == 0 && NewPassWordPasswordBox.Password.Trim().Length != 0))
            {
                ErrorConfirmPasswordTextBlock.Text = "Please confirm your new password!";
                ErrorConfirmPasswordTextBlock.Visibility = Visibility.Visible;
                check = false;
            }
            else
            {
                if (!ConfirmPassWordPasswordBox.Password.Trim().Equals(NewPassWordPasswordBox.Password.Trim()))
                {
                    ErrorConfirmPasswordTextBlock.Text = "Password not match!";
                    ErrorConfirmPasswordTextBlock.Visibility = Visibility.Visible;
                    check = false;
                }
            }

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (MainPage));
        }

        private void PassWordPasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PassWordPasswordBox.SelectAll();
            ErrorPasswordTextBlock.Visibility = Visibility.Collapsed;
        }

        private void NewPassWordPasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            NewPassWordPasswordBox.SelectAll();
            ErrorNewPasswordTextBlock.Visibility = Visibility.Collapsed;
        }

        private void ConfirmPassWordPasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ConfirmPassWordPasswordBox.SelectAll();
            ErrorConfirmPasswordTextBlock.Visibility = Visibility.Collapsed;
        }
    }
}
