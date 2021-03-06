﻿
using System;
using PhotoSharingApp.Universal.Views;
using Windows.UI.Xaml.Controls;

namespace PhotoSharingApp.Universal.NavigationBar
{
    /// <summary>
    /// Navigation bar menu item that navigates to the
    /// <see cref="WelcomePage" />.
    /// </summary>
    public class HistoryNavigationBarMenuItem : NavigationBarMenuItemBase,
        INavigationBarMenuItem
    {
        /// <summary>
        /// Gets the type of the destination page.
        /// </summary>
        public Type DestPage
        {
            get { return typeof(HistoryPage); }
        }

        /// <summary>
        /// Gets the title displayed in the navigation bar.
        /// </summary>
        public string Label
        {
            get { return "History"; }
        }

        /// <summary>
        /// Gets the symbol that is displayed in the navigation bar.
        /// </summary>
        public override Symbol Symbol
        {
            get { return Symbol.Library; }
        }
    }
}