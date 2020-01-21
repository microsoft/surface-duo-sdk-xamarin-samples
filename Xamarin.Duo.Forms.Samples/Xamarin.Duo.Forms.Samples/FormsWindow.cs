using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace Xamarin.Duo.Forms.Samples
{
    public class FormsWindow : INotifyPropertyChanged
    {
        ILayoutService LayoutService => DependencyService.Get<ILayoutService>();
        IHingeService HingeService => DependencyService.Get<IHingeService>();

        private Rectangle hinge;
        private Rectangle toolbar;
        private Rectangle leftPane;
        private Rectangle rightPane;
        private bool isSpanned;
        private bool isLandscape;
        private bool isPortrait;
        private Rectangle containerArea;
        private Page _mainPage;

        public FormsWindow(ContentPage contentPage)
        {
            _mainPage = contentPage;
            _mainPage.LayoutChanged += OnMainPageLayoutChanged;
            _mainPage.Appearing += OnPageAppearing;
            _mainPage.Disappearing += OnPageDisappearing;

            LayoutService.LayoutGuideChanged += OnLayoutGuideChanged;
            HingeService.OnHingeUpdated += OnHingeUpdated;
        }

        void OnMainPageLayoutChanged(object sender, EventArgs e)
        {
            UpdateLayouts();
        }

        void OnHingeUpdated(object sender, HingeEventArgs e)
        {
            UpdateLayouts();
        }

        void OnPageDisappearing(object sender, EventArgs e)
        {
            LayoutService.LayoutGuideChanged -= OnLayoutGuideChanged;
            HingeService.OnHingeUpdated -= OnHingeUpdated;
            _mainPage.LayoutChanged -= OnMainPageLayoutChanged;
        }

        void OnPageAppearing(object sender, EventArgs e)
        {
            LayoutService.LayoutGuideChanged -= OnLayoutGuideChanged;
            LayoutService.LayoutGuideChanged += OnLayoutGuideChanged;

            HingeService.OnHingeUpdated -= OnHingeUpdated; 
            HingeService.OnHingeUpdated += OnHingeUpdated;

            _mainPage.LayoutChanged -= OnMainPageLayoutChanged;
            _mainPage.LayoutChanged += OnMainPageLayoutChanged;
        }

        Page GetDisplayedPage(Page rootPage)
        {
            if (rootPage == null)
                return null;

            if (rootPage is Shell shell)
            {
                if (shell?.CurrentItem?.CurrentItem is IShellSectionController shellSectionController)
                    return shellSectionController.PresentedPage;

                return null;
            }

            if (rootPage is ContentPage contentPage)
            {
                return contentPage;
            }

            if (rootPage is TabbedPage tabbedPage)
            {
                return tabbedPage.CurrentPage;
            }

            if (rootPage is NavigationPage navigationPage)
            {
                return navigationPage.CurrentPage;
            }

            if (rootPage is MasterDetailPage mdp)
            {
                return GetDisplayedPage(mdp.Detail);
            }

            throw new Exception($"Unaccounted for Page Type {rootPage}");
        }

        Rectangle GetContainerArea(Page rootPage)
        {
            Rectangle returnValue = Rectangle.Zero;
            if (rootPage is ContentPage contentPage)
            {
                var bounds = contentPage.Bounds;
                returnValue = new Rectangle(0, 0, contentPage.Width, contentPage.Height);
            }

            if (returnValue == Rectangle.Zero)
                returnValue = (rootPage as IPageController).ContainerArea;

            if (returnValue == Rectangle.Zero)
            {
                var displayedPage = GetDisplayedPage(rootPage);

                if (displayedPage != null)
                {
                    returnValue = (displayedPage as IPageController).ContainerArea;
                    if (returnValue == Rectangle.Zero && displayedPage.Width > 0)
                    {
                        returnValue = new Rectangle(0, 0, displayedPage.Width, displayedPage.Height);
                    }
                }
            }

            return returnValue;
        }

        void UpdateLayouts()
        {
            var displayedPage = GetDisplayedPage(_mainPage);

            if (displayedPage == null)
                return;


            var containerArea = GetContainerArea(_mainPage);

            if (containerArea.Width <= 0)
            {
                return;
            }


            if (!HingeService.IsLandscape)
            {
                if (HingeService.IsSpanned)
                {
                    var paneWidth = (containerArea.Width - Hinge.Width) / 2;
                    Pane1 = new Rectangle(0, 0, paneWidth, containerArea.Height);
                    Pane2 = new Rectangle(paneWidth + Hinge.Width, 0, paneWidth, Pane1.Height);
                }
                else
                {
                    Pane1 = new Rectangle(0, 0, containerArea.Width, containerArea.Height);
                    Pane2 = Rectangle.Zero;
                }
            }
            else
            {
                var displayedScreenAbsCoordinates = LayoutService.GetLocationOnScreen(displayedPage) ?? Point.Zero;
                if (HingeService.IsSpanned)
                {
                    var screenSize = Device.info.ScaledScreenSize;
                    var topStuffHeight = displayedScreenAbsCoordinates.Y;
                    var bottomStuffHeight = screenSize.Height - topStuffHeight - containerArea.Height;
                    var paneWidth = containerArea.Width;
                    var leftPaneHeight = Hinge.Y - topStuffHeight;
                    var rightPaneHeight = screenSize.Height - topStuffHeight - leftPaneHeight - bottomStuffHeight - Hinge.Height;

                    Pane1 = new Rectangle(0, 0, paneWidth, leftPaneHeight);
                    Pane2 = new Rectangle(0, Hinge.Y + Hinge.Height - topStuffHeight, paneWidth, rightPaneHeight);
                }
                else
                {
                    Pane1 = new Rectangle(0, 0, containerArea.Width, containerArea.Height);
                    Pane2 = Rectangle.Zero;
                }
            }

            IsSpanned = HingeService.IsSpanned;
            IsPortrait = !HingeService.IsLandscape;
            IsLandscape = HingeService.IsLandscape;
            ContainerArea = containerArea;
        }

        private void OnLayoutGuideChanged(object sender, LayoutGuideChangedArgs e)
        {
            if (e.LayoutGuide.Name == "Hinge")
                Hinge = e.LayoutGuide.Rectangle;
            else if (e.LayoutGuide.Name == "Toolbar")
                Toolbar = e.LayoutGuide.Rectangle;
        }

        public bool IsPortrait
        {
            get
            {
                return isPortrait;
            }
            set
            {
                SetProperty(ref isPortrait, value);
            }
        }

        public bool IsLandscape
        {
            get
            {
                return isLandscape;
            }
            set
            {
                SetProperty(ref isLandscape, value);
            }
        }

        public bool IsSpanned
        {
            get
            {
                return isSpanned;
            }
            set
            {
                SetProperty(ref isSpanned, value);
            }
        }

        public Rectangle Pane1
        {
            get
            {
                return leftPane;
            }
            set
            {
                SetProperty(ref leftPane, value);
            }
        }

        public Rectangle ContainerArea
        {
            get
            {
                return containerArea;
            }
            set
            {
                SetProperty(ref containerArea, value);
            }
        }

        public Rectangle Pane2
        {
            get
            {
                return rightPane;
            }
            set
            {
                SetProperty(ref rightPane, value);
            }
        }

        public Rectangle Hinge
        {
            get
            {
                return hinge;
            }
            set
            {
                SetProperty(ref hinge, value);
            }
        }

        public Rectangle Toolbar
        {
            get => toolbar;
            set
            {
                SetProperty(ref toolbar, value);
            }
        }



        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
