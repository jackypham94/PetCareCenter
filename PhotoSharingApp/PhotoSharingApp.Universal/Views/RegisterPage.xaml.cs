using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Microsoft.Practices.ServiceLocation;
using PhotoSharingApp.Universal.Commands;
using PhotoSharingApp.Universal.Facades;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            this.InitializeComponent();
            ViewModel = ServiceLocator.Current.GetInstance<RegisterViewModel>();
            DataContext = ViewModel;

            InputPane.GetForCurrentView().Showing += ItemDetailPage_Showing;
            InputPane.GetForCurrentView().Hiding += ItemDetailPage_Hiding;


        }

        /// <summary>
        /// Gets the ViewModel.
        /// </summary>
        public RegisterViewModel ViewModel { get; }

        private void ItemDetailPage_Hiding(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            layoutRoot.Margin = new Thickness(0);
            args.EnsuredFocusedElementInView = true;
        }

        private void ItemDetailPage_Showing(InputPane sender, InputPaneVisibilityEventArgs args)
        {
            layoutRoot.Margin = new Thickness(0, 0, 0, args.OccludedRect.Height);
            args.EnsuredFocusedElementInView = true;
        }


        private void RegisterScrollViewer_KeyDown(object sender, KeyRoutedEventArgs e)
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
        }

        private void UsernameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            UsernameTextBox.SelectAll();
        }

        private void PassWordPasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PassWordPasswordBox.SelectAll();
        }

        private void ConfirmPassWordPasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ConfirmPassWordPasswordBox.SelectAll();
        }

        private void EmailTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            EmailTextBox.SelectAll();
        }

        private void AddressTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AddressTextBox.SelectAll();
        }

        private void PhoneTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PhoneTextBox.SelectAll();
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            CreateNewUser newUser = new CreateNewUser();
            newUser.Name = NameTextBox.Text;
            newUser.Username = UsernameTextBox.Text;
            newUser.Password = PassWordPasswordBox.Password;
            newUser.Email = EmailTextBox.Text;
            newUser.Address = AddressTextBox.Text;
            newUser.Phone = PhoneTextBox.Text;
            newUser.Gender = GenderList.SelectedIndex;

            //request POST to api
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://192.168.11.26:62252/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // New code:
                HttpResponseMessage response = await client.PutAsJsonAsync("api/Users/", newUser);
                if (response.IsSuccessStatusCode)
                {
                    int i = 0;
                }
                else
                {
                    ErrorProviderTextBlock.Text = "Username is already existed!";
                    ErrorProviderTextBlock.Visibility = Visibility.Visible;
                }
                //ErrorProviderTextBlock.Text = "This function is developing!";
            }
        }
    }
}
