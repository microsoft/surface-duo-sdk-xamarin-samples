using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;

namespace IntentToSecondScreen
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            FindViewById<Button>(Resource.Id.second_activity_button)
                .Click += (s, e) => StartIntentSecondActivity();

            FindViewById<Button>(Resource.Id.link_button)
                .Click += (s, e) => StartIntentBrowserApp(Resources.GetString(Resource.String.web_link));

            FindViewById<Button>(Resource.Id.map_button)
                .Click += (s, e) => StartIntentBrowserApp(Resources.GetString(Resource.String.map_link));

        }

        void StartIntentSecondActivity()
        {
            var intent = new Intent(this, typeof(SecondActivity));
            // Intent.FLAG_ACTIVITY_LAUNCH_ADJACENT is required to launch a second activity
            // on a second display while still keeping the first activity on the first display
            // (not pausing/stopping it)
            intent.AddFlags(ActivityFlags.LaunchAdjacent | ActivityFlags.NewTask);
            StartActivity(intent);
    }

        void StartIntentBrowserApp(string url)
        {
            var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(url));
            intent.AddFlags(ActivityFlags.LaunchAdjacent | ActivityFlags.NewTask);
            StartActivity(intent);
        }
    }
}