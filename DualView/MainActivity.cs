using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using DualView.Fragments;
using System.Collections.Generic;
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
02-Nov-21 Updated to AndroidX.Window-1.0.0-beta03
*/
namespace DualView
{
	[Android.App.Activity(
		Icon = "@mipmap/ic_launcher",
		Label = "@string/app_name",
		RoundIcon = "@mipmap/ic_launcher_round",
		Theme = "@style/AppTheme",
		MainLauncher = true,
		ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.SmallestScreenSize | Android.Content.PM.ConfigChanges.UiMode)]

	public class MainActivity : AppCompatActivity, BaseFragment.IOnItemSelectedListener, IConsumer
	{
		const string TAG = "JWM"; // Jetpack Window Manager
		WindowInfoRepositoryCallbackAdapter wir;
		FoldingFeatureOrientation hingeOrientation = FoldingFeatureOrientation.Vertical;
		bool isDuo, isDualMode;

		Dictionary<string, BaseFragment> fragmentMap;
		int currentSelectedPosition = -1;

		string GetSimpleName<T>()
			=> Java.Lang.Class.FromType(typeof(T)).SimpleName;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_main);
			var items = Item.Items;
			fragmentMap = new Dictionary<string, BaseFragment>();

			SinglePortrait singlePortrait = SinglePortrait.NewInstance(items);
			singlePortrait.RegisterOnItemSelectedListener(this);
			fragmentMap[GetSimpleName<SinglePortrait>()] = singlePortrait;

			var dualPortrait = DualPortrait.NewInstance(items);
			dualPortrait.RegisterOnItemSelectedListener(this);
			fragmentMap[GetSimpleName<DualPortrait>()] = dualPortrait;

			var dualLandscape = DualLandscape.NewInstance(items);
			dualLandscape.RegisterOnItemSelectedListener(this);
			fragmentMap[GetSimpleName<DualLandscape>()] = dualLandscape;

			wir = new WindowInfoRepositoryCallbackAdapter(WindowInfoRepository.Companion.GetOrCreate(this));
			wir.AddWindowLayoutInfoListener(runOnUiThreadExecutor(), this);


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
			wir.AddWindowLayoutInfoListener(runOnUiThreadExecutor(), this);
		}

		protected override void OnStop()
		{
			base.OnStop();
			wir.RemoveWindowLayoutInfoListener(this);
		}

		void UseSingleMode()
		{
			var baseFragment = fragmentMap[GetSimpleName<SinglePortrait>()];
			if (baseFragment != null)
				ShowFragment(baseFragment);
		}

		void UseDualMode(FoldingFeatureOrientation hingeOrientation)
		{
			if (hingeOrientation == FoldingFeatureOrientation.Horizontal)
			{   // hinge horizontal - setting layout for double landscape
				var baseFragment = fragmentMap[GetSimpleName<DualLandscape>()];
				if (baseFragment != null)
				{
					ShowFragment(baseFragment);
				}
			} 
			else
			{	//includes FoldingFeature.Orientation.Vertical
				// hinge vertical - setting layout for double portrait
				var baseFragment1 = fragmentMap[GetSimpleName<DualPortrait>()];
				if (baseFragment1 != null)
				{
					ShowFragment(baseFragment1);
				}
			}
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

		void ShowFragment(BaseFragment fragment)
		{
			var fragmentTransaction = SupportFragmentManager.BeginTransaction();
			if (!fragment.IsAdded)
				fragmentTransaction.Add(Resource.Id.activity_main, fragment);

			fragmentTransaction.Show(fragment);

			if (currentSelectedPosition != -1)
				fragment.SetCurrentSelectedPosition(currentSelectedPosition);

			foreach (var kvp in fragmentMap)
			{
				if (kvp.Value != fragment)
					fragmentTransaction.Hide(kvp.Value);
			}
			fragmentTransaction.Commit();
		}

		public override void OnBackPressed()
		{
			SetTitle(Resource.String.app_name);

			foreach (var kvp in fragmentMap)
			{
				var fragment = kvp.Value.JavaCast<BaseFragment>();
				if (fragment.IsVisible)
				{
					if (fragment.OnBackPressed())
						this.Finish();
				}
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

		public void OnItemSelected(int position)
		{
			currentSelectedPosition = position;
		}
	}
}