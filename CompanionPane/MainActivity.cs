
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.Core.Util;
using AndroidX.Fragment.App;
using AndroidX.Window.Layout;
using AndroidX.Window.Java.Layout;
using CompanionPane.Fragments;
using Java.Lang;
using Java.Util.Concurrent;
using System.Linq;
/*
23-Aug-21 Update to androidx.window-1.0.0-beta01
          HACK: need to JavaCast IDisplayFeature to IFoldingFeature
01-Sep-21 Updated to AndroidX.Window-1.0.0-beta02
02-Nov-21 Updated to AndroidX.Window-1.0.0-beta03 (note: beta03 was never deployed to NuGet.org)
02-Dec-21 Updated to AndroidX.Window-1.0.0-beta04
          Renamed WindowInfoRepository to WindowInfoTracker, added Activity context parameter
16-Dec-21 Updated to AndroidX.Window-1.0.0-rc01
27-Jan-22 Updated to AndroidX.Window-1.0.0 stable release!
*/
namespace CompanionPane
{
	[Android.App.Activity(
		Label = "@string/app_name",
		Icon = "@mipmap/ic_launcher",
		RoundIcon = "@mipmap/ic_launcher_round",
		Theme = "@style/AppTheme",
		MainLauncher = true,
		ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.SmallestScreenSize | Android.Content.PM.ConfigChanges.UiMode)]
	public class MainActivity : AppCompatActivity, BaseFragment.IOnItemSelectedListener, IConsumer
	{
		const string TAG = "JWM"; // Jetpack Window Manager
		WindowInfoTrackerCallbackAdapter wit;
		FoldingFeatureOrientation hingeOrientation = FoldingFeatureOrientation.Vertical;
		bool isDuo, isDualMode;

		SinglePortrait singlePortrait;
		DualPortrait dualPortrait;
		DualLandscape dualLandscape;
		int currentPosition;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_main);

			var slides = Slide.Slides.ToList();

			currentPosition = 0;
			singlePortrait = SinglePortrait.NewInstance(slides);
			singlePortrait.RegisterOnItemSelectedListener(this);
			dualPortrait = DualPortrait.NewInstance(slides);
			dualPortrait.RegisterOnItemSelectedListener(this);
			dualLandscape = DualLandscape.NewInstance(slides);
			dualLandscape.RegisterOnItemSelectedListener(this);

			wit = new WindowInfoTrackerCallbackAdapter(WindowInfoTracker.Companion.GetOrCreate(this));
			
			SetupLayout();
		}

		#region Used by WindowInfoRepository callback
		IExecutor runOnUiThreadExecutor()
		{
			return new MyExecutor();
		}
		class MyExecutor : Java.Lang.Object, IExecutor
		{
			Handler handler = new Handler(Looper.MainLooper);
			public void Execute(IRunnable r)
			{
				handler.Post(r);
			}
		}
		#endregion

		public void Accept(Java.Lang.Object newLayoutInfo)  // Object will be WindowLayoutInfo
		{
			Log.Info(TAG, "===LayoutStateChangeCallback.Accept");
			var wli = newLayoutInfo as WindowLayoutInfo;
			if (wli.DisplayFeatures.Count == 0)
			{ // no hinge found
				isDualMode = false;
			}
			else
			{
				foreach (var displayFeature in wli.DisplayFeatures)
				{
					Log.Info(TAG, "Bounds:" + displayFeature.Bounds);

					var foldingFeature = displayFeature.JavaCast<IFoldingFeature>();

					if (!(foldingFeature is null))
					{   // a hinge exists
						Log.Info(TAG, "Orientation: " + foldingFeature.Orientation);
						isDualMode = true;
						hingeOrientation = foldingFeature.Orientation;
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
			wit.AddWindowLayoutInfoListener(this, runOnUiThreadExecutor(), this);
			// first `this` is the Activity context, second `this` is the IConsumer implementation
		}

		protected override void OnStop()
		{
			base.OnStop();
			wit.RemoveWindowLayoutInfoListener(this);
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

        void UseDualMode(FoldingFeatureOrientation hingeOrientation)
		{
			if (hingeOrientation == FoldingFeatureOrientation.Horizontal)
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

		public void OnItemSelected(int position)
		{
			currentPosition = position;
		}
	}
}