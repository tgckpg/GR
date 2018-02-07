using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GR.Database.Models
{
	using Schema;

	public enum BookType : byte
	{
		/// <summary>
		/// Book type is Spider
		/// </summary>
		S,
		/// <summary>
		/// Book type is Local
		/// </summary>
		L,
		/// <summary>
		/// Book type is Ex
		/// </summary>
		W,
		/// <summary>
		/// Book type is ExD
		/// </summary>
		WD
	}

	public enum LayoutMethod : byte { VerticalWriting = 1, RightToLeft = 2 }
	public enum AnchorType : byte { BookMark = 1, AutoAnchor = 2 }

	public class Book
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string ZoneId { get; set; }
		public string ZItemId { get; set; }

		[Required]
		public BookType Type { get; set; }

		[Required]
		public string Title { get; set; }
		public string Description { get; set; }

		public bool Fav { get; set; }

		[Required]
		public LayoutMethod TextLayout { get; set; }

		[AutoNow( SqliteTriggers.INSERT | SqliteTriggers.UPDATE )]
		public DateTime DateModified { get; set; }

		public DateTime? LastAccess { get; set; }

		[Required]
		public BookInfo Info { get; set; }

		[NotMapped]
		public DbDictionary Meta { get; set; } = new DbDictionary();
		public string Json_Meta
		{
			get => Meta.Data;
			set => Meta.Data = value;
		}

		public List<Volume> Volumes { get; set; }
	}

	public class Anchor
	{
		[Key]
		public int Id { get; set; }

		public int BookId { get; set; }
		public Book Book { get; set; }

		public AnchorType Type { get; set; }

		public string Ref0 { get; set; }
		public string Ref1 { get; set; }
		public string Ref2 { get; set; }

		[NotMapped]
		public DbDictionary Meta { get; set; } = new DbDictionary();
		public string Json_Meta
		{
			get => Meta.Data;
			set => Meta.Data = value;
		}

		public int Index { get; set; } 
	}

	public class BookInfo
	{
		[Key]
		public int Id { get; set; }

		public int BookId { get; set; }
		public Book Book { get; set; }

		public string LongDescription { get; set; }
		public string DailyHitCount { get; set; }
		public string TotalHitCount { get; set; }
		public string FavCount { get; set; }
		public string PushCount { get; set; }
		public string PostingDate { get; set; }
		public string LastUpdateDate { get; set; }
		public string Author { get; set; }
		public string Press { get; set; }
		public string Status { get; set; }
		public string Length { get; set; }
		public string LatestSection { get; set; }

		[NotMapped]
		public DbHashSet Others { get; set; } = new DbHashSet();
		public string Json_Others
		{
			get => Others.Data;
			set => Others.Data = value;
		}

		[NotMapped]
		public DbHashSet Flags { get; set; } = new DbHashSet();
		public string Json_Flags
		{
			get => Flags.Data;
			set => Flags.Data = value;
		}

		public string OriginalUrl { get; set; }
		public string CoverSrcUrl { get; set; }

		[AutoNow( SqliteTriggers.INSERT | SqliteTriggers.UPDATE )]
		public DateTime DateModified { get; set; }
	}

	public class Volume
	{
		[Key]
		public int Id{ get; set; }

		// EF convension
		[Column( Order = 0 )]
		public int BookId { get; set; }
		public Book Book { get; set; }

		[Required, Column( Order = 1 )]
		public int Index { get; set; }
		public string Title { get; set; }

		public List<Chapter> Chapters { get; set; }

		[NotMapped]
		public DbDictionary Meta { get; set; } = new DbDictionary();
		public string Json_Meta
		{
			get => Meta.Data;
			set => Meta.Data = value;
		}

		[AutoNow( SqliteTriggers.INSERT | SqliteTriggers.UPDATE )]
		public DateTime DateModified { get; set; }
	}

	public class Chapter
	{
		[Key]
		public int Id { get; set; }

		// EF convension
		[Column( Order = 0 )]
		public int BookId { get; set; }
		public Book Book { get; set; }

		// EF convension
		[Column( Order = 1 )]
		public int VolumeId { get; set; }
		public Volume Volume { get; set; }

		[Required, Column( Order = 2 )]
		public int Index { get; set; }
		public string Title { get; set; }

		public ChapterContent Content { get; set; }
		public ChapterImage Image { get; set; }

		[NotMapped]
		public DbDictionary Meta { get; set; } = new DbDictionary();
		public string Json_Meta
		{
			get => Meta.Data;
			set => Meta.Data = value;
		}

		[AutoNow( SqliteTriggers.INSERT | SqliteTriggers.UPDATE )]
		public DateTime DateModified { get; set; }
	}

	public class ChapterContent 
	{
		[Key]
		public int Id { get; set; }

		public int ChapterId { get; set; }
		public Chapter Chapter { get; set; }

		public string Text { get; set; }
	}

	public class ChapterImage
	{
		[Key]
		public int Id { get; set; }

		public int ChapterId { get; set; }
		public Chapter Chapter { get; set; }

		[NotMapped]
		public DbList Urls { get; set; } = new DbList();
		public string Json_Urls
		{
			get => Urls.Data;
			set => Urls.Data = value;
		}
	}
}