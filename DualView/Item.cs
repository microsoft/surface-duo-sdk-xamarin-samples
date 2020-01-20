using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace DualView
{
	public class Item : Java.Lang.Object, IParcelable
	{
		public const string KEY = "item";

		internal Item(string title, PointF l)
		{
			Title = title;
			Location = l;
		}

		public string Title { get; private set; }

		public PointF Location { get; private set; }

		public int DescribeContents()
			=> 0;

		public override string ToString()
			=> Title;

		public static IParcelableCreator CREATOR => new ItemCreator();

		public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
		{
			dest.WriteString(Title);
			dest.WriteFloat(Location.X);
			dest.WriteFloat(Location.Y);
		}

		public static List<Item> Items
			=> new List<Item>
			{
				new Item("New York", new PointF(40.7128f, -74.0060f)),
				new Item("Seattle", new PointF(47.6062f, -122.3425f)),
				new Item("Palo Alto", new PointF(37.444184f, -122.161059f)),
				new Item("San Francisco", new PointF(37.7542f, -122.4471f))
			};
	}

	public class ItemCreator : Java.Lang.Object, IParcelableCreator
	{
		public Java.Lang.Object CreateFromParcel(Parcel source)
		{
			var title = source.ReadString();
			var x = source.ReadFloat();
			var y = source.ReadFloat();
			var location = new PointF(x, y);
			return new Item(title, location);
		}

		public Java.Lang.Object[] NewArray(int size)
		{
			return new Java.Lang.Object[size];
		}
	}
}