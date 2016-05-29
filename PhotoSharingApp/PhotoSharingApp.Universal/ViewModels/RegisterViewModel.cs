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
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using PhotoSharingApp.Universal.Commands;
using PhotoSharingApp.Universal.Facades;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Views;
using Windows.ApplicationModel.Resources;
using Microsoft.WindowsAzure.MobileServices;
using PhotoSharingApp.Universal.Extensions;
using PhotoSharingApp.Universal.Services;

namespace PhotoSharingApp.Universal.ViewModels
{
    /// <summary>
    /// ViewModel for Welcome screen.
    /// </summary>
    public class RegisterViewModel : ViewModelBase
    {
        private readonly INavigationFacade _navigationFacade;

        public RegisterViewModel(INavigationFacade navigationFacade)
        {
            _navigationFacade = navigationFacade;
            NavigateToTargetPageCommand = new RelayCommand(OnNavigateToTargetPage);
        }

        /// <summary>
        /// Gets the navigate to target page command.
        /// </summary>
        public RelayCommand NavigateToTargetPageCommand { get; }

        private void OnNavigateToTargetPage()
        {
            _navigationFacade.NavigateToSignInPage();
            _navigationFacade.RemoveBackStackFrames(2);
        }
    }
}