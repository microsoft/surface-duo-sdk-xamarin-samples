using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.ViewPager.Widget;
using AndroidX.Window;
using AndroidX.Core;
using AndroidX.Core.Util;
using AndroidX.Annotations;
using AndroidX.Collection;
using Java.Lang;
using Java.Util.Concurrent;
using Android.Util;
/*
15-Apr-21 Use androidx.window-1.0.0-alpha01
          This is a terrible hack that just aims to get the basics of Window Manager working
          It doesn't properly handle rotation - just single-portrait/dual-portrait 

19-Apr-21 Update to androidx.window-1.0.0-alpha05
          Discovered methods are missing from the binding

20-Apr-21 Update to androidx.window-1.0.0.1-alpha05 (adds RegisterLayoutChangeCallback method)
          Registering and receiving events works, but the orientation value seems off
          IConsumer.Accept is added to the Activity so that it can receive method calls on layout state changed

21-Apr-21 Refactor out test code, seems to work...
19-Jul-21 Update to androidx.window-1.0.0-apha09
		  FoldingFeature API changes - some properties became methods (GetOrientation, GetState, GetOcclusionType) and their types became "enums" (static class fields)
		  Use OnStart/Stop instead of OnAttachedToWindow/OnDetached
*/
namespace TwoPage
{
	[Android.App.Activity(
		Icon = "@mipmap/ic_launcher",
		Label = "@string/app_name",
		RoundIcon = "@mipmap/ic_launcher_round",
		Theme = "@style/AppTheme",
		MainLauncher = true,
		ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.SmallestScreenSize)]
	public class MainActivity : AppCompatActivity, ViewPager.IOnPageChangeListener, IConsumer
	{
		const string TAG = "JWM"; // Jetpack Window Manager
		WindowManager wm;
		FoldingFeature.Orientation hingeOrientation = FoldingFeature.Orientation.Vertical;
		bool isDuo, isDualMode; 
		
		ViewPager viewPager;
		PagerAdapter pagerAdapter;

		/// <summary>Page number</summary>
		int position = 0;
		View single;
		View dual;

		//LayoutStateChangeCallback layoutStateChangeCallback = new LayoutStateChangeCallback();

		public bool ShowTwoPages { get; set; } = false;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			var fragments = TestFragment.Fragments;
			pagerAdapter = new PagerAdapter(SupportFragmentManager, fragments);

			wm = new WindowManager(this); 
			
			single = LayoutInflater.Inflate(Resource.Layout.activity_main, null);
			dual = LayoutInflater.Inflate(Resource.Layout.double_landscape_layout, null);
			SetupLayout();
		}

        IExecutor runOnUiThreadExecutor() {
			return new MyExecutor();
        }
		class MyExecutor : Java.Lang.Object, IExecutor { 
			Handler handler = new Handler(Looper.MainLooper);
			public void Execute(IRunnable r)
            {
				handler.Post(r);
            }
        }

        [System.Obsolete("Sample uses the activity as the callback method host - keeping this for demonstration purposes only")]
		class LayoutStateChangeCallback : Java.Lang.Object, IConsumer
        {
            public void Accept(Java.Lang.Object newLayoutInfo) // Object will be WindowLayoutInfo
            {
				Log.Info(TAG, newLayoutInfo.ToString());
				var wli = newLayoutInfo as WindowLayoutInfo; // cast to Type
				foreach (var df in wli.DisplayFeatures)
				{
					Log.Info(TAG, "Bounds:" + df.Bounds);
					//var ff = df as FoldingFeature;
					if ((df is FoldingFeature ff))
					{
						Log.Info(TAG, "IsSeparating: " + ff.IsSeparating);
						Log.Info(TAG, "OcclusionMode: " + ff.GetOcclusionType());
						Log.Info(TAG, "Orientation: " + ff.GetOrientation());
						Log.Info(TAG, "State: " + ff.GetState());
					}
				}
            }
        }
		
		public void Accept(Java.Lang.Object newLayoutInfo)  // Object will be WindowLayoutInfo
		{
			Log.Info(TAG, "===LayoutStateChangeCallback.Accept");
			Log.Info(TAG, newLayoutInfo.ToString());
			var wli = newLayoutInfo as WindowLayoutInfo;
			if (wli.DisplayFeatures.Count == 0)
			{ // no hinge found
				isDualMode = false;
			}
			else
			{
				foreach (var df in wli.DisplayFeatures)
				{
					Log.Info(TAG, "Bounds:" + df.Bounds);
					Log.Info(TAG, "(deprecated)Type: {df.Type} (FOLD or HINGE)");
					var ff = df as FoldingFeature;
					if (!(ff is null))
					{   // a hinge exists
						Log.Info(TAG, "IsSeparating: " + ff.IsSeparating);
						Log.Info(TAG, "OcclusionMode: " + ff.GetOcclusionType());
						Log.Info(TAG, "Orientation: " + ff.GetOrientation());
						Log.Info(TAG, "State: " + ff.GetState());
						isDualMode = true;
						hingeOrientation = ff.GetOrientation();
						isDuo = true; //HACK: set first time we see the hinge, never un-set
					}
					else
					{ // no hinge found
						isDualMode = false;
					}
				}
			}
			SetupLayout();
		}

		protected override void OnStart()
		{
			base.OnStart();
			wm.RegisterLayoutChangeCallback(runOnUiThreadExecutor(), this);
		}

		protected override void OnStop()
		{
			base.OnStop();
			wm.UnregisterLayoutChangeCallback(this);
		}

		void UseSingleMode()
		{
			//Setting layout for single portrait
			SetContentView(single);
			ShowTwoPages = false;
			SetupViewPager();
		}

		void UseDualMode(FoldingFeature.Orientation hingeOrientation)
		{
			if (hingeOrientation == FoldingFeature.Orientation.Horizontal)
			{	// hinge horizontal - setting layout for double landscape
				SetContentView(dual);
				ShowTwoPages = false;
			}
			else 
			{	//includes FoldingFeature.OrientationVertical
				// hinge vertical - setting layout for double portrait
				SetContentView(single);
				ShowTwoPages = true;
			}
			SetupViewPager();
		}

		void SetupLayout()
		{
			if (isDuo)
			{
				if (isDualMode)
					UseDualMode(hingeOrientation);
				else
					UseSingleMode();
			}
			else
			{
				UseSingleMode();
			}
		}

        void SetupViewPager()
		{
			pagerAdapter.ShowTwoPages = ShowTwoPages;
			if (viewPager != null)
				viewPager.Adapter = null;

			viewPager = FindViewById<ViewPager>(Resource.Id.pager);
			viewPager.Adapter = pagerAdapter;
			viewPager.CurrentItem = position;
			viewPager.AddOnPageChangeListener(this);
		}

		public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
		{
			//
		}

		public void OnPageSelected(int position)
		{
			this.position = position;
		}

		public void OnPageScrollStateChanged(int state)
		{
			//
		}
	}
}