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
	public class PagerAdapter : FragmentPagerAdapter
	{
		List<TestFragment> fragments;

		public bool ShowTwoPages { get; set; } = false;

		public PagerAdapter(FragmentManager fm, List<TestFragment> fragments)
			: base(fm, BehaviorResumeOnlyCurrentFragment)
		{
			this.fragments = fragments;
		}

		public override Fragment GetItem(int position)
			=> fragments[position];

		public override int Count
			=> fragments.Count;

		// 0.5f : Each pages occupy full space
		// 1.0f : Each pages occupy half space
		public override float GetPageWidth(int position)
			=> ShowTwoPages ? 0.5f : 1.0f;
	}
}