
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using ListDetail.Fragments;
using AndroidX.Window.Layout;
using AndroidX.Window.Java.Layout;
using AndroidX.Core.Util;
using Java.Util.Concurrent;
using Java.Lang;
using Android.Util;
/*
23-Aug-21 Update to androidx.window-1.0.0-beta01
          HACK: need to JavaCast IDisplayFeature to IFoldingFeature
01-Sep-21 Updated to AndroidX.Window-1.0.0-beta02
02-Nov-21 Updated to AndroidX.Window-1.0.0-beta03 (note: beta03 was never deployed to NuGet.org)
02-Dec-21 Updated to AndroidX.Window-1.0.0-beta04
          Renamed WindowInfoRepository to WindowInfoTracker, added Activity context parameter
*/
namespace ListDetail
{
	[Android.App.Activity(
		Icon = "@mipmap/ic_launcher",
		Label = "@string/app_name",
		RoundIcon = "@mipmap/ic_launcher_round",
		Theme = "@style/AppTheme",
		MainLauncher = true,
		ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.SmallestScreenSize)]
	public class MainActivity : AppCompatActivity, IConsumer
	{
		const string TAG = "JWM"; // Jetpack Window Manager
		WindowInfoTrackerCallbackAdapter wit;
		FoldingFeatureOrientation hingeOrientation = FoldingFeatureOrientation.Vertical;
		bool isDuo, isDualMode;

		SinglePortrait singlePortrait;
		DualPortrait dualPortrait;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_main);
			var items = Item.Items;
			singlePortrait = SinglePortrait.NewInstance(items);
			dualPortrait = DualPortrait.NewInstance(items);

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

		void UseSingleMode()
			=> ShowFragment(singlePortrait);

		void UseDualMode(FoldingFeatureOrientation hingeOrientation)
		{
			if (hingeOrientation == FoldingFeatureOrientation.Horizontal)
			{
				// hinge horizontal - setting layout for double landscape
				UseSingleMode();
			} else {
				//includes FoldingFeature.Orientation.Vertical
				// hinge vertical - setting layout for double portrait
				ShowFragment(dualPortrait);
			}
		}

		private void SetupLayout()
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

		void ShowFragment(Fragment fragment)
		{
			var fragmentTransaction = SupportFragmentManager.BeginTransaction();
			if (!fragment.IsAdded)
				fragmentTransaction.Add(Resource.Id.activity_main, fragment);

			var isSingle = false;

			try { isSingle = fragment.JavaCast<SinglePortrait>() != null; }
			catch { }

			if (isSingle)
			{
				fragmentTransaction.Hide(dualPortrait);
				fragmentTransaction.Show(singlePortrait);
				singlePortrait.SetCurrentSelectedPosition(dualPortrait.GetCurrentSelectedPosition());
			}
			else
			{
				fragmentTransaction.Show(dualPortrait);
				fragmentTransaction.Hide(singlePortrait);
				dualPortrait.SetCurrentSelectedPosition(singlePortrait.GetCurrentSelectedPosition());
			}
			fragmentTransaction.Commit();
		}

		public override void OnBackPressed()
		{
			if (singlePortrait.IsVisible)
			{
				if (singlePortrait.OnBackPressed())
					this.Finish();
			}
			else
			{
				if (dualPortrait.OnBackPressed())
					this.Finish();
			}
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			if (item.ItemId == Android.Resource.Id.Home)
			{
				OnBackPressed();
				return true;
			}
			return base.OnOptionsItemSelected(item);
		}
	}
}