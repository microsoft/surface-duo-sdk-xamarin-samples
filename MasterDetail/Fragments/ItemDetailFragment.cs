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
using Newtonsoft.Json;

namespace MasterDetail.Fragments
{
	public class ItemDetailFragment : Fragment
	{
		Item item;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			try
			{
				if (Arguments != null)
					item = JsonConvert.DeserializeObject<Item>(Arguments.GetString("item"));
			}
			catch { }
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.fragment_item_detail, container, false);
			var tvTitle = view.FindViewById<TextView>(Resource.Id.tvTitle);
			var tvBody = view.FindViewById<TextView>(Resource.Id.tvBody);
			var ratingBar = view.FindViewById<RatingBar>(Resource.Id.rating);
			var image = view.FindViewById<ImageView>(Resource.Id.image);

			tvTitle.Text = item?.Title;
			tvBody.Text = item?.Body;
			image.SetImageResource(Resource.Drawable.ic_movie_black_24dp);
			ratingBar.Rating = 2;

			return view;
		}

		internal static ItemDetailFragment NewInstance(Item item)
		{
			var fragment = new ItemDetailFragment();
			var args = new Bundle();
			args.PutString("item", JsonConvert.SerializeObject(item ?? new Item(string.Empty, string.Empty)));
			fragment.Arguments = args;
			return fragment;
		}
	}
}