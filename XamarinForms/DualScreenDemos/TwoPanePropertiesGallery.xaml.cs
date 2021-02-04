﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.DualScreen;
using Xamarin.Forms.Xaml;

namespace DualScreenDemos
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TwoPanePropertiesGallery : ContentPage
	{
		public TwoPanePropertiesGallery()
		{
			InitializeComponent();
			Pane1Length.ValueChanged += PaneLengthChanged;
			Pane2Length.ValueChanged += PaneLengthChanged;
			PanePriority.ItemsSource = Enum.GetValues(typeof(TwoPaneViewPriority));
			TallModeConfiguration.ItemsSource = Enum.GetValues(typeof(TwoPaneViewTallModeConfiguration));
			WideModeConfiguration.ItemsSource = Enum.GetValues(typeof(TwoPaneViewWideModeConfiguration));
			twoPaneView.PanePriority = TwoPaneViewPriority.Pane1;
			Pane1Length.Value = 0.5;
			Pane2Length.Value = 0.5;
		}

		void PaneLengthChanged(object sender, ValueChangedEventArgs e)
		{
			twoPaneView.Pane1Length = new GridLength(Pane1Length.Value, GridUnitType.Star);
			twoPaneView.Pane2Length = new GridLength(Pane2Length.Value, GridUnitType.Star);
		}

		protected override void OnAppearing()
		{
			PanePriority.SelectedIndex = 0;
			TallModeConfiguration.SelectedIndex = 1;
			WideModeConfiguration.SelectedIndex = 1;

			DualScreenInfo.Current.HingeAngleChanged += OnHingeAngleChanged;
		}

		protected override void OnDisappearing()
		{
			DualScreenInfo.Current.HingeAngleChanged -= OnHingeAngleChanged;
		}

		void OnHingeAngleChanged(object sender, HingeAngleChangedEventArgs e)
		{
			lblHingeAngle.Text = e.HingeAngleInDegrees.ToString();
		}

		void Setup(double width, double height)
		{
			if (width <= 0 || height <= 0)
				return;

			lblScreenDim.Text = $"Screen Dimensions: {width}x{height}";
		}

		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height);
			Setup(width, height);
		}

		void OnReset(object sender, EventArgs e)
		{
			twoPaneView.PanePriority = TwoPaneViewPriority.Pane1;
			Pane1Length.Value = 0.5;
			Pane2Length.Value = 0.5;
		}
	}
}
