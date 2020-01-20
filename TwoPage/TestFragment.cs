using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;

namespace TwoPage
{
	public class TestFragment : Fragment
	{
		public string Text { get; private set; }

		public static TestFragment NewInstance(string text)
		{
			var testFragment = new TestFragment();
			var bundle = new Bundle();
			bundle.PutString("text", text);
			testFragment.Arguments = bundle;
			return testFragment;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.fragment_layout, container, false);
			var mTextView = view.FindViewById<TextView>(Resource.Id.text_view);
			if (Arguments != null)
			{
				Text = Arguments.GetString("text");
				mTextView.Text = Text;
			}
			return view;
		}

		public override string ToString()
			=> Text;

		// Init fragments for ViewPager
		public static List<TestFragment> Fragments
			=> Enumerable.Range(0, 9).Select(i => TestFragment.NewInstance($"Page {i}")).ToList();
	}
}