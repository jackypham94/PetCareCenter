using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Practices.ServiceLocation;
using PhotoSharingApp.Universal.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainPageViewModel _viewModel;
        public MainPage()
        {
            this.InitializeComponent();
            InitializeItem();
        }

        private void InitializeItem()
        {
            _viewModel = ServiceLocator.Current.GetInstance<MainPageViewModel>();
            DataContext = _viewModel;
            if (_viewModel.AccessoryCombinations == null)
            {
                NoConnectionGrid.Visibility = Visibility.Visible;
            }
            else 
            {
                NoConnectionGrid.Visibility = Visibility.Collapsed;
                CategoryListView.ItemsSource = _viewModel.AccessoryCombinations;
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await _viewModel.LoadState();
        }
    }
}
