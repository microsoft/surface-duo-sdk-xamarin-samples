using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.ViewPager.Widget;
//using Microsoft.Device.Display; HACK: WM alpha
using AndroidX.Window;
using AndroidX.Window.Extensions;
/*
15-Apr-21

This is a terrible hack that just aims to get the basics of Window Manager working

It doesn't properly handle rotation - just single-portrait/dual-portrait 

TODO: Update to androidx.window-1.0.0-alpha05

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
	public class MainActivity : AppCompatActivity, ViewPager.IOnPageChangeListener
	{
		ViewPager viewPager;
		PagerAdapter pagerAdapter;
		//ScreenHelper screenHelper;
		int position = 0;
		bool isDuo, isDualMode;
		View single;
		View dual;

		WindowManager wm; // HACK: alpha WM

		public bool ShowTwoPages { get; set; } = false;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			var fragments = TestFragment.Fragments;
			pagerAdapter = new PagerAdapter(SupportFragmentManager, fragments);
			//screenHelper = new ScreenHelper(); // HACK: alpha WM
			//isDuo = screenHelper.Initialize(this); // HACK: alpha WM
			wm = new WindowManager(this, null); // HACK: alpha WM
			
			single = LayoutInflater.Inflate(Resource.Layout.activity_main, null);
			dual = LayoutInflater.Inflate(Resource.Layout.double_landscape_layout, null);
			SetupLayout();
		}

        public override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
			var li = wm.WindowLayoutInfo;
        }
        void UseSingleMode()
		{
			//Setting layout for single portrait
			SetContentView(single);
			ShowTwoPages = false;
			SetupViewPager();
		}

		void UseDualMode(SurfaceOrientation rotation)
		{
			switch (rotation)
			{
				case SurfaceOrientation.Rotation90:
				case SurfaceOrientation.Rotation270:
					// Setting layout for double landscape
					SetContentView(dual);
					ShowTwoPages = false;
					break;
				default:
					// Setting layout for double portrait
					SetContentView(single);
					ShowTwoPages = true;
					break;
			}
			SetupViewPager();
		}

		void SetupLayout()
		{
			var rotation = Android.Views.SurfaceOrientation.Rotation0; //HACK: alpha WM ScreenHelper.GetRotation(this);
			if (isDuo)
			{
				if (isDualMode)  // HACK: alpha WM (screenHelper.IsDualMode)
					UseDualMode(rotation);
				else
					UseSingleMode();
			}
			else
			{
				UseSingleMode();
			}
		}

		public override void OnConfigurationChanged(Configuration newConfig)
		{
			base.OnConfigurationChanged(newConfig);

			// HACK: alpha WM
			//			if (ScreenHelper.IsDualScreenDevice(this))
			//				screenHelper.Update();

			var li = wm.WindowLayoutInfo; // HACK: alpha WM
			if (li.DisplayFeatures.Count > 0) // HACK: alpha WM
			{
				isDuo = true;
				isDualMode = true;
				var hinge = li.DisplayFeatures[0].Bounds; // just for debugging, shows "Rect(1350,0 - 1434,1800)"
			}
			else
			{
				isDualMode = false;
			}

			SetupLayout();
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