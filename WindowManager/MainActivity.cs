using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.Util;
using AndroidX.Window.Layout;
using AndroidX.Window.Java.Layout;
using Java.Lang;
using Java.Util.Concurrent;
using Java.Interop;

/*
 This sample is a C# port of this Kotlin code
 https://github.com/googlecodelabs/android-foldable-codelab/tree/main/window-manager
 which is part of a Google Codelab that explains how to use Window Manager

19-Jul-21 Update to androidx.window-1.0.0-alpha09
		  FoldingFeature API changes - some properties became methods (GetOrientation, GetState, GetOcclusionType) and their types became "enums" (static class fields)
          Use OnStart/Stop instead of OnAttachedToWindow/OnDetached
17-Aug-21 Updated to AndroidX.Window-1.0.0-alpha10 with
          AndroidX.Window.Java-1.0.0-alpha10 Java-compatibility API
18-Aug-21 Updated to AndroidX.Window-1.0.0-beta01
          Changing IFoldingFeature to interface broke the 'automatic' casting :(
          HACK: need to JavaCast IDisplayFeature to IFoldingFeature
01-Sep-21 Updated to AndroidX.Window-1.0.0-beta02
02-Nov-21 Updated to AndroidX.Window-1.0.0-beta03 (note: beta03 was never deployed to NuGet.org)
02-Dec-21 Updated to AndroidX.Window-1.0.0-beta04
          Renamed WindowInfoRepository to WindowInfoTracker, added Activity context parameter
16-Dec-21 Updated to AndroidX.Window-1.0.0-rc01
27-Jan-22 Updated to AndroidX.Window-1.0.0 stable release!
*/
namespace WindowManagerDemo
{
    [Activity(Label = "@string/app_name",
    Theme = "@style/AppTheme",
    MainLauncher = true)]    //ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.SmallestScreenSize)]
    public class MainActivity : AppCompatActivity, IConsumer
    {
        const string TAG = "JWM"; // Jetpack Window Manager
        WindowInfoTrackerCallbackAdapter wit;
        IWindowMetricsCalculator wmc;

        ConstraintLayout constraintLayout;
        TextView windowMetrics, layoutChange, configurationChanged;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            wit = new WindowInfoTrackerCallbackAdapter(WindowInfoTracker.Companion.GetOrCreate(this));
            wmc = WindowMetricsCalculator.Companion.OrCreate; // HACK: source method is `getOrCreate`, binding generator munges this badly :(
           
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            constraintLayout = FindViewById<ConstraintLayout>(Resource.Id.constraint_layout);
            windowMetrics = FindViewById<TextView>(Resource.Id.window_metrics);
            layoutChange = FindViewById<TextView>(Resource.Id.layout_change);
            configurationChanged = FindViewById<TextView>(Resource.Id.configuration_changed);
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

        void printLayoutStateChange(WindowLayoutInfo newLayoutInfo)
        {
            Log.Info(TAG, wmc.ComputeCurrentWindowMetrics(this).Bounds.ToString());
            Log.Info(TAG, wmc.ComputeMaximumWindowMetrics(this).Bounds.ToString());
            windowMetrics.Text = $"CurrentWindowMetrics: {wmc.ComputeCurrentWindowMetrics(this).Bounds}\n" +
                $"MaximumWindowMetrics: {wmc.ComputeMaximumWindowMetrics(this).Bounds}";

            layoutChange.Text = newLayoutInfo.ToString();

            configurationChanged.Text = "One logic/physical display - unspanned";
            
            foreach (var displayFeature in newLayoutInfo.DisplayFeatures)
            {

                var foldingFeature = displayFeature.JavaCast<IFoldingFeature>();

                if (foldingFeature != null) // HACK: requires JavaCast as shown above
                {
                    alignViewToDeviceFeatureBoundaries(newLayoutInfo);

                    if (foldingFeature.OcclusionType == FoldingFeatureOcclusionType.None)
                    {
                        configurationChanged.Text = "App is spanned across a fold";
                    }
                    if (foldingFeature.OcclusionType == FoldingFeatureOcclusionType.Full)
                    {
                        configurationChanged.Text = "App is spanned across a hinge";
                    }
                    configurationChanged.Text += "\nIsSeparating: " + foldingFeature.IsSeparating
                            + "\nOrientation: " + foldingFeature.Orientation  // FoldingFeature.OrientationVertical or Horizontal
                            + "\nState: " + foldingFeature.State; // FoldingFeature.StateFlat or StateHalfOpened
                }
                else
                {
                    Log.Info(TAG, "DisplayFeature is not a fold or hinge");
                }
            }
        }

        void alignViewToDeviceFeatureBoundaries(WindowLayoutInfo newLayoutInfo)
        {
            var set = new ConstraintSet();
            set.Clone(constraintLayout); // existing constraints baseline
            var foldFeature = newLayoutInfo.DisplayFeatures[0].JavaCast<IFoldingFeature>(); // HACK: as IFoldingFeature;
            //We get the display feature bounds.
            var rect = foldFeature.Bounds;
            //Set the red hinge indicator's width and height using the Bounds
            set.ConstrainHeight(Resource.Id.device_feature, rect.Bottom - rect.Top);
            set.ConstrainWidth(Resource.Id.device_feature, rect.Right - rect.Left);

            set.Connect(
                Resource.Id.device_feature, ConstraintSet.Start,
                ConstraintSet.ParentId, ConstraintSet.Start, 0
            );

            set.Connect(
                Resource.Id.device_feature, ConstraintSet.Top,
                ConstraintSet.ParentId, ConstraintSet.Top, 0
            );

            if (foldFeature.Orientation == FoldingFeatureOrientation.Vertical)
            {
                // Device feature is placed vertically
                set.SetMargin(Resource.Id.device_feature, ConstraintSet.Start, rect.Left);
                set.Connect(
                    Resource.Id.layout_change, ConstraintSet.End,
                    Resource.Id.device_feature, ConstraintSet.Start, 0
                );
            }
            else
            {
                //Device feature is placed horizontally
                var statusBarHeight = calculateStatusBarHeight();
                var toolBarHeight = calculateToolbarHeight();

                set.SetMargin(
                    Resource.Id.device_feature, ConstraintSet.Top,
                    rect.Top - statusBarHeight - toolBarHeight
                );
                set.Connect(
                    Resource.Id.layout_change, ConstraintSet.Top,
                    Resource.Id.device_feature, ConstraintSet.Bottom, 0
                );
            }

            //Set the view to visible and apply constraints
            set.SetVisibility(Resource.Id.device_feature, (int)SystemUiFlags.Visible); // public static final int VISIBLE = 0x00000000;
            set.ApplyTo(constraintLayout);
        }

        int calculateToolbarHeight()
        {
            var typedValue = new TypedValue();
            if (Theme.ResolveAttribute(Android.Resource.Attribute.ActionBarSize, typedValue, true))
            {
                return TypedValue.ComplexToDimensionPixelSize(typedValue.Data, Resources.DisplayMetrics);
            }
            else
            {
                return 0;
            }
        }

        int calculateStatusBarHeight()
        {
            var rect = new Rect();
            Window.DecorView.GetWindowVisibleDisplayFrame(rect);
            return rect.Top;
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

        public void Accept(Java.Lang.Object newLayoutInfo)  // Object will be WindowLayoutInfo
        {
            Log.Info(TAG, "===LayoutStateChangeCallback.Accept");
            Log.Info(TAG, newLayoutInfo.ToString());
            printLayoutStateChange(newLayoutInfo as WindowLayoutInfo);
        }
        #endregion
    }
}