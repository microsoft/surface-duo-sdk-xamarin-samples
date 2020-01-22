using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Duo.Forms.Samples
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailsPage : DuoPage
    {
        public DetailsPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            FormsWindow.PropertyChanged += OnFormsWindowPropertyChanged;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            FormsWindow.PropertyChanged -= OnFormsWindowPropertyChanged;
        }

        async void OnFormsWindowPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(FormsWindow.IsSpanned) || e.PropertyName == nameof(FormsWindow.IsPortrait))
            {
                if (FormsWindow.IsSpanned && FormsWindow.IsPortrait)
                    await Navigation.PopAsync();
            }
        }
    }
}