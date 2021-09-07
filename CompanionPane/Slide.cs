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

namespace CompanionPane
{
	public class Slide
	{
		public string Title { get; private set; }
		public string Content { get; private set; }

		private Slide(string title, string content)
		{
			Title = title;
			Content = content;
		}

		public static IEnumerable<Slide> Slides =>
			Enumerable.Range(0, 9)
			.Select(i => new Slide($"Slide {i + 1}", $"Slide content {i + 1}"));
	}
}