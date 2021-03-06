﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GR.CompositeElement
{
	using GSystem;

	sealed class CompatMenuFlyoutItem : MenuFlyoutItem
	{
		public static readonly DependencyProperty Icon2Property = DependencyProperty.Register(
			"Icon2", typeof( IconElement ), typeof( CompatMenuFlyoutItem )
			, new PropertyMetadata( null, OnIcon2Changed ) );

		public IconElement Icon2
		{
			get { return ( IconElement ) ( Utils.APIv4 ? GetValue( IconProperty ) : GetValue( Icon2Property ) ); }
			set { SetValue( Utils.APIv4 ? IconProperty : Icon2Property, value ); }
		}

		private ContentPresenter Icon2Presenter;

		public CompatMenuFlyoutItem()
			: base()
		{
			if ( !Utils.APIv4 )
			{
				DefaultStyleKey = typeof( CompatMenuFlyoutItem );
			}
		}

		private static void OnIcon2Changed( DependencyObject d, DependencyPropertyChangedEventArgs e ) => ( ( CompatMenuFlyoutItem ) d ).UpdateIcon();

		private void UpdateIcon()
		{
			if ( Utils.APIv4 )
			{
				Icon = Icon2;
			}
			else if ( Icon2Presenter != null )
			{
				Icon2Presenter.Content = Icon2;
			}
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if ( !Utils.APIv4 )
			{
				Icon2Presenter = ( ContentPresenter ) GetTemplateChild( "Icon2" );
				if ( Icon2 != null )
				{
					Icon2Presenter.Content = Icon2;
				}
			}
		}

	}
}