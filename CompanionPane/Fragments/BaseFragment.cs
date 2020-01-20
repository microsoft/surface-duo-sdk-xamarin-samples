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

namespace CompanionPane.Fragments
{
	public class BaseFragment : Fragment
	{
		internal IOnItemSelectedListener listener;

		public interface IOnItemSelectedListener
		{
			void OnItemSelected(int position);
		}

		public void RegisterOnItemSelectedListener(IOnItemSelectedListener listener)
		{
			this.listener = listener;
		}
	}
}