using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Net.Astropenguin.DataModel;

namespace wenku8.Settings.Theme
{

	class ColorPickerSection : ActiveData
	{
		private IEnumerable<Palette> _palettes;

		public IEnumerable<Palette> Palettes
		{
			get
			{
				return _palettes;
			}
			private set
			{
				_palettes = value;
			}
		}

		public ColorItem CColor { get; set; }

		public ColorPickerSection( ColorItem Target )
		{
			CColor = Target;
			GeneratePalettes( CColor );
		}

		protected void GeneratePalettes( ColorItem C )
		{
			ObservableCollection<Palette> GPalettes = new ObservableCollection<Palette>();

			GPalettes.Add( new Palette( "Current", C.TColor, PaletteSize.Large ) );
			GPalettes.Add( L( +10, new Palette( "+1", C.TColor, PaletteSize.Medium ) ) );
			GPalettes.Add( L( -10, new Palette( "-1", C.TColor, PaletteSize.Medium ) ) );
			GPalettes.Add( L( +20, new Palette( "+2", C.TColor, PaletteSize.Small ) ) );
			GPalettes.Add( L( +40, new Palette( "+4", C.TColor, PaletteSize.Small ) ) );
			GPalettes.Add( L( -20, new Palette( "-2", C.TColor, PaletteSize.Small ) ) );
			GPalettes.Add( L( -40, new Palette( "-4", C.TColor, PaletteSize.Small ) ) );
			GPalettes.Add( L( +30, new Palette( "+3", C.TColor, PaletteSize.Small ) ) );
			GPalettes.Add( L( +50, new Palette( "+5", C.TColor, PaletteSize.Small ) ) );
			GPalettes.Add( L( -30, new Palette( "-3", C.TColor, PaletteSize.Small ) ) );
			GPalettes.Add( L( -50, new Palette( "-5", C.TColor, PaletteSize.Small ) ) );

			Palettes = GPalettes;
			NotifyChanged( "Palettes" );
		}

		private Palette L( int v, Palette palette )
		{
			double dL = v / 100.0;

			if( v < 0 )
			{
				palette.L = ( int ) Math.Floor( palette.L + palette.L * dL );
			}
			else if( 0 < v )
			{
				palette.L = ( int ) Math.Floor( palette.L + ( 100.0 - palette.L ) * dL );
			}

			return palette;
		}
	}
}
