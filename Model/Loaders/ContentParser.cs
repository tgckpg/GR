using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.Logging;

namespace GR.Model.Loaders
{
	using Database.Models;
	using Resources;
	using Settings;
	using Text;

	sealed class ContentParser
	{
		public static readonly string ID = typeof( ContentParser ).Name;

		public Task ParseAsync( string e, Chapter C )
		{
			return Task.Run( () => Parse( e, C ) );
		}

		/// <summary>
		/// Content Preprocessing
		/// </summary>
		/// <param name="e"> Content </param>
		/// <param name="path"> Save Location </param>
		/// <param name="illspath"> Illustraction Location </param>
		private void Parse( string e, Chapter C )
		{
			if( C.Content == null )
			{
				Shared.BooksDb.LoadRef( C, b => b.Content );
			}

			/** WARNING: DO NOT MODIFY THIS LOGIC AS IT WILL MESS WITH THE BOOK ANCHOR **/
			try
			{
				string content = "";
				string[] paragraphs = e.Split( new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries );
				// Filter unecessary line break and spaces
				string s;
				foreach ( string p in paragraphs )
				{
					s = p.Trim();
					if ( !string.IsNullOrEmpty( s ) )
						content += s + '\n';
					/*
					int k = LengthOfSpaces( p );
					// This skip the space-only line
					if ( k != p.Length )
						content += p.Substring( LengthOfSpaces( p ) ) + '\n';
					*/
				}
				paragraphs = null;

				ExtractImages( ref content, C );

				if ( C.Content == null )
					C.Content = new ChapterContent();

				C.Content.Data.BytesValue = Manipulation.PatchSyntax( Encoding.UTF8.GetBytes( content ) );
				Shared.BooksDb.SaveChanges();
			}
			catch ( Exception ex )
			{
				Logger.Log( ID, ex.Message, LogType.ERROR );
			}
		}

		private void ExtractImages( ref string content, Chapter C )
		{
			const string token = "<!--image-->";
			const int tokenl = 12;

			if( C.Image == null )
			{
				Shared.BooksDb.LoadRef( C, b => b.Image );
			}

			ChapterImage ills = C.Image ?? new ChapterImage();

			int i = content.IndexOf( token );

			if ( i == -1 ) return;

			int nIllus = 0;
			string Replaced = 0 < i ? content.Substring( 0, i ) : "";
			int j = content.IndexOf( token, i + tokenl );

			while ( !( i == -1 || j == -1 ) )
			{
				ills.Urls.Add( content.Substring( i + tokenl, j - i - tokenl ) );

				i = content.IndexOf( token, j + tokenl );

				string ImgFlag = "\n" + AppKeys.ANO_IMG + nIllus.ToString() + "\n";

				if ( i == -1 )
				{
					Replaced += ImgFlag + content.Substring( j + tokenl );
					break;
				}
				else
				{
					Replaced += ImgFlag + content.Substring( j + tokenl, i - j - tokenl );
				}

				j = content.IndexOf( token, i + tokenl );

				nIllus++;
			}

			content = Replaced;

			if( C.Image != ills )
				C.Image = ills;
		}
	}
}