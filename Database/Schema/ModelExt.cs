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
		private static void _EachProperty<TEntity, T>( TEntity Entity, Func<T, T> Exec )
		{
			PropertyInfo[] SrcMbInfo = Entity.GetType().GetProperties();

			foreach ( PropertyInfo SrcInfo in SrcMbInfo )
			{
				if ( SrcInfo.PropertyType is T )
				{
					SrcInfo.SetValue( Entity, Exec( ( T ) SrcInfo.GetValue( Entity ) ) );
				}
			}
		}

		public static void EachProperty<T>( this Book Source, Func<T, T> Exec ) => _EachProperty( Source, Exec );
		public static void EachProperty<T>( this BookInfo Source, Func<T, T> Exec ) => _EachProperty( Source, Exec );
	}
}