using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;

namespace MasterDetail
{
	public class Item
	{
		public string Title { get; private set; }
		public string Body { get; private set; }

		public Item(string title, string body)
		{
			this.Title = title;
			this.Body = body;
		}

		//Init items for ListView
		public static List<Item> Items
			=> new List<Item>
			{
				new Item("Item 1", "This is the first item"),
				new Item("Item 2", "This is the second item"),
				new Item("Item 3", "This is the third item")
			};

		public override string ToString()
			=> Title;
	}
}