using GR.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GR.Database.Schema
{
	public static class ModelExt 
	{
		public static void CopyTo( this Book Source, object Target )
		{
			PropertyInfo[] SrcMbInfo = Source.GetType().GetProperties();

			Type TargetMbType = Target.GetType();

			foreach( PropertyInfo SrcInfo in SrcMbInfo )
			{
				TargetMbType.GetProperty( SrcInfo.Name ).SetValue( Target, SrcInfo.GetValue( Source ) );
			}
		}

		public static void CopyFrom( this Book Target, object Source )
		{
			PropertyInfo[] TargetMbInfo = Target.GetType().GetProperties();

			Type SrcMbType = Source.GetType();

			foreach( PropertyInfo TargetInfo in TargetMbInfo )
			{
				TargetInfo.SetValue( Target, SrcMbType.GetProperty( TargetInfo.Name ).GetValue( Source ) );
			}
		}

	}
}