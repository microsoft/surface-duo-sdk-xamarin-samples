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
    public partial class MasterDetail : DuoPage
    {
        DetailsPage detailsPagePushed;

        public MasterDetail()
        {
            InitializeComponent();
            FormsWindow.PropertyChanged += OnFormsWindowPropertyChanged;
            masterPage.SelectionChanged += OnTitleSelected;
            detailsPagePushed = new DetailsPage();
        }

        private void OnTitleSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection == null || e.CurrentSelection.Count == 0)
                return;

            SetBindingContext();
            SetupViews();            
        }


        void SetBindingContext()
        {
            var bindingContext = masterPage.SelectedItem ?? (masterPage.ItemsSource as IList<MasterDetailsItem>)[0];
            detailsPagePushed.BindingContext = bindingContext;
            detailsPage.BindingContext = bindingContext;
        }

        async void SetupViews()
        {
            if (FormsWindow.IsSpanned && FormsWindow.IsPortrait)
                SetBindingContext();

            if (detailsPage.BindingContext == null)
                return;

            if (!FormsWindow.IsSpanned || FormsWindow.IsLandscape)
            {
                if (!Navigation.NavigationStack.Contains(detailsPagePushed))
                {
                    await Navigation.PushAsync(detailsPagePushed);
                }
            }

        }

        protected override void OnAppearing()
        {
            if (!FormsWindow.IsSpanned)
                masterPage.SelectedItem = null;
        }

        void OnFormsWindowPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(FormsWindow.IsSpanned) || e.PropertyName == nameof(FormsWindow.IsPortrait))
            {
                SetupViews();
            }
        }
    }
}