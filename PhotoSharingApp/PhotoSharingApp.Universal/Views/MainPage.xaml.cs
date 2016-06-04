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
        private readonly MainPageViewModel _viewModel;
        public MainPage()
        {
            this.InitializeComponent();
            _viewModel = ServiceLocator.Current.GetInstance<MainPageViewModel>();
            DataContext = _viewModel;
            InitializeItem();

        }

        private void InitializeItem()
        {
            if (_viewModel.AccessoryCombinations == null)
            {
                NoConnectionGrid.Visibility = Visibility.Visible;
            }
            else 
            {
                NoConnectionGrid.Visibility = Visibility.Collapsed;
                CategoryListView.ItemsSource = _viewModel.AccessoryCombinations;
                //Category1TextBlock.Text = _viewModel.AccessoryCombinations[0].Category.CategoryName;
                //Category1ListView.ItemsSource = _viewModel.AccessoryCombinations[0].ListOfAccessory;

                //Category2TextBlock.Text = _viewModel.AccessoryCombinations[1].Category.CategoryName;
                //Category2ListView.ItemsSource = _viewModel.AccessoryCombinations[1].ListOfAccessory;

                //Category3TextBlock.Text = _viewModel.AccessoryCombinations[2].Category.CategoryName;
                //Category3ListView.ItemsSource = _viewModel.AccessoryCombinations[2].ListOfAccessory;

            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await _viewModel.LoadState();
        }
    }
}
