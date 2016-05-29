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
using Microsoft.WindowsAzure.MobileServices;
using PhotoSharingApp.Universal.Commands;
using PhotoSharingApp.Universal.Facades;
using PhotoSharingApp.Universal.Services;
using PhotoSharingApp.Universal.Views;
using Windows.ApplicationModel.Resources;
using PhotoSharingApp.Universal.Models;

namespace PhotoSharingApp.Universal.ViewModels
{
    /// <summary>
    /// The ViewModel for the sign-in view.
    /// </summary>
    public class SignInViewModel : ViewModelBase
    {
        private readonly INavigationFacade _navigationFacade;

        public SignInViewModel(INavigationFacade navigationFacade)
        {
            _navigationFacade = navigationFacade;

            // Initialize commands
            //ChooseAuthProviderCommand = new RelayCommand<MobileServiceAuthenticationProvider>(OnChooseAuthProvider);

            NavigateToTargetPageCommand = new RelayCommand(OnNavigateToTargetPage);
            // Initialize auth providers
            //AuthenticationProviders = photoService.GetAvailableAuthenticationProviders();
        }

        /// <summary>
        /// Gets the navigate to target page command.
        /// </summary>
        public RelayCommand NavigateToTargetPageCommand { get; }

        /// <summary>
        /// Gets or sets the authentication providers.
        /// </summary>
        /// <value>
        /// The authentication providers.
        /// </value>
        public List<MobileServiceAuthenticationProvider> AuthenticationProviders { get; set; }

        /// <summary>
        /// Gets the authentication reassurance message.
        /// </summary>
        public string AuthenticationReassuranceMessage { get; } = ResourceLoader.GetForCurrentView().GetString("SignInPage_ReassuranceMessage");

        /// <summary>
        /// Gets the choose authentication provider command.
        /// </summary>
        /// <value>
        /// The choose authentication provider command.
        /// </value>
        //public RelayCommand<MobileServiceAuthenticationProvider> ChooseAuthProviderCommand { get; private set; }

        /// <summary>
        /// Enables or disables the trigger to redirect
        /// to the profile page after a successful sign-in.
        /// 
        /// The default use case is that users will directly navigate to the
        // sign-in page which should redirect to the profile page.
        // Alternatively, users can sign-in using the sign-in
        // dialog, which should not do any redirections.
        /// </summary>
        public bool RedirectToProfilePage { get; set; } = true;

        private void OnNavigateToTargetPage()
        {
            _navigationFacade.NavigateToRegisterPage();
            //_navigationFacade.RemoveBackStackFrames(1);
        }
    }
}