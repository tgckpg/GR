using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.Linq;
using Net.Astropenguin.Logging;
using Net.Astropenguin.Messaging;

namespace GR.Model.Book
{
	using Database.Models;
	using Loaders;
	using GSystem;
	using Settings;

	sealed class LocalTextDocument : BookItem
	{
		new public static readonly string ID = typeof( LocalTextDocument ).Name;

		public bool IsValid { get; private set; }

		public LocalTextDocument( Book Bk ) : base( Bk ) { }
		public LocalTextDocument( string id ) : base( AppKeys.ZLOCAL, BookType.L, id ) { }

		private List<TextEpisode> Episodes;

		public async static Task<LocalTextDocument> ParseAsync( string ZItemId, string Doc )
		{
			try
			{
				LocalTextDocument TDoc = new LocalTextDocument( ZItemId );
				TDoc.Episodes = new List<TextEpisode>();

				string[] lines = Doc.Split( new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries );

				int l = lines.Length - 2;

				MessageBus.Send( typeof( ListItem.LocalBook ), "Verifying ...", ZItemId );
				// Filter unecessary line break and spaces
				string s;
				TDoc.Title = lines[ 1 ];
				TextEpisode Ep = null;
				for( int i = 2; i < l;  i ++ )
				{
					s = lines[ i ];
					if ( 3 < s.Length )
					{
						if ( !( s[ 0 ] == ' ' && s[ 1 ] == ' ' && s[ 2 ] == ' ' && s[ 3 ] == ' ' ) )
						{
							if ( Ep != null )
							{
								TDoc.Episodes.Add( Ep );
							}
							Ep = new TextEpisode() { Title = s.Trim(), Content = "" };
							continue;
						}
					}

					if ( Ep == null ) throw new Exception( "Invalid leading paragraph" );

					s = s.Trim();
					if ( !string.IsNullOrEmpty( s ) )
					{
						Ep.Content += s + "\n";
					}
				}

				MessageBus.Send( typeof( ListItem.LocalBook ), "Analyzing ...", ZItemId );
				await TDoc.GuessVolTitle();

				return TDoc;
			}
			catch ( Exception ex )
			{
				Logger.Log( ID, ex.Message, LogType.ERROR );
			}

			throw new FormatException();
		}

		public async Task Save()
		{
			SaveInfo();
			foreach( TextEpisode Ep in Episodes )
			{
				MessageBus.Send( typeof( ListItem.LocalBook ), "Saving: " + Ep.Ch.Title, ZItemId );
				await new ContentParser().ParseAsync( Ep.Content, Ep.Ch );
			}

			Episodes.Clear();
		}

		private async Task GuessVolTitle()
		{
			Logger.Log( ID, "Guessing Volumes for: " + Title, LogType.DEBUG );

			if ( Entry.Volumes == null )
			{
				Entry.Volumes = await Resources.Shared.BooksDb.LoadCollection( Entry, x => x.Volumes, x => x.Index );
			}

			Entry.Volumes.Clear();

			IEnumerable<TextEpisode> OEps = Episodes.Where( x => true );
			while ( 0 < OEps.Count() )
			{
				TextEpisode First = OEps.First();
				int i = 0; int j = 0; int Taken = 0;
				IEnumerable<TextEpisode> Eps = OEps.Where( x => true );
				while ( true )
				{
					char RefS = Eps.First( x => i < x.Title.Length ).Title[ i ];

					int LongestTitle = 0;
					foreach ( TextEpisode Ep in Eps )
					{
						if ( LongestTitle < Ep.Title.Length )
						{
							LongestTitle = Ep.Title.Length;
						}

						if ( Ep.Title.Length <= i ) continue;

						if ( !( i < Ep.Title.Length ) || RefS != Ep.Title[ i ] )
							break;
						j++;
					}

					if ( 1 < j && i < LongestTitle )
					{
						Eps = Eps.Take( Taken = j );
						i++;
					}
					else
					{
						if ( 0 < Taken )
						{
							Volumes.Add( ProcessVolume( Eps, i ) );
							OEps = OEps.Skip( Eps.Count() );
						}
						else
						{
							Volumes.Add( ProcessVolume( OEps.Take( 1 ), i ) );
							OEps = OEps.Skip( 1 );
						}
						Taken = 0;
						break;
					}
					j = 0;
				}

				Eps = OEps.Where( x => true );

				await Task.Delay( 100 );
			}
		}

		private Volume ProcessVolume( IEnumerable<TextEpisode> Chapters, int VSplice )
		{
			string VolTitle = Chapters.First().Title;

			if ( 0 < VSplice )
			{
				VolTitle = VolTitle.Substring( 0, VSplice );
			}

			VolTitle = VolTitle.Trim();

			Logger.Log( ID, "Guess this is a Volume: " + VolTitle, LogType.DEBUG );
			MessageBus.SendUI( typeof( ListItem.LocalBook ), VolTitle, ZItemId );

			Volume Vol = new Volume()
			{
				Title = VolTitle,
				Book = Entry
			};

			Vol.Meta[ AppKeys.GLOBAL_VID ] = Utils.Md5( VolTitle );
			Vol.Chapters = Chapters.Remap( ( x, i ) =>
			{
				x.Ch = new Chapter()
				{
					Book = Entry,
					Volume = Vol,
					Title = x.Title.Substring( VSplice ).Trim(),
					Index = i
				};
				x.Ch.Meta[ AppKeys.GLOBAL_CID ] = Utils.Md5( x.Ch.Title );
				return x.Ch;
			} ).ToList();

			return Vol;
		}

		private class TextEpisode
		{
			public string Title;
			public string Content;
			public Chapter Ch;
		}
	}
}