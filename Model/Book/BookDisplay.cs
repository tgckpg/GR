using GR.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Model.Book
{
	using Database.Models;

	public class BookDisplay
	{
		public Book Entry { get; private set; }

		public BookDisplay( Book Entry )
		{
			this.Entry = Entry;
		}

		public string Zone => Entry.ZoneId;
		public string Intro => Entry.Info.LongDescription ?? Entry.Description;
		public string LastAccess => Entry.LastAccess.ToString();

		public override bool Equals( object obj ) => Entry.Equals( ( obj as BookDisplay )?.Entry );
		public override int GetHashCode() => Entry.GetHashCode();

		public object Payload { get; set; }
	}
}