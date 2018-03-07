using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GR.Database.Models
{
	class FTSChapter
	{
		[Key]
		public int ChapterId { get; set; }
		public string Text { get; set; }
	}
}