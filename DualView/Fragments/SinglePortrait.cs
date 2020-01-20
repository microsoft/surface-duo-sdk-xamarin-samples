using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;

namespace DualView.Fragments
{

	public class SinglePortrait : BaseFragment, ItemsListFragment.IOnItemSelectedListener
	{
		int currentSelectedPosition = 0;
		ItemsListFragment itemListFragment;
		List<Item> items;

		public static SinglePortrait NewInstance(List<Item> items)
		{
			SinglePortrait singlePortrait = new SinglePortrait();
			singlePortrait.items = items;
			return singlePortrait;
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			itemListFragment = ItemsListFragment.NewInstance(items);
			itemListFragment.RegisterOnItemSelectedListener(this);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.fragment_single_portrait, container, false);
			ShowFragment(itemListFragment);
			return view;
		}

		public override void OnHiddenChanged(bool hidden)
		{
			base.OnHiddenChanged(hidden);

			if (!hidden)
			{
				// Current view is detail view
				if (ChildFragmentManager.BackStackEntryCount == 2)
				{
					ShowBackOnActionBar(true);
					OnBackPressed();
				}
				itemListFragment.SetSelectedItem(currentSelectedPosition);
			}
		}

		void ShowFragment(Fragment fragment)
		{
			var fragmentTransaction = ChildFragmentManager.BeginTransaction();
			var showFragment = ChildFragmentManager.FindFragmentById(Resource.Id.master_single);
			if (showFragment == null)
				fragmentTransaction.Add(Resource.Id.master_single, fragment);
			else
				fragmentTransaction.Hide(showFragment).Add(Resource.Id.master_single, fragment);
			fragmentTransaction.AddToBackStack(fragment.Class.Name);
			fragmentTransaction.Commit();
		}

		public void OnItemSelected(Item item, int position)
		{
			if (listener != null)
				listener.OnItemSelected(position);

			currentSelectedPosition = position;
			ShowBackOnActionBar(true);
			ShowFragment(ItemDetailFragment.NewInstance(item));
		}

		void ShowBackOnActionBar(bool show)
		{
			if (Activity != null)
			{
				var actionbar = ((AppCompatActivity)Activity).SupportActionBar;
				if (actionbar != null)
				{
					actionbar.SetDisplayHomeAsUpEnabled(show);
					actionbar.SetHomeButtonEnabled(show);
				}
			}
		}

		public override bool OnBackPressed()
		{
			// Current view is listview
			if (ChildFragmentManager.BackStackEntryCount == 1)
			{
				return true;
			}
			else
			{
				ChildFragmentManager.PopBackStack();
				ChildFragmentManager.ExecutePendingTransactions();
				// Do not show back on the actionbar when current fragment is ItemsListFragment
				var showFragment = ChildFragmentManager.FindFragmentById(Resource.Id.master_single);
				try
				{
					if (showFragment.JavaCast<ItemsListFragment>() != null)
						ShowBackOnActionBar(false);
				}
				catch { }
			}
			return false;
		}

		public override void SetCurrentSelectedPosition(int position)
		{
			if (currentSelectedPosition != position)
				currentSelectedPosition = position;
		}
	}
}