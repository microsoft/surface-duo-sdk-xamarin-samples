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

namespace MasterDetail.Fragments
{
	public class ItemsListFragment : Fragment, ListView.IOnItemClickListener
	{
		ArrayAdapter<Item> adapterItems;
		ListView listView;
		List<Item> items;

		IOnItemSelectedListener listener;

		public interface IOnItemSelectedListener
		{
			void OnItemSelected(Item i, int position);
		}

		public void RegisterOnItemSelectedListener(IOnItemSelectedListener onItemSelectedListener)
		{
			listener = onItemSelectedListener;
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			var items = Item.Items;

			if (Activity != null)
				adapterItems = new ArrayAdapter<Item>(Activity, Android.Resource.Layout.SimpleListItemActivated1, items);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.fragment_items_list, container, false);
			listView = view.FindViewById<ListView>(Resource.Id.list_view);
			listView.Adapter = adapterItems;
			listView.ChoiceMode = ChoiceMode.Single;
			listView.OnItemClickListener = this;

			return view;
		}

		public void SetSelectedItem(int position)
		{
			listView.SetItemChecked(position, true);
			listener.OnItemSelected(items[position], position);
		}

		public static ItemsListFragment NewInstance(List<Item> items)
		{
			var fragment = new ItemsListFragment();
			fragment.items = items;
			return fragment;
		}

		public void OnItemClick(AdapterView parent, View view, int position, long id)
		{
			var i = adapterItems.GetItem(position);
			listener.OnItemSelected(i, position);
		}
	}
}