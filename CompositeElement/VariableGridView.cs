using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using System.ComponentModel;

using Net.Astropenguin.Helpers;
using Net.Astropenguin.Logging;

namespace GR.CompositeElement
{
	using Model.Interfaces;

	public class VariableGridView : GridView, INotifyPropertyChanged
	{
		public static readonly string ID = typeof( VariableGridView ).Name;
		public ScrollViewer RefSV;

		public delegate void ScrollViewerViewChangedHandler( object sender, ScrollViewerViewChangedEventArgs e );

		public event PropertyChangedEventHandler PropertyChanged;
		public event ScrollViewerViewChangedHandler ViewChanged;

		public double HorizontalOffset
		{
			get { return RefSV.HorizontalOffset; }
		}

		public double VerticalOffset
		{
			get { return RefSV.VerticalOffset; }
		}

		protected override void PrepareContainerForItemOverride( DependencyObject element, object item )
		{
			var viewModel = item as ISpanable;

			if( viewModel != null )
			{
				element.SetValue( VariableSizedWrapGrid.ColumnSpanProperty, viewModel.ColSpan );
				element.SetValue( VariableSizedWrapGrid.RowSpanProperty, viewModel.RowSpan );
			}

			base.PrepareContainerForItemOverride( element, item );
		}

		protected override Size ArrangeOverride( Size finalSize )
		{
			return base.ArrangeOverride( finalSize );
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			RefSV = this.ChildAt<ScrollViewer>( 1 );

			RefSV.ViewChanged += RefSV_ViewChanged;
		}

		private void RefSV_ViewChanged( object sender, ScrollViewerViewChangedEventArgs e )
		{
			if ( ViewChanged != null )
				ViewChanged( this, e );

			if( PropertyChanged != null )
			{
				PropertyChanged( this, new PropertyChangedEventArgs( "HorizontalOffset" ) );
				PropertyChanged( this, new PropertyChangedEventArgs( "VerticalOffset" ) );
			}
		}

	}
}
