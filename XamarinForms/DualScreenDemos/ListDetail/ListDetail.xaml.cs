using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.DualScreen;
using Xamarin.Forms.Xaml;

namespace DualScreenDemos
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListDetail
    {
        bool IsSpanned => DualScreenInfo.Current.SpanMode != TwoPaneViewMode.SinglePane;
        DetailsPage detailsPagePushed;

        public ListDetail()
        {
            InitializeComponent();
            listPage.SelectionChanged += OnTitleSelected;
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
            var bindingContext = listPage.SelectedItem ?? (listPage.ItemsSource as IList<ListDetailsItem>)[0];
            detailsPagePushed.BindingContext = bindingContext;
            detailsPage.BindingContext = bindingContext;
        }

        async void SetupViews()
        {
            if (IsSpanned && !DualScreenInfo.Current.IsLandscape)
                SetBindingContext();

            if (detailsPage.BindingContext == null)
                return;

            if (!IsSpanned)
            {
                if (!Navigation.NavigationStack.Contains(detailsPagePushed))
                {
                    await Navigation.PushAsync(detailsPagePushed);
                }
            }

        }

        protected override void OnAppearing()
        {
            if (!IsSpanned)
                listPage.SelectedItem = null;
            DualScreenInfo.Current.PropertyChanged += OnFormsWindowPropertyChanged;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            DualScreenInfo.Current.PropertyChanged -= OnFormsWindowPropertyChanged;
        }

        void OnFormsWindowPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DualScreenInfo.Current.SpanMode) || e.PropertyName == nameof(DualScreenInfo.Current.IsLandscape))
            {
                SetupViews();
            }
        }
    }
}