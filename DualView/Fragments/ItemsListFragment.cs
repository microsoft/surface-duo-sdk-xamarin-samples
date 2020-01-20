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

namespace DualView.Fragments
{

	public class ItemsListFragment : Fragment, ListView.IOnItemClickListener
	{
		private ArrayAdapter<Item> adapterItems;
		private ListView lvItems;
		private List<Item> items;
		private IOnItemSelectedListener listener;

		public interface IOnItemSelectedListener
		{
			void OnItemSelected(Item i, int position);
		}

		public void RegisterOnItemSelectedListener(IOnItemSelectedListener l)
		{
			listener = l;
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			var items = Item.Items;

			if (Activity != null)
			{
				adapterItems = new ArrayAdapter<Item>(Activity,
						Android.Resource.Layout.SimpleListItemActivated1, items);
			}
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.fragment_items_list, container, false);
			lvItems = view.FindViewById<ListView>(Resource.Id.lvItems);
			lvItems.Adapter = adapterItems;
			lvItems.ChoiceMode = ChoiceMode.Single;
			lvItems.OnItemClickListener = this;

			return view;
		}

		public void SetSelectedItem(int position)
		{
			lvItems.SetItemChecked(position, true);
			listener.OnItemSelected(items[position], position);
		}

		public void OnItemClick(AdapterView adapterView, View item, int position, long rowId)
		{
			Item i = adapterItems.GetItem(position);
			listener.OnItemSelected(i, position);
		}

		public static ItemsListFragment NewInstance(List<Item> items)
		{
			ItemsListFragment fragment = new ItemsListFragment();
			fragment.items = items;
			return fragment;
		}
	}
}