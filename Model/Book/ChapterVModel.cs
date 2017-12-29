using System.ComponentModel;
using System;

using Net.Astropenguin.DataModel;

namespace GR.Model.Book
{
	using Database.Models;
	using Settings;
	using Text;

	class ChapterVModel : ActiveData
	{
		public Chapter Ch { get; private set; }

		public bool IsCached;
		public bool HasIllustrations;

		public ChapterVModel( Chapter Ch )
		{
			this.Ch = Ch;
		}

		public Paragraph[] GetParagraphs()
		{
			ChapterContent Cont = Ch.Content;
			string[] Lines = Cont.Text.Split( new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries );

			int l = Lines.Length;
			Paragraph[] Paragraphs = new Paragraph[ Lines.Length ];

			if ( Ch.Image != null )
			{
				int iIndex;
				for ( int i = 0; i < l; i++ )
				{
					string Line = Lines[ i ];
					Paragraph p;

					if ( Line[ 0 ] == AppKeys.ANO_IMG
						&& int.TryParse( Line.Substring( 1 ), out iIndex ) )
					{
						p = new IllusPara( Ch.Image.Urls[ iIndex ] );
					}
					else
					{
						p = new Paragraph( Lines[ i ] );
					}

					Paragraphs[ i ] = p;
				}
			}
			else
			{
				for ( int i = 0; i < l; i++ )
					Paragraphs[ i ] = new Paragraph( Lines[ i ] );
			}

			return Paragraphs;
		}

		public void UpdateStatus()
		{
			NotifyChanged( "IsCached", "HasIllustrations" );
		}

		public override bool Equals( object obj )
		{
			ChapterVModel C = obj as ChapterVModel;
			if ( C != null )
			{
				return C.Ch == this.Ch;
			}

			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}