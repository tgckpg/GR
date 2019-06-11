using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GR.Data
{
	using Database.Schema;

	public static class Ext
	{
		public static string AsBase64ZString( this XElement Element )
		{
			using ( MemoryStream s = new MemoryStream() )
			{
				Element.Save( s, SaveOptions.DisableFormatting );
				s.Position = 0;

				ZData ZVal = new ZData();
				ZVal.WriteStream( s );
				return ZVal.GetBase64Raw();
			}
		}

		public static string FromBase64ZString( this string DataString )
		{
			ZData ZVal = new ZData();
			ZVal.SetBase64Raw( DataString );
			return ZVal.StringValue;
		}
	}
}