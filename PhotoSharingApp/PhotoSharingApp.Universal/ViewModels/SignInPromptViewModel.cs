
using PhotoSharingApp.Universal.Commands;
using PhotoSharingApp.Universal.Facades;

namespace PhotoSharingApp.Universal.ViewModels
{
    /// <summary>
    /// The ViewModel for the sign-in prompt view.
    /// </summary>
    public class SignInPromptViewModel : ViewModelBase
    {
        private readonly INavigationFacade _navigationFacade;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="navigationFacade">The navigation facade.</param>
        public SignInPromptViewModel(INavigationFacade navigationFacade)
        {
            _navigationFacade = navigationFacade;
            SignInCommand = new RelayCommand(OnSignIn);
        }

        /// <summary>
        /// Gets the sign-in command.
        /// </summary>
        public RelayCommand SignInCommand { get; }

        private void OnSignIn()
        {
            _navigationFacade.NavigateToSignInView();
        }
    }
}