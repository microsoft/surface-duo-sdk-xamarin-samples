using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Duo.Forms.Samples;
using Xamarin.Forms;

namespace Xamarin.Duo.Forms.Samples
{
    public enum DuoSplitPaneBehavior
    {
        ShowOnlyLeft,
        ShowOnlyRight,
        ShowBoth,
    }

    public class TwoPaneView : Layout<View>
    {
        ContentPage _contentPage;
        FormsWindow _ScreenViewModel;

        public TwoPaneView() : base()
        {
            this.VerticalOptions = LayoutOptions.FillAndExpand;
            this.HorizontalOptions = LayoutOptions.FillAndExpand;
        }

        FormsWindow ScreenViewModel
        {
            get
            {
                ContentPage parentPage = null;

                var parent = this.Parent;

                while (parentPage == null && parent != null)
                {
                    parentPage = parent as ContentPage;
                    parent = parent?.Parent;
                }

                if(_contentPage != parentPage && parentPage != null)
                {
                    if(_ScreenViewModel != null)
                        _ScreenViewModel.PropertyChanged -= OnScreenViewModelChanged;

                    _ScreenViewModel = new FormsWindow(parentPage);
                    _contentPage = parentPage;
                    _ScreenViewModel.PropertyChanged += OnScreenViewModelChanged;
                }

                return _ScreenViewModel;
            }
        }

        private void OnScreenViewModelChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            InvalidateLayout();
        }

        public View Pane1
            => Children?.FirstOrDefault();

        public View Pane2
            => Children?.Skip(1)?.FirstOrDefault();

        public bool IsDualView
            => Pane1.IsVisible && Pane2.IsVisible;

        public bool IsLandscape
            => ScreenViewModel.IsLandscape;

        public bool IsPortrait
            => !IsLandscape;

        public bool IsSpanned
            => ScreenViewModel.IsSpanned;

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            var left = Pane1;
            var right = Pane2;

            if (left == null)
                return;

            var formsWindows = ScreenViewModel;
            var pane1 = formsWindows.Pane1;
            var pane2 = formsWindows.Pane2;

            Rectangle leftViewRect = Rectangle.Zero;
            Rectangle rightViewRect = Rectangle.Zero;

            if (!formsWindows.IsSpanned)
            {
                leftViewRect = pane1;
                rightViewRect = pane2;
                if(right != null)
                    right.IsVisible = false;

                left.IsVisible = true;
            }
            else if (formsWindows.IsPortrait)
            {
                if (right != null)
                    right.IsVisible = true;

                leftViewRect = pane1;
                rightViewRect = pane2;
            }
            else
            {
                if (right != null)
                    right.IsVisible = false;

                left.IsVisible = true;
                leftViewRect = formsWindows.ContainerArea;
            }

            if (left.IsVisible)
                LayoutChildIntoBoundingRegion(left, leftViewRect);

            if (right != null && right.IsVisible)
                LayoutChildIntoBoundingRegion(right, rightViewRect);

            OnPropertyChanged(nameof(IsLandscape));
            OnPropertyChanged(nameof(IsPortrait));
            OnPropertyChanged(nameof(IsDualView));
            OnPropertyChanged(nameof(IsSpanned));
        }

    }
}
