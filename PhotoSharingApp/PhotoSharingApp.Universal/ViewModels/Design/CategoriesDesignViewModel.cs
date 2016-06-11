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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PhotoSharingApp.Universal.Models;
using PhotoSharingApp.Universal.Services;

namespace PhotoSharingApp.Universal.ViewModels.Design
{
    /// <summary>
    /// The design-time ViewModel for the categories view.
    /// </summary>
    public class CategoriesDesignViewModel
    {
        public CategoriesDesignViewModel()
        {
            //var photoDummyService = new PetCareDummyService();
            //HeroImages = new ObservableCollection<Photo>(photoDummyService.PhotoStreams.First().Photos.Take(5));
            //SelectedHeroImage = HeroImages.FirstOrDefault();
            //TopCategories = new List<CategoryPreview>(photoDummyService.TopCategories);
        }

        public ObservableCollection<Photo> HeroImages { get; set; }

        public Photo SelectedHeroImage { get; set; }

        public IList<CategoryPreview> TopCategories { get; set; }
    }
}