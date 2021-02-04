using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DualScreenDemos
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            twoPaneView.TallModeConfiguration = Xamarin.Forms.DualScreen.TwoPaneViewTallModeConfiguration.TopBottom;
        }

        void OnNavigateToSample(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            switch(button.Text)
            {
                case "Two Page":
                    Navigation.PushAsync(new TwoPage());
                    break;
                case "Dual View":
                    Navigation.PushAsync(new DualViewListPage());
                    break;
                case "Extend Canvas":
                    Navigation.PushAsync(new ExtendCanvas());
                    break;
                case "List Detail":
                    Navigation.PushAsync(new ListDetail());
                    break;
                case "Companion Pane":
                    Navigation.PushAsync(new CompanionPane());
                    break;
                case "TwoPaneView Playground":
                    Navigation.PushAsync(new TwoPanePropertiesGallery());
                    break;
                case "Nested TwoPaneView Split Across Hinge":
                    Navigation.PushAsync(new NestedTwoPaneViewSplitAcrossHinge());
                    break;
                case "DualScreenInfo with non TwoPaneView":
                    Navigation.PushAsync(new GridUsingDualScreenInfo());
                    break;
                case "Drag And Drop":
                    Navigation.PushAsync(new DragAndDrop());
                    break;
                case "Dual Screen Info":
                    Navigation.PushAsync(new DualScreenInfoGallery());
                    break;

            }
        }
    }
}
