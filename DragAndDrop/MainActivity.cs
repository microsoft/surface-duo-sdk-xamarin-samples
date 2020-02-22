using Android.App;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using Google.Android.Material.FloatingActionButton;
using Microsoft.Device.Display;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace DragAndDrop
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        ScreenHelper screenHelper;
        bool isDuo = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            screenHelper = new ScreenHelper();
            isDuo = screenHelper.Initialize(this);

            SetupLayout();

            var fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += (s, e) => Recreate();
        }

        void SetupLayout()
        {
            var bundle = new Bundle();
            var rotation = ScreenHelper.GetRotation(this);

            if (isDuo && screenHelper.IsDualMode)
            {
                switch (rotation)
                {
                    case Android.Views.SurfaceOrientation.Rotation90:
                    case Android.Views.SurfaceOrientation.Rotation270:
                        // Setting layout for double landscape
                        bundle.PutInt(AdaptiveFragment.KEY_LAYOUT_ID, Resource.Layout.fragment_dual_landscape);
                        break;
                    default:
                        bundle.PutInt(AdaptiveFragment.KEY_LAYOUT_ID, Resource.Layout.fragment_dual_portrail);
                        break;
                }
            }
            else
            {
                bundle.PutInt(AdaptiveFragment.KEY_LAYOUT_ID, Resource.Layout.fragment_single_portrait);
            }

            Fragment adaptiveFragment = AdaptiveFragment.NewInstance();
            adaptiveFragment.Arguments = bundle;
            ShowFragment(adaptiveFragment);
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            SetupLayout();
        }

        void ShowFragment(Fragment fragment)
        {
            SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.activity_main, fragment)
                    .Commit();
        }
    }
}