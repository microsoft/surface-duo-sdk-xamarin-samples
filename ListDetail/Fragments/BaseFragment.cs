﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;

namespace ListDetail.Fragments
{
	public abstract class BaseFragment : Fragment
	{
		public abstract bool OnBackPressed();

		public abstract int GetCurrentSelectedPosition();

		public abstract void SetCurrentSelectedPosition(int position);
	}
}