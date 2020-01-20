using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;
using AndroidX.ViewPager.Widget;

namespace CompanionPane.Fragments
{
	public class DualLandscape : BaseFragment, ViewPager.IOnPageChangeListener, ContextFragment.OnItemSelectedListener
	{
		List<Slide> slides;
		ViewPager viewPager;
		ContextFragment contextFragment;
		int currentPosition;

		public static DualLandscape NewInstance(List<Slide> slides)
		{
			var dualLandscape = new DualLandscape();
			dualLandscape.slides = slides;
			dualLandscape.currentPosition = 0;
			dualLandscape.contextFragment = ContextFragment.NewInstance(slides);
			return dualLandscape;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View view = inflater.Inflate(Resource.Layout.fragment_dual_landscape, container, false);
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

		public void setCurrentPosition(int position)
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
			if (this.listener != null)
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