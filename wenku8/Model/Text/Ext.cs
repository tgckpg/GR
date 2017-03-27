using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace wenku8.Model.Text
{
	static class Ext
	{
		private static Regex TrimDef = new Regex( @"[\s\p{P}]+", RegexOptions.IgnoreCase );

		public static string[] ToHashTags( this string Text )
		{
			return TrimDef.Replace( Text, " " ).Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );
		}

		public static string TrimForSearch( this string Text )
		{
			return TrimDef.Replace( Text, " " ).Trim();
		}

	}
}