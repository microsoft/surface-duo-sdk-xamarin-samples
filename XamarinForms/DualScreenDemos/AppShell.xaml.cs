using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.DualScreen;

namespace DualScreenDemos
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            DualScreenInfo.Current.PropertyChanged += OnDualScreenInfoChanged;
        }

        private void OnDualScreenInfoChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var info = DualScreenInfo.Current;
            if (info.SpanMode == TwoPaneViewMode.Wide)
            {
                FlyoutHeight = -1;
                FlyoutWidth = info.SpanningBounds[0].Width;
            }
            else if (info.SpanMode == TwoPaneViewMode.Tall)
            {
                FlyoutHeight = info.SpanningBounds[0].Height;
                FlyoutWidth = -1;
                FlyoutBehavior = FlyoutBehavior.Flyout;
            }
            else
            {
                FlyoutHeight = -1;
                FlyoutWidth = -1;
                FlyoutBehavior = FlyoutBehavior.Flyout;
            }
        }

        void DuoImageDragStarting(System.Object sender, Xamarin.Forms.DragStartingEventArgs e)
        {
            e.Data.Text = "https://docs.microsoft.com/en-us/dual-screen/xamarin/use-sdk";
            e.Handled = true;
        }
    }
}
