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

namespace CompanionPane.Fragments
{
	public class SlideFragment : Fragment
	{
		const string CONTENT = "content";
		string content;

		static SlideFragment NewInstance(Slide slide)
		{
			var testFragment = new SlideFragment();
			var bundle = new Bundle();
			bundle.PutString(CONTENT, slide.Content);
			testFragment.Arguments = bundle;
			return testFragment;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.fragment_slide_layout, container, false);
			var textView = view.FindViewById<TextView>(Resource.Id.text_view);

			if (Arguments != null)
			{
				content = Arguments.GetString(CONTENT);
				textView.Text = content;
			}
			return view;
		}

		public override string ToString()
			=> content;

		// Init fragments for ViewPager
		public static List<SlideFragment> GetFragments(List<Slide> slides)
				=> slides.Select(s => SlideFragment.NewInstance(s)).ToList();
	}
}