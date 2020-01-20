
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using MasterDetail.Fragments;
using Microsoft.Device.Display;

namespace MasterDetail
{
	[Android.App.Activity(
		Icon = "@mipmap/ic_launcher",
		Label = "@string/app_name",
		RoundIcon = "@mipmap/ic_launcher_round",
		Theme = "@style/AppTheme",
		MainLauncher = true,
		ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.SmallestScreenSize)]
	public class MainActivity : AppCompatActivity
	{
		ScreenHelper screenHelper;
		bool isDuo;
		SinglePortrait singlePortrait;
		DualPortrait dualPortrait;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_main);
			screenHelper = new ScreenHelper();
			isDuo = screenHelper.Initialize(this);
			var items = Item.Items;
			singlePortrait = SinglePortrait.NewInstance(items);
			dualPortrait = DualPortrait.NewInstance(items);
			SetupLayout();
		}

		void UseSingleMode()
			=> ShowFragment(singlePortrait);

		void UseDualMode(SurfaceOrientation rotation)
		{
			switch (rotation)
			{
				case SurfaceOrientation.Rotation90:
				case SurfaceOrientation.Rotation270:
					// Setting layout for double landscape
					UseSingleMode();
					break;
				default:
					ShowFragment(dualPortrait);
					break;
			}
		}

		private void SetupLayout()
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