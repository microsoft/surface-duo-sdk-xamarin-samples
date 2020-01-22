using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Xamarin.Duo.Forms.Samples
{
    public class DuoPage : ContentPage
    {
        public static readonly BindableProperty FormsWindowProperty = BindableProperty.Create("FormsWindow", typeof(FormsWindow), typeof(DuoPage), 
            defaultValueCreator: OnDefaultValueCreator);

        private static object OnDefaultValueCreator(BindableObject bindable)
        {
            return new FormsWindow((ContentPage)bindable);
        }

        public FormsWindow FormsWindow
        {
            get { return (FormsWindow)GetValue(FormsWindowProperty); }
            set { SetValue(FormsWindowProperty, value); }
        }


        public DuoPage()
        {
        }
    }
}