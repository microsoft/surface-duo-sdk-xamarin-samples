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
using ADragFlags = Android.Views.DragFlags;

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

            dragTextView.Tag = new Java.Lang.String("Text");
            dragImageView.Tag = new Java.Lang.String("Image");

            dragTextView.SetOnLongClickListener(this);
            dragImageView.SetOnLongClickListener(this);

            return view;
        }

        public bool OnLongClick(View v)
        {
            var item = new ClipData.Item(v.Tag.JavaCast<Java.Lang.String>());
            var mimeTypes = new String[1];
            ClipData data = null;

            if (v is ImageView) {
                // image item only drags WITHIN THE APP 
                mimeTypes[0] = "image/jpeg";
                data = new ClipData(v.Tag.JavaCast<Java.Lang.String>().ToString(), mimeTypes, item);
                // TODO: allow image to drag to other apps
            } else if (v is TextView) {
                // use plain text, can drag outside the app
                data = ClipData.NewPlainText(
                    new Java.Lang.String(v.Tag.ToString()),
                    new Java.Lang.String((v as TextView).Text)
                    );
            }

            var dragShadowBuilder = new View.DragShadowBuilder(v);
            // flags required to drag to other apps
            v.StartDragAndDrop(data, dragShadowBuilder, v, (int)ADragFlags.Global | (int)ADragFlags.GlobalUriRead | (int)ADragFlags.GlobalUriWrite);
            
            return true;
        }
    }
}