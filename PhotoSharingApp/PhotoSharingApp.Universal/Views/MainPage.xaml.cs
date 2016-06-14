﻿//-----------------------------------------------------------------------------------
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

using Microsoft.Practices.ServiceLocation;
using PhotoSharingApp.Universal.Unity;
using PhotoSharingApp.Universal.ViewModels;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// The categories page that displays the most recent categories
    /// with photo thumbnails.
    /// </summary>
    public sealed partial class MainPage : BasePage
    {
        private int _thumbnailImageSideLength;
        private MainPageViewModel _viewModel;

        /// <summary>
        /// The constructor.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            UpdateThumbnailSize();

            SizeChanged += MainPage_SizeChanged;

            // We want to scroll to the previously selected item,
            // so we need to watch the Loaded event as this is the
            // earliest time we can change the scroll position.
            CategoriesList.Loaded += OnCategoriesListLoaded;
        }

        /// <summary>
        /// The side length of a thumbnail
        /// </summary>
        public int ThumbnailImageSideLength
        {
            get { return _thumbnailImageSideLength; }
            set
            {
                if (value != _thumbnailImageSideLength)
                {
                    _thumbnailImageSideLength = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateThumbnailSize();
        }

        private void OnCategoriesListLoaded(object sender, RoutedEventArgs e)
        {
            // If there is a category selected already, we want the view to update
            // the scroll position to show it.
            if (_viewModel.SelectedAccessoryCategory != null)
            {
                // MainScrollViewer control holds both the hero images and categories,
                // so we need to find the offset of the selected category in 
                // the parent ScrollViewer control.
                var container =
                    CategoriesList.ContainerFromItem(_viewModel.SelectedAccessoryCategory) as FrameworkElement;

                if (container != null)
                {
                    var focusedVisualTransform = container.TransformToVisual(MainScrollViewer);
                    var transformPoint = focusedVisualTransform.TransformPoint(new Point(0, 0));

                    // Now adjust the scroll position.
                    MainScrollViewer.ChangeView(null, transformPoint.Y, null, true);
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            //_viewModel.StopHeroImageSlideShow();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var loadData = e.NavigationMode != NavigationMode.Back;
            _viewModel = ServiceLocator.Current.GetInstance<MainPageViewModel>(loadData);
            DataContext = _viewModel;
            NoConnectionGrid.Visibility = Visibility.Collapsed;
            MainScrollViewer.Visibility = Visibility.Visible;

            if (loadData)
            {
                await _viewModel.LoadState();
            }
            if (!_viewModel.IsConnect)
            {
                NoConnectionGrid.Visibility = Visibility.Visible;
                MainScrollViewer.Visibility = Visibility.Collapsed;
            }

            bool isSignIn = e.Parameter != null && (bool)e.Parameter;
            if (isSignIn)
            {
                Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1);
            }

            //_viewModel.StartHeroImageSlideShow();
        }

        private void UpdateThumbnailSize()
        {
            if (PageRoot.ActualWidth > 1300)
            {
                ThumbnailImageSideLength = 150;
            }
            else
            {
                ThumbnailImageSideLength = 100;
            }
        }

        private void AddNewPetButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddPetPage));
        }
    }
}