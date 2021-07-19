using Android.App;
using Android.Content.Res;
using Android.OS;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Fragment = AndroidX.Fragment.App.Fragment;
using AndroidX.Window;
using AndroidX.Core.Util;
using Java.Util.Concurrent;
using Java.Lang;
using Android.Util;
using Android.Views;
using Android.Content;
using Java.Interop;

namespace DragAndDrop
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IConsumer
    {
        const string TAG = "JWM"; // Jetpack Window Manager
        WindowManager wm;
        bool isDuo, isDualMode;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            wm = new WindowManager(this);

            SetupLayout();

            var fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += (s, e) => Recreate();
        }

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
                foreach (var df in wli.DisplayFeatures)
                {
                    Log.Info(TAG, "Bounds:" + df.Bounds);
                    var ff = df as FoldingFeature;
                    if (!(ff is null))
                    {   // a hinge exists
                        Log.Info(TAG, "Orientation: " + ff.GetOrientation());
                        isDualMode = true;
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

        public override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            wm.RegisterLayoutChangeCallback(runOnUiThreadExecutor(), this);
        }

        public override void OnDetachedFromWindow()
        {
            base.OnDetachedFromWindow();
            wm.UnregisterLayoutChangeCallback(this);
        }

        void SetupLayout()
        {
            var bundle = new Bundle();

            var wm = this.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            var rotation = SurfaceOrientation.Rotation0;
            if (wm != null)
                rotation = wm.DefaultDisplay.Rotation;

            if (isDuo && isDualMode)
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