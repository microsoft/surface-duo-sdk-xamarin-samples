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
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.App;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace DragAndDrop
{
	public class DropTargetFragment : Fragment, View.IOnDragListener
	{

        RelativeLayout imageDropContainer;
        RelativeLayout textDropContainer;
        ConstraintLayout imageHintContainer;
        ConstraintLayout textHintContainer;

        public static DropTargetFragment NewInstance()
            => new DropTargetFragment();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.drop_target_layout, container, false);

            imageHintContainer = view.FindViewById<ConstraintLayout>(Resource.Id.drop_image_hint);
            textHintContainer = view.FindViewById<ConstraintLayout>(Resource.Id.drop_text_hint);

            imageDropContainer = view.FindViewById<RelativeLayout>(Resource.Id.drop_image_container);
            textDropContainer = view.FindViewById<RelativeLayout>(Resource.Id.drop_text_container);
            imageDropContainer.SetOnDragListener(this);
            textDropContainer.SetOnDragListener(this);

            return view;
        }

        public bool OnDrag(View v, DragEvent e)
        {
            var action = e.Action;
            var mimeType = string.Empty;

            if (e.ClipDescription != null)
                mimeType = e.ClipDescription.GetMimeType(0);

            switch (action) {
                case DragAction.Started:
                    if (string.IsNullOrEmpty(mimeType))
                        return false;

                    if (IsImage(mimeType) || IsText(mimeType)) {
                        SetBackgroundColor(mimeType);
                        return true;
                    }

                    return false;
                case DragAction.Entered:
                    SetBackgroundColor(mimeType);
                    return true;

                case DragAction.Drop:
                    if (IsText(mimeType)) {
                        HandleTextDrop(e);
                        v.Elevation = 1;
                    } else if (IsImage(mimeType)) {
                        HandleImageDrop(e);
                        v.Elevation = 1;
                    }
                    ClearBackgroundColor(mimeType);
                    return true;

                case DragAction.Ended:
                    ClearBackgroundColor();
                    return true;

                case DragAction.Location:
                case DragAction.Exited:
                    // Ignore events
                    return true;
                default:
                    break;
            }
            return false;
        }

        void SetBackgroundColor(string mimeType)
        {
            var colorFilter = new PorterDuffColorFilter(Color.Gray, PorterDuff.Mode.SrcIn);
            if (IsImage(mimeType))
            {
                imageHintContainer.Background.SetColorFilter(colorFilter);
                imageHintContainer.Elevation = 4;
                imageHintContainer.Invalidate();
            }
            else if (IsText(mimeType))
            {
                textHintContainer.Background.SetColorFilter(colorFilter);
                textHintContainer.Elevation = 4;
                textHintContainer.Invalidate();
            }
        }

        void ClearBackgroundColor(string mimeType)
        {
            if (IsImage(mimeType))
            {
                imageHintContainer.Background.ClearColorFilter();
                imageHintContainer.Elevation = 0;
                imageHintContainer.Invalidate();
            }
            else if (IsText(mimeType))
            {
                textHintContainer.Background.ClearColorFilter();
                textHintContainer.Elevation = 0;
                textHintContainer.Invalidate();
            }
        }

        void ClearBackgroundColor()
        {
            imageHintContainer.Background.ClearColorFilter();
            imageHintContainer.Elevation = 0;
            imageHintContainer.Invalidate();
            textHintContainer.Background.ClearColorFilter();
            textHintContainer.Elevation = 0;
            textHintContainer.Invalidate();
        }

        bool IsImage(String mime)
            => mime.StartsWith("image/");

        bool IsText(String mime)
            => mime.StartsWith("text/");

        void HandleTextDrop(DragEvent e)
        {
            var item = e.ClipData.GetItemAt(0);
            var dragData = item.Text.ToString();
            var vw = e.LocalState.JavaCast<View>();
            //Remove the local text view, vw is null if drop from another app
            if (vw != null)
            {
                var owner = (ViewGroup)vw.Parent;
                owner.RemoveView(vw);
            }
            else
            {
                var textView = new TextView(this.Context);
                textView.Text = dragData;
                vw = textView;
            }

            textDropContainer.RemoveAllViews();
            textDropContainer.AddView(vw);
            vw.Visibility = ViewStates.Visible;
        }

        void HandleImageDrop(DragEvent e)
        {
            var item = e.ClipData.GetItemAt(0);
            var vw = e.LocalState.JavaCast<View>();
            //Remove the local image view, vw is null if drop from another app
            if (vw != null)
            {
                var owner = (ViewGroup)vw.Parent;
                owner.RemoveView(vw);
            }
            else
            {
                ImageView imageView = new ImageView(Context);
                var uri = item.Uri;
                if (ContentResolver.SchemeContent.Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
                {
                    // Accessing a "content" scheme Uri requires a permission grant.
                    var dropPermissions = ActivityCompat.RequestDragAndDropPermissions(this.Activity, e);

                    if (dropPermissions == null)
                    {
                        // Permission could not be obtained.
                        return;
                    }

                    imageView.SetImageURI(uri);
                }
                else
                {
                    // Other schemes (such as "android.resource") do not require a permission grant.
                    imageView.SetImageURI(uri);
                }

                vw = imageView;
            }

            imageDropContainer.RemoveAllViews();
            imageDropContainer.AddView(vw);
            vw.Visibility = ViewStates.Visible;
        }

	}
}