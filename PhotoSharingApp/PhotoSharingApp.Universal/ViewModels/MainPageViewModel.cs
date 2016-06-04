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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using PhotoSharingApp.Universal.Commands;
using PhotoSharingApp.Universal.Facades;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Views;
using Windows.ApplicationModel.Resources;

namespace PhotoSharingApp.Universal.ViewModels
{
    /// <summary>
    /// ViewModel for Welcome screen.
    /// </summary>
    public class MainPageViewModel : ViewModelBase
    {
        private readonly INavigationFacade _navigationFacade;

        private InstructionItem _selectedInstructionItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="WelcomeViewModel" /> class.
        /// </summary>
        /// <param name="navigationFacade">The navigation facade.</param>
        public MainPageViewModel(INavigationFacade navigationFacade)
        {
            _navigationFacade = navigationFacade;
            NavigateToTargetPageCommand = new RelayCommand<InstructionItem>(OnNavigateToTargetPage);

            try
            {
                InitializeCategoryItems().Wait();
            }
            catch (AggregateException)
            {
                //throw;
            }
        }

        /// <summary>
        /// The instructional items.
        /// </summary>
        public List<ReturnAccessoryCombination> AccessoryCombinations { get; private set; }

        /// <summary>
        /// Gets the navigate to target page command.
        /// </summary>
        public RelayCommand<InstructionItem> NavigateToTargetPageCommand { get; }

        /// <summary>
        /// Gets or sets the current instructional item.
        /// </summary>
        //public InstructionItem SelectedInstructionItem
        //{
        //    get { return _selectedInstructionItem; }
        //    set
        //    {
        //        if (value != _selectedInstructionItem)
        //        {
        //            _selectedInstructionItem = value;
        //            NotifyPropertyChanged(nameof(SelectedInstructionItem));
        //        }
        //    }
        //}

        public async Task InitializeCategoryItems()
        {
            using (var client = new HttpClient())
            {
                var resourceLoader = ResourceLoader.GetForCurrentView();
                string serverURL = resourceLoader.GetString("ServerURL");
                client.BaseAddress = new Uri(serverURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // New code:
                HttpResponseMessage response = await client.GetAsync("api/AccessoryCategoriesDisplay").ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    AccessoryCombinations = await  response.Content.ReadAsAsync< List<ReturnAccessoryCombination>>();
                }
            }

        }

        /// <summary>
        /// Loads the state.
        /// </summary>
        //public override async Task LoadState()
        //{
        //    await base.LoadState();

        //    SelectedInstructionItem = null;
        //    SelectedInstructionItem = InstructionItems.FirstOrDefault();
        //}

        private void OnNavigateToTargetPage(InstructionItem instructionItem)
        {
            _navigationFacade.NavigateToMainPage();
        }
    }
}