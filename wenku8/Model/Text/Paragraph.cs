using System;
using System.ComponentModel;

using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

using Net.Astropenguin.DataModel;

namespace wenku8.Model.Text
{
	using Config;

	class Paragraph : ActiveData, IDisposable
	{
		private const int HORZ_SPC = 5;
		// So many paragrahs, it will be dumb for new-ing all of them
		private static bool Horizontal = false;
		private static double Fts = Properties.APPEARANCE_CONTENTREADER_FONTSIZE;
		private static double ls = Properties.APPEARANCE_CONTENTREADER_LINEHEIGHT;
		private static FontWeight fw = Properties.APPEARANCE_CONTENTREADER_FONTWEIGHT;
		private static Thickness ps = new Thickness( Properties.APPEARANCE_CONTENTREADER_PARAGRAPHSPACING, 0, Properties.APPEARANCE_CONTENTREADER_PARAGRAPHSPACING, 0 );
		private static SolidColorBrush cr = new SolidColorBrush( Properties.APPEARANCE_CONTENTREADER_FONTCOLOR );
		private static SolidColorBrush ab = new SolidColorBrush( new Windows.UI.Color() { A = 0 } );

		// Non reusable properties 
		private string s;
		// Overidde properties
		// Dynamic anchor colors
		private SolidColorBrush abo;
		// Tint Color
		private SolidColorBrush cro;

		public string Text { get { return s; } }

		public double LineHeight
		{
			get { return ls; }
			set { ls = value; NotifyChanged( "LineHeight" ); }
		}
		public double FontSize
		{
			get { return Fts; }
			set
			{
				Fts = value;
				// Suspend the Update
				// NotifyChanged( "FontSize" );
			}
		}
		public FontWeight FontWeight
		{
			get { return fw; }
			set { fw = value; NotifyChanged( "FontWeight" ); }
		}
		public Thickness ParagraphSpacing
		{
			get { return ps; }
			set { ps = value; NotifyChanged( "ParagraphSpacing" ); }
		}

		public SolidColorBrush FontColor { get { return cro == null ? cr : cro; } set { cro = value; NotifyChanged( "FontColor" ); } }
		public SolidColorBrush AnchorColor { get { return abo == null ? ab : abo; } set { abo = value; NotifyChanged( "AnchorColor" ); } }

		public static void SetHorizontal( bool IsHorz )
		{
			Horizontal = IsHorz;
			ps = Horizontal
				? new Thickness( Properties.APPEARANCE_CONTENTREADER_PARAGRAPHSPACING, 0, Properties.APPEARANCE_CONTENTREADER_PARAGRAPHSPACING, 0 )
				: new Thickness( HORZ_SPC, Properties.APPEARANCE_CONTENTREADER_PARAGRAPHSPACING, HORZ_SPC, Properties.APPEARANCE_CONTENTREADER_PARAGRAPHSPACING )
				;
		}

		public void SetParagraphSpacing( double value )
		{
			ParagraphSpacing = Horizontal
				? new Thickness( value = 0.5 * value, 0, value, 0 )
				: new Thickness( HORZ_SPC, value = 0.5 * value, HORZ_SPC, value )
				;
		}

		void AppPropertyChanged( object sender, PropertyChangedEventArgs e )
		{
			switch ( e.PropertyName )
			{
				case Parameters.APPEARANCE_CONTENTREADER_FONTSIZE:
					FontSize = Properties.APPEARANCE_CONTENTREADER_FONTSIZE;
					break;
				case Parameters.APPEARANCE_CONTENTREADER_LINEHEIGHT:
					LineHeight = Properties.APPEARANCE_CONTENTREADER_LINEHEIGHT;
					break;
				case Parameters.APPEARANCE_CONTENTREADER_PARAGRAPHSPACING:
					SetHorizontal( Horizontal );
					NotifyChanged( "ParagraphSpacing" );
					break;
				case Parameters.APPEARANCE_CONTENTREADER_FONTWEIGHT:
					FontWeight = Properties.APPEARANCE_CONTENTREADER_FONTWEIGHT;
					break;
				case Parameters.APPEARANCE_CONTENTREADER_FONTCOLOR:
					cr.Color = Properties.APPEARANCE_CONTENTREADER_FONTCOLOR;
					cro = null;
					NotifyChanged( "FontColor" );
					break;
			}
		}

		public Paragraph( string Text )
		{
			s = Text;
			AppSettings.PropertyChanged += AppPropertyChanged;
		}

		virtual public void Dispose()
		{
			try
			{
				AppSettings.PropertyChanged -= AppPropertyChanged;
			}
			catch ( Exception ) { }
		}

		~Paragraph() { Dispose(); }
		
	}
}