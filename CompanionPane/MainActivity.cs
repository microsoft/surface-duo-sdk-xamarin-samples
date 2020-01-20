
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using CompanionPane.Fragments;
using Microsoft.Device.Display;
using System.Linq;

namespace CompanionPane
{
	[Android.App.Activity(
		Label = "@string/app_name",
		Icon = "@mipmap/ic_launcher",
		RoundIcon = "@mipmap/ic_launcher_round",
		Theme = "@style/AppTheme",
		MainLauncher = true,
		ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.SmallestScreenSize)]
	public class MainActivity : AppCompatActivity, BaseFragment.IOnItemSelectedListener
	{
		SinglePortrait singlePortrait;
		DualPortrait dualPortrait;
		DualLandscape dualLandscape;
		ScreenHelper screenHelper;
		bool isDuo;
		int currentPosition;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_main);

			screenHelper = new ScreenHelper();

			isDuo = screenHelper.Initialize(this);

			var slides = Slide.Slides.ToList();

			currentPosition = 0;
			singlePortrait = SinglePortrait.NewInstance(slides);
			singlePortrait.RegisterOnItemSelectedListener(this);
			dualPortrait = DualPortrait.NewInstance(slides);
			dualPortrait.RegisterOnItemSelectedListener(this);
			dualLandscape = DualLandscape.NewInstance(slides);
			dualLandscape.RegisterOnItemSelectedListener(this);
			SetupLayout();
		}

		void ShowFragment(Fragment fragment)
		{
			var fragmentTransaction = SupportFragmentManager.BeginTransaction();
			if (!fragment.IsAdded)
				fragmentTransaction.Add(Resource.Id.activity_main, fragment);
			fragmentTransaction.Show(fragment);
			fragmentTransaction.Commit();
		}

		void HideFragment(Fragment fragment)
		{
			if (fragment.IsAdded)
			{
				var fragmentTransaction = SupportFragmentManager.BeginTransaction();
				fragmentTransaction.Hide(fragment);
				fragmentTransaction.Commit();
			}
		}

		public override void OnConfigurationChanged(Configuration newConfig)
		{
			base.OnConfigurationChanged(newConfig);
			SetupLayout();
		}

		void UseDualMode(SurfaceOrientation rotation)
		{
			if (rotation == SurfaceOrientation.Rotation90 || rotation == SurfaceOrientation.Rotation270)
			{
				dualLandscape.setCurrentPosition(currentPosition);
				ShowFragment(dualLandscape);
				HideFragment(dualPortrait);
				HideFragment(singlePortrait);
			}
			else
			{
				dualPortrait.SetCurrentPosition(currentPosition);
				ShowFragment(dualPortrait);
				HideFragment(singlePortrait);
				HideFragment(dualLandscape);
			}
		}

		void UseSingleMode()
		{
			singlePortrait.SetCurrentPosition(currentPosition);
			ShowFragment(singlePortrait);
			HideFragment(dualLandscape);
			HideFragment(dualPortrait);
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

		public void OnItemSelected(int position)
		{
			currentPosition = position;
		}
	}
}