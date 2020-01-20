using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.CardView.Widget;
using AndroidX.RecyclerView.Widget;

namespace CompanionPane.Adapters
{
	class RecyclerViewAdapter : RecyclerView.Adapter
	{
		LayoutInflater layoutInflater;

		List<Slide> slides;

		int currentPosition;

		public RecyclerViewAdapter(Context context, List<Slide> slides)
		{
			layoutInflater = LayoutInflater.From(context);
			this.slides = slides;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
			=> new SlideViewHolder(layoutInflater.Inflate(Resource.Layout.item_slide, parent, false));

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			var h = holder.JavaCast<SlideViewHolder>();

			h.Content.Text = slides[position].Content;
			h.Title.Text = slides[position].Title;

			h.SetSelected(currentPosition == position);
		}

		public void SetCurrentPosition(int position)
		{
			currentPosition = position;
			NotifyDataSetChanged();
		}

		public override int ItemCount
			=> slides.Count;

		public class SlideViewHolder : RecyclerView.ViewHolder
		{
			public TextView Content { get; private set; }
			public TextView Title { get; private set; }
			public CardView CardView { get; private set; }

			public SlideViewHolder(View view)
				: base(view)
			{
				Title = view.FindViewById<TextView>(Resource.Id.slide_title);
				Content = view.FindViewById<TextView>(Resource.Id.slide_content);
				CardView = view.FindViewById<CardView>(Resource.Id.card_view);
			}

			public void SetSelected(bool selected)
				=> CardView.Selected = selected;
		}
	}
}