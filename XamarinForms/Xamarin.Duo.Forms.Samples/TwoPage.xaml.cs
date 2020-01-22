using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Duo.Forms.Samples
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TwoPage : DuoPage
    {
        IItemsLayout linearLayout = null;
        IItemsLayout gridLayout = null;
        bool disableUpdates = false;

        public TwoPage()
        {
            InitializeComponent();
            cv.ItemsSource =
                Enumerable.Range(0, 1000)
                    .Select(i => $"Page {i}")
                    .ToList();

            FormsWindow.PropertyChanged += OnFormsWindowPropertyChanged;
        }

        void OnFormsWindowPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Content == null || disableUpdates)
                return;

            if(e.PropertyName == nameof(FormsWindow.IsLandscape) || e.PropertyName == nameof(FormsWindow.IsPortrait))
            {                
                SetupColletionViewLayout();
            }
            else if (e.PropertyName == nameof(FormsWindow.Pane2))
            {
                OnPropertyChanged(nameof(ContentHeight));
                OnPropertyChanged(nameof(ContentWidth));
            }
        }

        public double ContentHeight => (FormsWindow.IsPortrait) ? FormsWindow.Pane1.Height :  FormsWindow.Pane1.Height + FormsWindow.Pane2.Height;

        public double ContentWidth => (FormsWindow.Pane1.Width);
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            SetupColletionViewLayout();
        }

        void SetupColletionViewLayout()
        {
            disableUpdates = true;
            var resetCV = cv;
            if (linearLayout == null && cv.ItemsLayout is LinearItemsLayout linear)
            {
                linearLayout = cv.ItemsLayout;
                linear.SnapPointsType = SnapPointsType.None;
                linear.SnapPointsAlignment = SnapPointsAlignment.Start;
            }

            if (gridLayout == null && cv.ItemsLayout is GridItemsLayout)
                gridLayout = cv.ItemsLayout;
            
            if (FormsWindow.IsLandscape)
            {
                if (cv.ItemsLayout != linearLayout)
                {
                    resetCV.ItemsSource = null;
                    resetCV.ItemsLayout = linearLayout;
                    Content = null;
                }
            }
            else
            {
                if (cv.ItemsLayout != gridLayout)
                {
                    resetCV.ItemsSource = null;
                    resetCV.ItemsLayout = gridLayout;
                    Content = null;
                }
            }

            if (Content == null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Content = resetCV;
                    resetCV.ItemsSource =
                        Enumerable.Range(0, 1000)
                            .Select(i => $"Page {i}")
                            .ToList();

                    disableUpdates = false;
                });
            }
            else
            {
                disableUpdates = false;
            }
        }
    }
}