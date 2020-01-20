using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.ViewPager.Widget;
using Microsoft.Device.Display;

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
		ScreenHelper screenHelper;
		int position = 0;
		bool isDuo;
		View single;
		View dual;

		public bool ShowTwoPages { get; set; } = false;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			var fragments = TestFragment.Fragments;
			pagerAdapter = new PagerAdapter(SupportFragmentManager, fragments);
			screenHelper = new ScreenHelper();
			isDuo = screenHelper.Initialize(this);
			single = LayoutInflater.Inflate(Resource.Layout.activity_main, null);
			dual = LayoutInflater.Inflate(Resource.Layout.double_landscape_layout, null);
			SetupLayout();
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
			var rotation = ScreenHelper.GetRotation(this);
			if (isDuo)
			{
				if (screenHelper.IsDualMode)
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