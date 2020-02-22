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
	public class AdaptiveFragment  : Fragment
	{
        public const string KEY_LAYOUT_ID = "layoutId";

        DragSourceFragment dragSourceFragment;
        DropTargetFragment dropTargetFragment;

        public static AdaptiveFragment NewInstance()
            => new AdaptiveFragment();

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            dragSourceFragment = DragSourceFragment.NewInstance();
            dropTargetFragment = DropTargetFragment.NewInstance();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var bundle = this.Arguments;
            int layoutId = bundle != null ?
                    this.Arguments.GetInt(KEY_LAYOUT_ID, Resource.Layout.fragment_single_portrait)
                    : Resource.Layout.fragment_single_portrait;
            var view = inflater.Inflate(layoutId, container, false);

            ChildFragmentManager.BeginTransaction()
                    .Add(Resource.Id.drag_source_container, dragSourceFragment)
                    .Add(Resource.Id.drop_target_container, dropTargetFragment)
                    .Commit();
            return view;
        }
    }
}