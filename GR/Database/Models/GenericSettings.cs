using System;
using System.ComponentModel.DataAnnotations;

namespace GR.Database.Models
{
	enum GSDataType { BOOL, INT, STRING, COLOR }

	abstract class GenericSettings
	{
		[Key]
		public string Key { get; set; }
		public GSDataType Type { get; set; }
		public string Value { get; set; }
		public DateTime DateModified { get; set; }

		public bool GetBool( string Key, bool Default = false )
		{
			if ( Value == null ) return Default;
			return ( Value == "1" );
		}
	}
}