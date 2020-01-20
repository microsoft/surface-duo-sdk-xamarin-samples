using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.ViewPager.Widget;

namespace CompanionPane.Fragments
{
	public class SinglePortrait : BaseFragment, ViewPager.IOnPageChangeListener
	{
		ViewPager viewPager;
		List<SlideFragment> fragments;
		int currentPosition;

		public static SinglePortrait NewInstance(List<Slide> slides)
		{
			SinglePortrait singlePortrait = new SinglePortrait();
			singlePortrait.fragments = SlideFragment.GetFragments(slides);
			singlePortrait.currentPosition = 0;
			return singlePortrait;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.fragment_single_portrait, container, false);
			viewPager = view.FindViewById<ViewPager>(Resource.Id.pager);
			var pagerAdapter = new Adapters.PagerAdapter(ChildFragmentManager, fragments);
			viewPager.Adapter = pagerAdapter;
			viewPager.SetCurrentItem(currentPosition, false);
			viewPager.AddOnPageChangeListener(this);
			return view;
		}

		public override void OnHiddenChanged(bool hidden)
		{
			base.OnHiddenChanged(hidden);
			if (!hidden)
				viewPager.SetCurrentItem(currentPosition, false);
		}

		public void SetCurrentPosition(int position)
		{
			currentPosition = position;
		}

		public void OnPageScrollStateChanged(int state)
		{
		}

		public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
		{
		}

		public void OnPageSelected(int position)
		{
			currentPosition = position;
			if (listener != null)
				listener.OnItemSelected(position);
		}
	}
}