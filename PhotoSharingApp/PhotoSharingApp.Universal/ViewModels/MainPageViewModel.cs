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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using PhotoSharingApp.Universal.Commands;
using PhotoSharingApp.Universal.ComponentModel;
using PhotoSharingApp.Universal.Facades;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Services;
using PhotoSharingApp.Universal.Views;
using Windows.UI.Xaml;

namespace PhotoSharingApp.Universal.ViewModels
{
    /// <summary>
    /// The ViewModel for the categories view.
    /// </summary>
    public class MainPageViewModel : ViewModelBase
    {
        /// <summary>
        /// The number of hero images to show
        /// </summary>
        public bool IsConnect { get; set; }

        /// <summary>
        /// The auth enforcement handler.
        /// </summary>
        private readonly IAuthEnforcementHandler _authEnforcementHandler;

        private readonly IDialogService _dialogService;

        /// <summary>
        /// The hero iamges
        /// </summary>
        //private ObservableCollection<Photo> _heroImages;

        /// <summary>
        /// The timer for scrolling though hero images
        /// </summary>
        //private DispatcherTimer _heroImageScrollTimer;

        /// <summary>
        /// True, if ViewModel is busy
        /// </summary>
        private bool _isBusy;

        /// <summary>
        /// The visibility status of the empty data message.
        /// </summary>
        private bool _isEmptyDataMessageVisible;

        /// <summary>
        /// The visibility status of the status container.
        /// </summary>
        private bool _isStatusContainerVisible;

        /// <summary>
        /// True, if user is signed in. Otherwise, false.
        /// </summary>
        private bool _isUserSignedIn;

        /// <summary>
        /// The navigation facade
        /// </summary>
        private readonly INavigationFacade _navigationFacade;

        /// <summary>
        /// The photo service
        /// </summary>
        private readonly IPetCareService _petCareService;

        /// <summary>
        /// The current hero image
        /// </summary>
        private Photo _selectedHeroImage;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPageViewModel" /> class.
        /// </summary>
        /// <param name="navigationFacade">The navigation facade.</param>
        /// <param name="petCareService">The photo service.</param>
        /// <param name="authEnforcementHandler">The auth enforcement handler.</param>
        /// <param name="dialogService">The dialog service</param>
        public MainPageViewModel(INavigationFacade navigationFacade, IPetCareService petCareService,
            IAuthEnforcementHandler authEnforcementHandler, IDialogService dialogService)
        {
            _navigationFacade = navigationFacade;
            _petCareService = petCareService;
            _authEnforcementHandler = authEnforcementHandler;
            _dialogService = dialogService;

            // Initialize collections.
            TopImages = new ObservableCollection<ReturnAccessory>();

            //try
            //{
            //    InitializeCategoryItems().Wait();
            //}
            //catch (AggregateException)
            //{
            //    //throw;
            //}

            // Initialize commands
            ShowAllCommand = new RelayCommand<ReturnAccessoryCombination>(OnShowAllSelected);
            //PhotoThumbnailSelectedCommand = new RelayCommand<PhotoThumbnail>(OnPhotoThumbnailSelected);
            PhotoThumbnailSelectedCommand = new RelayCommand<ReturnAccessory>(OnPhotoThumbnailSelected);
            //HeroImageSelectedCommand = new RelayCommand<Photo>(OnHeroImageSelected);
            GiveGoldCommand = new RelayCommand<Photo>(OnGiveGold);
            UserSelectedCommand = new RelayCommand<User>(OnUserSelected);

            IsUserSignedIn = AppEnvironment.Instance.CurrentUser != null;
        }

        /// <summary>
        /// Gets the category selected command.
        /// </summary>
        public RelayCommand<ReturnAccessoryCombination> ShowAllCommand { get; private set; }

        /// <summary>
        /// Gets or sets the favorite categories.
        /// </summary>
        /// <value>
        /// The favorite categories.
        /// </value>
        public IncrementalLoadingCollection<CategoryPreview> FavoriteCategories { get; set; }

        /// <summary>
        /// Gets give gold command.
        /// </summary>
        public RelayCommand<Photo> GiveGoldCommand { get; private set; }

        /// <summary>
        /// Gets or sets the hero images.
        /// </summary>
        public ObservableCollection<ReturnAccessory> TopImages { get; set; }

        /// <summary>
        /// Gets the hero image selected command.
        /// </summary>
        //public RelayCommand<Photo> HeroImageSelectedCommand { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is busy.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is busy; otherwise, <c>false</c>.
        /// </value>
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (value != _isBusy)
                {
                    _isBusy = value;
                    NotifyPropertyChanged(nameof(IsBusy));
                }
            }
        }

        /// <summary>
        /// Gets or sets the visibility of the status message that no
        /// data is available.
        /// </summary>
        public bool IsEmptyDataMessageVisible
        {
            get { return _isEmptyDataMessageVisible; }
            set
            {
                if (value != _isEmptyDataMessageVisible)
                {
                    _isEmptyDataMessageVisible = value;
                    NotifyPropertyChanged(nameof(IsEmptyDataMessageVisible));
                }
            }
        }

        /// <summary>
        /// Gets or sets the visibility of the status container.
        /// </summary>
        public bool IsStatusContainerVisible
        {
            get { return _isStatusContainerVisible; }
            set
            {
                if (value != _isStatusContainerVisible)
                {
                    _isStatusContainerVisible = value;
                    NotifyPropertyChanged(nameof(IsStatusContainerVisible));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indication whether the user is signed in.
        /// </summary>
        public bool IsUserSignedIn
        {
            get { return _isUserSignedIn; }
            set
            {
                if (value != _isUserSignedIn)
                {
                    _isUserSignedIn = value;
                    NotifyPropertyChanged(nameof(IsUserSignedIn));
                }
            }
        }

        /// <summary>
        /// Gets or sets the new categories.
        /// </summary>
        /// <value>
        /// The new categories.
        /// </value>
        public IncrementalLoadingCollection<CategoryPreview> NewCategories { get; set; }

        public RelayCommand<ReturnAccessory> PhotoThumbnailSelectedCommand { get; private set; }

        /// <summary>
        /// Gets or sets the selected category.
        /// </summary>
        //public CategoryPreview SelectedCategoryPreview { get; set; }

        public ReturnAccessoryCombination SelectedAccessoryCategory { get; set; }

        /// <summary>
        /// Gets or sets the hero image.
        /// </summary>
        public Photo SelectedHeroImage
        {
            get { return _selectedHeroImage; }
            set
            {
                if (value != _selectedHeroImage)
                {
                    _selectedHeroImage = value;
                    NotifyPropertyChanged(nameof(SelectedHeroImage));

                    // Reset the flip timer to keep it consistent if a
                    // user changes the photo in between automatic flips.
                    //if (_heroImageScrollTimer != null)
                    //{
                    //    _heroImageScrollTimer.Stop();
                    //    _heroImageScrollTimer.Start();
                    //}
                }
            }
        }

        /// <summary>
        /// Gets or sets the top categories.
        /// </summary>
        /// <value>
        /// The top categories.
        /// </value>
        public ObservableCollection<ReturnAccessoryCombination> TopCategories { get; set; } =
            new ObservableCollection<ReturnAccessoryCombination>();

        /// <summary>
        /// Gets the user selected command.
        /// </summary>
        public RelayCommand<User> UserSelectedCommand { get; private set; }

        /// <summary>
        /// Loads the state.
        /// </summary>
        public override async Task LoadState()
        {
            await base.LoadState();

            IsBusy = true;
            IsStatusContainerVisible = true;

            try
            {
                TopCategories.Clear();

                // Load hero images
                //var heroImages = await _petCareService.GetHeroImages(NumberOfHeroImages);
                TopImages.Clear();
                //heroImages.ForEach(h => HeroImages.Add(h));


                // Load categories

                InitializeCategoryItems().Wait();

                if (AccessoryCombinations != null)
                {
                    if (AccessoryCombinations.Count > 0)
                    {
                        var categories = AccessoryCombinations;

                        IsEmptyDataMessageVisible = !categories.Any();
                        IsStatusContainerVisible = !categories.Any();

                        try
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                TopImages.Add(categories[0].ListOfAccessory[i]);
                            }
                        }
                        catch (Exception)
                        {
                            // ignored
                        }

                        foreach (var c in categories)
                        {
                            TopCategories.Add(c);

                            // For UI animation purposes, we wait a little until the next
                            // element is inserted.
                            await Task.Delay(200);
                        }
                    }
                }

            }
            catch (ServiceException)
            {
                await _dialogService.ShowGenericServiceErrorNotification();
            }
            finally
            {
                IsBusy = false;
            }
        }

        public List<ReturnAccessoryCombination> AccessoryCombinations { get; private set; }

        public async Task InitializeCategoryItems()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var resourceLoader = ResourceLoader.GetForCurrentView();
                    string serverUrl = resourceLoader.GetString("ServerURL");
                    client.BaseAddress = new Uri(serverUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.Timeout = TimeSpan.FromMilliseconds(2000);

                    HttpResponseMessage response = await client.GetAsync("/api/AccessoryCategoriesDisplay").ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        AccessoryCombinations = await response.Content.ReadAsAsync<List<ReturnAccessoryCombination>>();
                        IsConnect = true;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is TimeoutException || ex is AggregateException)
                {
                    IsConnect = false;
                    //AccessoryCombinations = new List<ReturnAccessoryCombination>();
                }
            }
        }

        private void OnShowAllSelected(ReturnAccessoryCombination accessoryCombination)
        {
            SelectedAccessoryCategory = accessoryCombination;
            _navigationFacade.NavigateToCategoryPage(accessoryCombination);
            //SelectedCategoryPreview = categoryPreview;
            //_navigationFacade.NavigateToPhotoStream(categoryPreview);
        }

        private async void OnGiveGold(Photo photo)
        {
            try
            {
                await _authEnforcementHandler.CheckUserAuthentication();
                await _navigationFacade.ShowGiveGoldDialog(photo);
            }
            catch (SignInRequiredException)
            {
                // Swallow exception. User canceled the Sign-in dialog.
            }
            catch (ServiceException)
            {
                await _dialogService.ShowGenericServiceErrorNotification();
            }
        }

        private void OnHeroImageSelected(Photo photo)
        {
            _navigationFacade.NavigateToPhotoDetailsView(photo);
        }

        private void OnPhotoThumbnailSelected(ReturnAccessory accessory)
        {
            var categoryPreview = TopCategories.SingleOrDefault(c => c.ListOfAccessory.Contains(accessory));

            if (categoryPreview != null)
            {
                //_navigationFacade.NavigateToPhotoStream(categoryPreview, accessory);
                _navigationFacade.NavigateToAccessoryDetail(categoryPreview, accessory);
                //_navigationFacade.NavigateToRegisterPage();
            }
        }

        private void OnUserSelected(User user)
        {
            _navigationFacade.NavigateToProfileView(user);
        }

    }
}