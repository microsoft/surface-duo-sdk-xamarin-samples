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
	public abstract class BaseFragment : Fragment
	{
		public interface IOnItemSelectedListener
		{
			void OnItemSelected(int position);
		}

		internal IOnItemSelectedListener listener;

		public void RegisterOnItemSelectedListener(IOnItemSelectedListener listener)
		{
			this.listener = listener;
		}

		public abstract bool OnBackPressed();

		public abstract void SetCurrentSelectedPosition(int position);
	}
}