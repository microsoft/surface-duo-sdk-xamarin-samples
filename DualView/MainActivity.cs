using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using DualView.Fragments;
using Microsoft.Device.Display;
using System.Collections.Generic;

namespace DualView
{
	[Android.App.Activity(
		Icon = "@mipmap/ic_launcher",
		Label = "@string/app_name",
		RoundIcon = "@mipmap/ic_launcher_round",
		Theme = "@style/AppTheme",
		MainLauncher = true,
		ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.SmallestScreenSize)]

	public class MainActivity : AppCompatActivity, BaseFragment.IOnItemSelectedListener
	{
		ScreenHelper screenHelper;
		bool isDuo;
		Dictionary<string, BaseFragment> fragmentMap;
		int currentSelectedPosition = -1;

		string GetSimpleName<T>()
			=> Java.Lang.Class.FromType(typeof(T)).SimpleName;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_main);
			screenHelper = new ScreenHelper();
			isDuo = screenHelper.Initialize(this);
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

			SetupLayout();
		}

		void UseSingleMode()
		{
			var baseFragment = fragmentMap[GetSimpleName<SinglePortrait>()];
			if (baseFragment != null)
				ShowFragment(baseFragment);
		}

		void UseDualMode(SurfaceOrientation rotation)
		{
			switch (rotation)
			{
				case SurfaceOrientation.Rotation90:
				case SurfaceOrientation.Rotation270:
					// Setting layout for double landscape
					var baseFragment = fragmentMap[GetSimpleName<DualLandscape>()];
					if (baseFragment != null)
						ShowFragment(baseFragment);
					break;
				default:
					var baseFragment1 = fragmentMap[GetSimpleName<DualPortrait>()];
					if (baseFragment1 != null)
						ShowFragment(baseFragment1);
					break;
			}
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

			if (ScreenHelper.IsDualScreenDevice(this))
				screenHelper.Update();

			SetupLayout();
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