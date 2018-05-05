using GR.Database.Schema;
using System;
using System.ComponentModel.DataAnnotations;

namespace GR.Database.Models
{
	enum GSDataType { UNKNOWN, BOOL, USHORT, INT, UINT, DOUBLE, STRING, COLOR, FLOAT }

	abstract class GenericSettings
	{
		[Key]
		public string Key { get; set; }
		public GSDataType Type { get; set; }
		public string Value { get; set; }

		[AutoNow( SqliteTriggers.INSERT | SqliteTriggers.UPDATE )]
		public DateTime DateModified { get; set; }

		public object GetValue()
		{
			switch ( Type )
			{
				case GSDataType.BOOL:
					return Value == "1";
				case GSDataType.INT:
					if ( int.TryParse( Value, out int IntValue ) )
						return IntValue;
					break;
				case GSDataType.USHORT:
					if ( ushort.TryParse( Value, out ushort uValue0 ) )
						return uValue0;
					break;
				case GSDataType.UINT:
					if ( uint.TryParse( Value, out uint uValue1 ) )
						return uValue1;
					break;
				case GSDataType.DOUBLE:
					if ( double.TryParse( Value, out double dValue ) )
						return dValue;
					break;
				case GSDataType.FLOAT:
					if ( float.TryParse( Value, out float fValue ) )
						return fValue;
					break;
				case GSDataType.STRING:
					return Value;
				case GSDataType.COLOR:
					return GSystem.ThemeManager.StringColor( Value );
			}

			return Value;
		}

		public void SetValue( object Val )
		{
			if ( Val == null )
			{
				Value = null;
				return;
			}

			if ( Val is string || Type == GSDataType.STRING )
			{
				Type = GSDataType.STRING;
				Value = ( string ) Val;
			}
			else if ( Val is bool || Type == GSDataType.BOOL )
			{
				Type = GSDataType.BOOL;
				Value = ( ( bool ) Val ) ? "1" : "0";
			}
			else if ( Val is ushort || Type == GSDataType.USHORT )
			{
				Type = GSDataType.USHORT;
				Value = Val.ToString();
			}
			else if ( Val is int || Type == GSDataType.INT )
			{
				Type = GSDataType.INT;
				Value = Val.ToString();
			}
			else if ( Val is uint || Type == GSDataType.UINT )
			{
				Type = GSDataType.UINT;
				Value = Val.ToString();
			}
			else if ( Val is float || Type == GSDataType.FLOAT )
			{
				Type = GSDataType.FLOAT;
				Value = Val.ToString();
			}
			else if ( Val is double || Type == GSDataType.DOUBLE )
			{
				Type = GSDataType.DOUBLE;
				Value = Val.ToString();
			}
			else if ( Val is Windows.UI.Color || Type == GSDataType.COLOR )
			{
				Type = GSDataType.COLOR;
				Value = Val.ToString();
			}
			else
			{
				throw new InvalidOperationException( "Unknow data type" );
			}
		}

	}
}