﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using PhotoSharingApp.Universal.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PhotoSharingApp.Universal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Categories : Page
    {
        private ReturnAccessoryCombination Acessories { get; set; }
        public Categories()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                string text = e.Parameter.ToString();
                int id = Int32.Parse(text);
                try
                {
                    InitializeAccessoriesDetails(id).Wait();
                }
                catch (AggregateException)
                {
                }
                if (Acessories == null)
                {
                    NoConnectionGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    NoConnectionGrid.Visibility = Visibility.Collapsed;
                    CategoryListView.ItemsSource = Acessories.ListOfAccessory;
                    TitleTextBlock.Text = Acessories.Category.CategoryName.ToUpper();
                }
                
            }
        }

        public async Task InitializeAccessoriesDetails(int id)
        {
            using (var client = new HttpClient())
            {
                var resourceLoader = ResourceLoader.GetForCurrentView();
                string serverUrl = resourceLoader.GetString("ServerURL");
                client.BaseAddress = new Uri(serverUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // New code:
                String apiUrl = "/api/AccessoryCategoriesFull/" + id;
                HttpResponseMessage response = await client.GetAsync(apiUrl).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    Acessories = await response.Content.ReadAsAsync<ReturnAccessoryCombination>();
                }
            }

        }
    }
}