﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Model.Book
{
	using Database.Models;
	using Resources;

	partial class BookItem
	{
		public static readonly string ID = typeof( BookItem ).Name;

		protected Book _Entry;
		public Book Entry
		{
			get
			{
				if ( _Entry == null )
				{
					throw new InvalidOperationException( "_Entry is not initialized" );
				}

				return _Entry;
			}
		}

		public string GID
		{
			get { return string.Format( "{0}.{1}.{2}", Entry.ZoneId, Entry.Type, Entry.ZItemId ); }
		}

		public int Id
		{
			get { return Entry.Id; }
			set { Entry.Id = value; }
		}

		public string ZoneId
		{
			get { return Entry.ZoneId; }
			set { Entry.ZoneId = value; }
		}

		public string ZItemId
		{
			get { return Entry.ZItemId; }
			set { Entry.ZItemId = value; }
		}

		public BookType Type
		{
			get { return Entry.Type; }
			set { Entry.Type = value; }
		}

		public string Title
		{
			get { return Entry.Title; }
			set { Entry.Title = value; NotifyChanged( "Title" ); }
		}

		public string Description
		{
			get { return Entry.Description; }
			set { Entry.Description = value; NotifyChanged( "Description" ); }
		}

		public bool IsFav
		{
			get { return Entry.Fav; }
			set
			{
				Entry.Fav = value;
				NotifyChanged( "IsFav" );
			}
		}

		public DateTime LastCache
		{
			get { return Entry.DateModified; }
			set { Entry.DateModified = value; NotifyChanged( "LastCache" ); }
		}

		public BookInfo Info => Entry.Info;
		public List<Volume> Volumes => Entry.Volumes;
		public HashSet<string> Others => Info.Others;

		protected BookItem( Book Bk )
		{
			if ( Shared.BooksDb.Entry( Bk ).State == Microsoft.EntityFrameworkCore.EntityState.Detached )
			{
				if ( Bk.Id == 0 ) throw new InvalidOperationException( "Only tracked Entry is allowed" );
			}

			_Entry = Bk;
		}

		protected BookItem( string ZoneId, BookType SrcType, string ItemId )
		{
			_Entry = Shared.BooksDb.GetBook( ZoneId, ItemId, SrcType );
		}
	}
}