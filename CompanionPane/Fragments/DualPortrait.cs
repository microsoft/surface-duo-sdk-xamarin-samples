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
using AndroidX.ViewPager.Widget;

namespace CompanionPane.Fragments
{
	public class DualPortrait : BaseFragment, ViewPager.IOnPageChangeListener, ContextFragment.OnItemSelectedListener
	{
		List<Slide> slides;
		ViewPager viewPager;
		ContextFragment contextFragment;
		int currentPosition;

		public static DualPortrait NewInstance(List<Slide> slides)
		{
			var dualPortrait = new DualPortrait();
			dualPortrait.slides = slides;
			dualPortrait.currentPosition = 0;
			dualPortrait.contextFragment = ContextFragment.NewInstance(slides);
			return dualPortrait;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.fragment_dual_portrait, container, false);
			viewPager = view.FindViewById<ViewPager>(Resource.Id.pager);
			contextFragment.AddOnItemSelectedListener(this);
			ShowFragment(contextFragment);
			SetupViewPager();
			return view;
		}

		public override void OnResume()
		{
			base.OnResume();
			SetCurrentPosition();
		}

		public override void OnHiddenChanged(bool hidden)
		{
			base.OnHiddenChanged(hidden);
			if (!hidden)
				SetCurrentPosition();
		}

		void ShowFragment(Fragment fragment)
		{
			var fragmentManager = ChildFragmentManager;
			var fragmentTransaction = fragmentManager.BeginTransaction();
			if (!fragment.IsAdded)
				fragmentTransaction.Add(Resource.Id.all_slides, fragment);
			fragmentTransaction.Commit();
		}

		void SetupViewPager()
		{
			var slideFragments = SlideFragment.GetFragments(slides);
			var pagerAdapter = new Adapters.PagerAdapter(ChildFragmentManager, slideFragments);
			viewPager.Adapter = pagerAdapter;
			viewPager.SetCurrentItem(currentPosition, false);
			viewPager.AddOnPageChangeListener(this);
		}

		void SetCurrentPosition()
		{
			contextFragment.SetCurrentItem(currentPosition);
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
			contextFragment.SetCurrentItem(position);
			currentPosition = position;
			if (listener != null)
				listener.OnItemSelected(position);
		}

		public void OnItemSelected(int position)
		{
			viewPager.CurrentItem = position;
			currentPosition = position;
			if (listener != null)
				listener.OnItemSelected(position);
		}
	}

}