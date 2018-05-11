using System;
using System.Globalization;
using Windows.UI;

namespace GR.Settings.Theme
{
	using Model.Interfaces;
	enum PaletteSize
	{
		Large, Medium, Small
	}

	class Palette : ColorItem, ISpanable
	{
		private PaletteSize Size;
		private const int StdSize = 40;

		public int ColSpan
		{
			get
			{
				return SpanSize();
			}

			set
			{
			}
		}

		public int RowSpan
		{
			get
			{
				return SpanSize();
			}

			set
			{
			}
		}

		public Color TextColor { get; private set; }

		public double Dim 
		{
			get { return SizeDim(); }
		}

		public Palette( string name, Color TColor, PaletteSize C )
			:base( name, TColor )
		{
			Size = C;
			UpdateTextColor();
			PropertyChanged += Palette_PropertyChanged;
		}

		~Palette()
		{
			PropertyChanged -= Palette_PropertyChanged;
		}

		private void Palette_PropertyChanged( object sender, global::System.ComponentModel.PropertyChangedEventArgs e )
		{
			if ( e.PropertyName == "TColor" )
			{
				UpdateTextColor();
			}
		}

		private void UpdateTextColor()
		{
			ColorItem Item = new ColorItem(
				ColorTag
				, new Color()
				{
					A = TColor.A,
					R = TColor.R,
					G = TColor.G,
					B = TColor.B
				}
			);

			TextColor = Item.L < 50
				? Colors.White
				: Colors.Black
				;

			// Blue-Black Range adjustment
			if( Item.R < 100 && Item.G < 100 && 200 < Item.B )
			{
				TextColor = Colors.White;
			}

			// Yellow-White Range adjustment
			else if( 200 < Item.R && 200 < Item.G && Item.B < 100 )
			{
				TextColor = Colors.Black;
			}


		}

		private int SpanSize()
		{
			switch( Size )
			{
				case PaletteSize.Large:
					return 4;
				case PaletteSize.Medium:
					return 2;
				case PaletteSize.Small:
				default:
					return 1;
			}
		}

		private int SizeDim()
		{
			switch( Size )
			{
				case PaletteSize.Large:
					return 4 * StdSize;
				case PaletteSize.Medium:
					return 2 * StdSize;
				case PaletteSize.Small:
				default:
					return StdSize;
			}
		}
	}
}
