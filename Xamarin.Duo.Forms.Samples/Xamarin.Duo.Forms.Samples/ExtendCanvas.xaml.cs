﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Duo.Forms.Samples
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExtendCanvas : DuoPage
    {
        public ExtendCanvas()
        {
            InitializeComponent();

            searchBar.SearchButtonPressed += SearchBar_SearchButtonPressed;

            webView.Source = "file:///android_asset/googlemapsearch.html";

            StartSearch();
        }

        private void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            StartSearch();
        }

        void StartSearch()
        {
            var place = searchBar?.Text ?? string.Empty;

            webView.Source = "file:///android_asset/googlemapsearch.html?place=" + System.Web.HttpUtility.UrlEncode(place);
        }
    }
}