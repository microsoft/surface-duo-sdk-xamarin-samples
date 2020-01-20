using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.ViewPager.Widget;
using static AndroidX.ViewPager.Widget.ViewPager;

namespace TwoPage
{
	// VerticalViewPager can be swiped vertically
	public class VerticalViewPager : ViewPager
	{
		public VerticalViewPager(Context context)
			: base(context, null)
		{
		}

		public VerticalViewPager(Context context, IAttributeSet attrs)
			: base(context, attrs)
		{
			Init();
		}

		public override bool CanScrollHorizontally(int direction)
			=> false;

		public override bool CanScrollVertically(int direction)
			=> base.CanScrollHorizontally(direction);

		void Init()
		{
			SetPageTransformer(true, new VerticalPageTransformer());
			OverScrollMode = OverScrollMode.Never;
		}

		public bool onInterceptTouchEvent(MotionEvent ev)
		{
			var toIntercept = base.OnInterceptTouchEvent(FlipXY(ev));
			FlipXY(ev);
			return toIntercept;
		}

		public override bool OnTouchEvent(MotionEvent ev)
		{
			var toHandle = base.OnTouchEvent(FlipXY(ev));
			FlipXY(ev);
			return toHandle;
		}

		MotionEvent FlipXY(MotionEvent ev)
		{
			var width = Width;
			var height = Height;
			var x = (ev.GetY() / height) * width;
			var y = (ev.GetX() / width) * height;
			ev.SetLocation(x, y);
			return ev;
		}

		class VerticalPageTransformer : Java.Lang.Object, IPageTransformer
		{
			public void TransformPage(View view, float position)
			{
				var pageWidth = view.Width;
				var pageHeight = view.Height;
				if (position < -1)
				{
					view.Alpha = 0;
				}
				else if (position <= 1)
				{
					view.Alpha = 1;
					view.TranslationX = pageWidth * -position;
					float yPosition = position * pageHeight;
					view.TranslationY = yPosition;
				}
				else
				{
					view.Alpha = 0;
				}
			}
		}
	}
}