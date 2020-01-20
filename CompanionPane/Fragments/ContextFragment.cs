using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.Gestures;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using CompanionPane.Adapters;

namespace CompanionPane.Fragments
{
	public class ContextFragment : Fragment
	{
		List<Slide> slides;
		RecyclerView recyclerView;
		RecyclerViewAdapter recyclerViewAdapter;
		OnItemSelectedListener listener;
		int prevSelectedPosition = 0;
		GestureDetector gestureDetector;

		public interface OnItemSelectedListener
		{
			void OnItemSelected(int position);
		}

		public static ContextFragment NewInstance(List<Slide> slides)
		{
			var contextFragment = new ContextFragment();
			contextFragment.slides = slides;
			return contextFragment;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View view = inflater.Inflate(Resource.Layout.fragment_context_layout, container, false);
			recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recycler_view);

			recyclerViewAdapter = new RecyclerViewAdapter(recyclerView.Context, slides);
			recyclerView.SetAdapter(recyclerViewAdapter);
			recyclerView.SetLayoutManager(new StaggeredGridLayoutManager(1, OrientationHelper.Vertical));

			gestureDetector = new GestureDetector(Activity, new MySimpleOnGestureListener(recyclerView, listener));

			recyclerView.AddOnItemTouchListener(new MyRvItemTouchListener(gestureDetector));

			return view;
		}

		public void AddOnItemSelectedListener(OnItemSelectedListener listener)
			=> this.listener = listener;

		public void SetCurrentItem(int position)
		{
			if (prevSelectedPosition != position)
			{
				int scrollTo;
				if (prevSelectedPosition - position > 0)
					scrollTo = position - 1;
				else
					scrollTo = position + 1;

				if (scrollTo < 0)
					scrollTo = 0;

				prevSelectedPosition = position;
				recyclerView.SmoothScrollToPosition(scrollTo);
				recyclerViewAdapter.SetCurrentPosition(position);
			}
		}

		public class MyRvItemTouchListener : Java.Lang.Object, RecyclerView.IOnItemTouchListener
		{
			public MyRvItemTouchListener(GestureDetector gestureDetector)
			{
				GestureDetector = gestureDetector;
			}

			public GestureDetector GestureDetector { get; private set; }

			public bool OnInterceptTouchEvent(RecyclerView recyclerView, MotionEvent @event)
				=> GestureDetector.OnTouchEvent(@event);

			public void OnRequestDisallowInterceptTouchEvent(bool disallow)
			{
			}

			public void OnTouchEvent(RecyclerView recyclerView, MotionEvent @event)
			{
			}
		}

		public class MySimpleOnGestureListener : GestureDetector.SimpleOnGestureListener
		{
			public MySimpleOnGestureListener(RecyclerView rv, OnItemSelectedListener listener)
			{
				Rv = rv;
				Listener = listener;
			}

			public RecyclerView Rv { get; private set; }
			public OnItemSelectedListener Listener { get; private set; }

			public override bool OnSingleTapUp(MotionEvent e)
			{
				var childView = Rv.FindChildViewUnder(e.GetX(), e.GetY());

				if (childView != null)
				{
					var position = Rv.GetChildLayoutPosition(childView);

					if (Listener != null)
						Listener.OnItemSelected(position);

					return true;
				}
				return base.OnSingleTapUp(e);
			}
		}
	}
}