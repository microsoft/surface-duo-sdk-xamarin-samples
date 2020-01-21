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

            detailsPage.BindingContext = e.CurrentSelection[0];
            SetupViews();
        }

        async void SetupViews()
        {
            if (masterPage.SelectedItem == null && FormsWindow.IsSpanned)
                masterPage.SelectedItem = (masterPage.ItemsSource as IList<MasterDetailsItem>)[0];

            GetDetailsContent().BindingContext = masterPage.SelectedItem;
            if (FormsWindow.IsSpanned)
            {
                if(Navigation.NavigationStack.Contains(detailsPagePushed))
                {
                    await Navigation.PopAsync();
                }
            }
            else
            {
                if (!Navigation.NavigationStack.Contains(detailsPagePushed))
                {
                    await Navigation.PushAsync(detailsPagePushed);
                }
            }
        }

        void OnFormsWindowPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(FormsWindow.IsSpanned))
            {
                SetupViews();
            }
        }

        BindableObject GetDetailsContent()
        {
            if(FormsWindow.IsSpanned)
            {
                return detailsPage;
            }

            return detailsPagePushed;
        }
    }
}