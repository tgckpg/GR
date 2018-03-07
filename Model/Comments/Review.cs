using System;
using System.Globalization;

namespace GR.Model.Comments
{
	class Review : Comment
	{
		public string Id { get; set; }

		public string LastReply
		{
			get
			{
				return TimeStamp.ToString();
			}
			set
			{
				TimeStamp = DateTime.ParseExact( value + "+8", "yyyyMMddHHmmssz", CultureInfo.CurrentUICulture );
			}
		}

		new public string PostTime { get; set; } 

		private int _numreply;

		public string NumReplies
		{
			get { return 0 < _numreply ? "+" + _numreply : ""; }
			set { _numreply = int.Parse( value ); }
		}
	}
}
