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
using CompanionPane.Fragments;

namespace CompanionPane.Adapters
{
	public class PagerAdapter : FragmentPagerAdapter
	{
		public List<SlideFragment> Fragments { get; private set; }

		public PagerAdapter(FragmentManager fm, List<SlideFragment> fragments)
			: base(fm, BehaviorResumeOnlyCurrentFragment)
		{
			Fragments = fragments;
		}

		public override AndroidX.Fragment.App.Fragment GetItem(int position)
			=> Fragments[position];

		public override int Count
			=> Fragments.Count;
	}
}