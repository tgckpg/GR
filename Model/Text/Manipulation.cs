using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using libtranslate;

namespace GR.Model.Text
{
	using Config;
	class Manipulation
	{
		public static bool DoSyntaxPatch = Properties.MISC_TEXT_PATCH_SYNTAX;
		public static string PatchSyntax( string s )
		{
			if ( DoSyntaxPatch )
			{
				return Symbolic.PatchSyntax( s );
			}

			return s;
		}

		public static byte[] PatchSyntax( byte[] s )
		{
			if ( DoSyntaxPatch )
			{
				return Symbolic.PatchSyntax( s );
			}

			return s;
		}
	}
}