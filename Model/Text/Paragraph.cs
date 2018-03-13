using System;
using System.ComponentModel;

using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

using Net.Astropenguin.DataModel;

namespace GR.Model.Text
{
	using Config;
	using Net.Astropenguin.Messaging;

	public class Paragraph : ActiveData
	{
		private const int HORZ_SPC = 5;
		// So many paragrahs, it will be dumb for new-ing all of them
		private static bool Horizontal = false;
		private static double Fts = GRConfig.ContentReader.FontSize;
		private static double ls = GRConfig.ContentReader.LineHeight;
		private static FontWeight fw = GRConfig.ContentReader.FontWeight;
		private static double _ps = GRConfig.ContentReader.ParagraphSpacing;
		private static Thickness ps = new Thickness( _ps, 0, _ps, 0 );
		private static SolidColorBrush cr = new SolidColorBrush( GRConfig.ContentReader.FontColor );
		private static SolidColorBrush ab = new SolidColorBrush( new Windows.UI.Color() { A = 0 } );

		// Non reusable properties 
		private string s;
		// Override properties
		// Dynamic anchor colors
		private SolidColorBrush abo;
		// Tint Color
		private SolidColorBrush cro;

		public string Text => s;

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

		public SolidColorBrush FontColor { get { return cro ?? cr; } set { cro = value; NotifyChanged( "FontColor" ); } }
		public SolidColorBrush AnchorColor { get { return abo ?? ab; } set { abo = value; NotifyChanged( "AnchorColor" ); } }

		public static void SetHorizontal( bool IsHorz )
		{
			Horizontal = IsHorz;
			ps = Horizontal
				? new Thickness( _ps, 0, _ps, 0 )
				: new Thickness( HORZ_SPC, _ps, HORZ_SPC, _ps )
				;
		}

		public void SetParagraphSpacing( double value )
		{
			ParagraphSpacing = Horizontal
				? new Thickness( value = 0.5 * value, 0, value, 0 )
				: new Thickness( HORZ_SPC, value = 0.5 * value, HORZ_SPC, value )
				;
		}

		private void GRConfigChanged( Message Mesg )
		{
			if ( Mesg.TargetType == typeof( Config.Scopes.ContentReader ) )
			{
				switch ( Mesg.Content )
				{
					case "FontSize":
						FontSize = ( double ) Mesg.Payload;
						break;
					case "LineHeight":
						LineHeight = ( double ) Mesg.Payload;
						break;
					case "ParagraphSpacing":
						_ps = ( double ) Mesg.Payload;
						SetHorizontal( Horizontal );
						NotifyChanged( "ParagraphSpacing" );
						break;
					case "FontWeight":
						FontWeight = new FontWeight() { Weight = ( ushort ) Mesg.Payload };
						break;
					case "FontColor":
						cr.Color = ( Windows.UI.Color ) Mesg.Payload;
						cro = null;
						NotifyChanged( "FontColor" );
						break;
				}
			}
		}

		public Paragraph( string Text )
		{
			s = Text;
			GRConfig.ConfigChanged.AddHandler( this, GRConfigChanged );
		}
	}
}