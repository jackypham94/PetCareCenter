using Windows.UI.Xaml.Controls;
using Microsoft.Practices.ServiceLocation;
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
            var viewModel = ServiceLocator.Current.GetInstance<RegisterViewModel>();
            DataContext = viewModel;
        }
    }
}
