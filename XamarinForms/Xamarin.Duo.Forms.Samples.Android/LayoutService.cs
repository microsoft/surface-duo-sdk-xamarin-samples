using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Duo.Forms.Samples;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: Dependency(typeof(LayoutService))]
namespace Xamarin.Duo.Forms.Samples
{
    public class LayoutService : LayoutServiceBase, ILayoutService
    {
        public override Point? GetLocationOnScreen(VisualElement visualElement)
        {
            var view = Platform.GetRenderer(visualElement);

            if (view?.View == null)
                return null;

            int[] location = new int[2];
            view.View.GetLocationOnScreen(location);
            return new Point(view.View.Context.FromPixels(location[0]), view.View.Context.FromPixels(location[1]));
        }
    }
}