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

        public static readonly BindableProperty LandscapeBehaviorProperty =
            BindableProperty.Create(nameof(LandscapeBehavior), typeof(DuoSplitPaneBehavior), typeof(TwoPaneView), DuoSplitPaneBehavior.ShowBoth);

        public static readonly BindableProperty PortraitBehaviorProperty =
            BindableProperty.Create(nameof(PortraitBehavior), typeof(DuoSplitPaneBehavior), typeof(TwoPaneView), DuoSplitPaneBehavior.ShowBoth);


        public DuoSplitPaneBehavior LandscapeBehavior
        {
            get => (DuoSplitPaneBehavior)GetValue(LandscapeBehaviorProperty);
            set
            {
                SetValue(LandscapeBehaviorProperty, value);
                InvalidateLayout();
            }
        }

        public DuoSplitPaneBehavior PortraitBehavior
        {
            get => (DuoSplitPaneBehavior)GetValue(PortraitBehaviorProperty);
            set
            {
                SetValue(PortraitBehaviorProperty, value);
                InvalidateLayout();
            }
        }

        public View Left
            => Children?.FirstOrDefault();

        public View Right
            => Children?.Skip(1)?.FirstOrDefault();

        public bool IsDualView
            => Left.IsVisible && Right.IsVisible;

        public bool IsLandscape
            => ScreenViewModel.IsLandscape;

        public bool IsPortrait
            => !IsLandscape;

        public bool IsSpanned
            => ScreenViewModel.IsSpanned;

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            var left = Left;
            var right = Right;

            if (left == null)
                return;

            var screenViewModel = ScreenViewModel;
            var lefTPane = screenViewModel.Pane1;
            var rightPane = screenViewModel.Pane2;

            Rectangle leftViewRect = Rectangle.Zero;
            Rectangle rightViewRect = Rectangle.Zero;

            DuoSplitPaneBehavior duoSplitPaneBehavior;

            if (screenViewModel.IsPortrait)
                duoSplitPaneBehavior = PortraitBehavior;
            else
                duoSplitPaneBehavior = LandscapeBehavior;

            if (!screenViewModel.IsSpanned)
            {
                leftViewRect = lefTPane;
                rightViewRect = rightPane;
                if(right != null)
                    right.IsVisible = false;
                left.IsVisible = true;
            }
            else if (duoSplitPaneBehavior == DuoSplitPaneBehavior.ShowOnlyLeft)
            {
                if (right != null)
                    right.IsVisible = false;
                left.IsVisible = true;
                rightViewRect = screenViewModel.ContainerArea;
            }
            else if (duoSplitPaneBehavior == DuoSplitPaneBehavior.ShowOnlyRight)
            {
                if (right != null)
                    right.IsVisible = true;
                left.IsVisible = false;
                leftViewRect = screenViewModel.ContainerArea;
            }
            else
            {
                leftViewRect = lefTPane;
                rightViewRect = rightPane;
                if (right != null)
                    right.IsVisible = screenViewModel.IsSpanned;
                left.IsVisible = true;
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
