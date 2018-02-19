using System.ComponentModel;
using System;
using System.Linq;

using Net.Astropenguin.DataModel;
using Net.Astropenguin.Messaging;

namespace GR.Model.Book
{
	using Database.Models;
	using Resources;
	using Settings;
	using Text;

	class ChapterVModel : ActiveData
	{
		public static Messenger ChapterLoaded = new Messenger();

		public Chapter Ch { get; private set; }

		public string Title => Ch.Title;
		public bool IsCached { get; private set; }

		public ChapterVModel( Chapter Ch )
		{
			this.Ch = Ch;
			IsCached = Ch.Content != null || Shared.BooksDb.ChapterContents.Any( x => x.Chapter == Ch );
			ChapterLoaded.AddHandler( this, OnChapterLoaded );
		}

		private void OnChapterLoaded( Message Mesg )
		{
			if ( Mesg.Payload is Chapter C && C == Ch )
			{
				IsCached = true;
				NotifyChanged( "IsCached" );
				ChapterLoaded.RemoveHandler( this, OnChapterLoaded );
			}
		}

		public Paragraph[] GetParagraphs()
		{
			ChapterContent Cont = Ch.Content;
			string[] Lines = Cont.Text.Split( new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries );

			int l = Lines.Length;
			Paragraph[] Paragraphs = new Paragraph[ Lines.Length ];

			if ( Ch.Image != null )
			{
				for ( int i = 0; i < l; i++ )
				{
					string Line = Lines[ i ];
					Paragraph p;

					if ( Line[ 0 ] == AppKeys.ANO_IMG
						&& int.TryParse( Line.Substring( 1 ), out int iIndex ) )
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
			if ( obj is ChapterVModel C )
			{
				return C.Ch == Ch;
			}

			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}