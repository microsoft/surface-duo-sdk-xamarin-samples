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

using Fragment = AndroidX.Fragment.App.Fragment;

namespace DragAndDrop
{
	public class DragSourceFragment : Fragment, View.IOnLongClickListener
	{
        public static DragSourceFragment NewInstance()
            => new DragSourceFragment();


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.drag_source_layout, container, false);

            var dragTextView = view.FindViewById<TextView>(Resource.Id.drag_text_view);
            var dragImageView = view.FindViewById<ImageView>(Resource.Id.drag_image_view);

            dragTextView.Tag = new Java.Lang.String("text_view");
            dragImageView.Tag = new Java.Lang.String("image_view");

            dragTextView.SetOnLongClickListener(this);
            dragImageView.SetOnLongClickListener(this);

            return view;
        }

        public bool OnLongClick(View v)
        {
            var item = new ClipData.Item(v.Tag.JavaCast<Java.Lang.String>());
            var mimeTypes = new String[1];

            if (v is ImageView) {
                mimeTypes[0] = "image/jpeg";
            } else if (v is TextView) {
                mimeTypes[0] = ClipDescription.MimetypeTextPlain;
            }

            var data = new ClipData(v.Tag.JavaCast<Java.Lang.String>().ToString(), mimeTypes, item);
            
            var dragShadowBuilder = new View.DragShadowBuilder(v);
            v.StartDragAndDrop(data, dragShadowBuilder, v, 0);
            
            return true;
        }
    }
}