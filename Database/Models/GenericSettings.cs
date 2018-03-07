using GR.Database.Schema;
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

		[AutoNow( SqliteTriggers.INSERT | SqliteTriggers.UPDATE )]
		public DateTime DateModified { get; set; }

		public bool GetBool( bool Default = false )
		{
			if ( Value == null ) return Default;
			return ( Value == "1" );
		}

		public void SetBool( bool v )
		{
			Type = GSDataType.BOOL;
			Value = v ? "1" : "0";
		}
	}
}