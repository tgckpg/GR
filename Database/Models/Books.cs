using GR.Database.Schema;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GR.Database.Models
{
	public enum BookType : byte { S, L, W }

	public enum LayoutMethod : byte
	{
		Orientation = 1, TextDirection = 2
	}

	public class Book
	{
		[Key]
		public string Id { get; set; }

		[Required]
		public BookType Type { get; set; }

		[Required]
		public string Title { get; set; }

		public string Description { get; set; }

		[Required]
		public LayoutMethod TextLayout { get; set; }

		[AutoNow( SqliteTriggers.INSERT | SqliteTriggers.UPDATE )]
		public DateTime DateModified { get; set; }

		public BookInfo Info { get; set; }
	}

	public class BookInfo
	{
		[Key, ForeignKey( "Book" )]
		public string BookId { get; set; }

		public string LongDescription { get; set; }
		public string TodayHitCount { get; set; }
		public string TotalHitCount { get; set; }
		public string FavCount { get; set; }
		public string PushCount { get; set; }
		public string RecentUpdate { get; set; }
		public string UpdateStatus { get; set; }
		public string Author { get; set; }
		public string Press { get; set; }
		public string Intro { get; set; }
		public string Status { get; set; }
		public string StatusLong { get; set; }
		public string Length { get; set; }
	}
}