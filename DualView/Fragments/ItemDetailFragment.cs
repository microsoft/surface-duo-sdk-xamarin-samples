using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using AndroidX.Fragment.App;
using Java.Interop;

namespace DualView.Fragments
{

	public class ItemDetailFragment : Fragment
	{
		WebView webView;

		public double Lat { get; private set; }
		public double Lng { get; private set; }

		// These are the javascript callbacks for Assets/googlemap.html
		[Export("getLat")]
		public double getLat()
			=> Lat;

		[Export("getLng")]
		public double getLng()
			=> Lng;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			if (Arguments != null)
			{
				var item = Arguments.GetParcelable(Item.KEY).JavaCast<Item>();
				if (item != null && item.Location != null)
				{
					Lat = item.Location.X;
					Lng = item.Location.Y;
					var title = item.ToString();
					if (Activity != null)
						Activity.Title = title;
				}
			}
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.fragment_item_detail,
					container, false);
			webView = view.FindViewById<WebView>(Resource.Id.web_view);
			webView.Settings.JavaScriptEnabled = true;
			webView.AddJavascriptInterface(this, "AndroidFunction");
			webView.SetWebViewClient(new WebViewClient());
			webView.LoadUrl("file:///android_asset/googlemap.html");
			return view;
		}

		public static ItemDetailFragment NewInstance(Item item)
		{
			var fragment = new ItemDetailFragment();
			Bundle args = new Bundle();
			args.PutParcelable(Item.KEY, item);
			fragment.Arguments = args;
			return fragment;
		}
	}
}