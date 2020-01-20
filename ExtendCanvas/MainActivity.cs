
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Webkit;
using Android.Widget;
using AndroidX.AppCompat.App;
using Java.Interop;

namespace ExtendCanvas
{
	[Activity(
		Icon = "@mipmap/ic_launcher",
		Label = "@string/app_name",
		RoundIcon = "@mipmap/ic_launcher_round",
		Theme = "@style/AppTheme",
		MainLauncher = true,
		ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.SmallestScreenSize)]
	public class MainActivity : AppCompatActivity
	{
		SearchView searchView;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_main);

			SetupWebView();
		}

		void SetupWebView()
		{
			var webView = FindViewById<WebView>(Resource.Id.web_view);
			webView.Settings.JavaScriptEnabled = true;
			// Injects the supplied Java object into WebView
			webView.AddJavascriptInterface(this, "AndroidFunction");
			webView.SetWebViewClient(new WebViewClient());
			webView.LoadUrl("file:///android_asset/googlemapsearch.html");
		}

		void StartSearch()
		{
			PlaceToGo = searchView.Query.ToString();
			SetupWebView();
			HideKeyboard();
		}

		public string PlaceToGo { get; private set; }

		// This callback is for assets/googlemapsearch.html
		[Export("placeToGo")]
		public string placeToGo()
			=> PlaceToGo;

		private void HideKeyboard()
		{
			var view = this.CurrentFocus;
			if (view == null)
				view = new View(this);
			var imm = (InputMethodManager)this.GetSystemService(Android.App.Activity.InputMethodService);
			if (imm != null)
				imm.HideSoftInputFromWindow(view.WindowToken, 0);
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			var inflater = MenuInflater;
			inflater.Inflate(Resource.Menu.search_bar, menu);

			var searchManager = (SearchManager)GetSystemService(Context.SearchService);
			var searchView = (SearchView)menu.FindItem(Resource.Id.action_search).ActionView;
			if (searchManager != null && searchManager.GetSearchableInfo(ComponentName) != null)
				searchView.SetSearchableInfo(searchManager.GetSearchableInfo(ComponentName));
			searchView.SetIconifiedByDefault(false);
			searchView.Focusable = true;
			searchView.RequestFocusFromTouch();
			searchView.OnActionViewExpanded();
			searchView.Touch += SearchView_Touch;
			searchView.QueryTextSubmit += SearchView_QueryTextSubmit;
			this.searchView = searchView;
			SetupWebView();
			return true;
		}

		void SearchView_QueryTextSubmit(object sender, SearchView.QueryTextSubmitEventArgs e)
		{
			StartSearch();
			e.Handled = false;
		}

		void SearchView_Touch(object sender, View.TouchEventArgs e)
		{
			if (e.Event.Action == MotionEventActions.Down)
			{
				StartSearch();
				e.Handled = true;
			}
		}
	}
}