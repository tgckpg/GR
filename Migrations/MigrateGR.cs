using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Text;
using GR.Config;
using GR.Database.Contexts;
using GR.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.Migrations
{
	class MigrateGR
	{
		public static void Start()
		{
			var mgr = new MigrateGR();
			mgr.M0001_ContentReader_Theme();
		}

		private void M0001_ContentReader_Theme()
		{
			Type ParamType = typeof( Parameters );
			Type PropType = typeof( Properties );

			using ( SettingsContext Context = new SettingsContext() )
			{
				IEnumerable<FieldInfo> FInfo = ParamType.GetFields();
				foreach ( FieldInfo Info in FInfo )
				{
					string ParamName = Info.Name;
					string ParamValue = ( string ) Info.GetValue( null );
					(string DValue, GSDataType DType) = ValueType( PropType.GetProperty( ParamName ).GetValue( null ) );

					if ( ParamName.Contains( "CONTENTREADER" ) )
					{
						ParamValue = ParamValue.Replace( "Appearance_", "" ).Replace( "ContentReader_", "" );
						InsertOrUpdate( Context.ContentReader, new ContentReader() { Key = ParamValue, Type = DType, Value = DValue } );
					}
					else if( ParamName.Contains( "THEME" ) )
					{
						ParamValue = ParamValue.Replace( "Appearance_", "" ).Replace( "Theme_", "" ).Replace( "Appearence_", "" );
						InsertOrUpdate( Context.Theme, new Theme() { Key = ParamValue, Type = DType, Value = DValue } );
					}
				}

				Context.SaveChanges();
			}

		}

		private ( string, GSDataType ) ValueType( object Val )
		{
			if( Val is bool )
			{
				return (( bool ) Val ? "1" : "0", GSDataType.BOOL);
			}
			else if( Val is int )
			{
				return (Val.ToString(), GSDataType.INT);
			}
			else if ( Val is Color )
			{
				return (Val.ToString(), GSDataType.COLOR);
			}
			else if( Val is FontWeight )
			{
				return (( ( FontWeight ) Val ).Weight.ToString(), GSDataType.INT);
			}

			return (Val?.ToString(), GSDataType.STRING);
		}

		private void InsertOrUpdate<T>( DbSet<T> Table, T Entry ) where T : GenericSettings
		{
			T OEntry = Table.FirstOrDefault( e => e.Key == Entry.Key );
			if ( OEntry == null )
			{
				Table.Add( Entry );
			}
			else
			{
				OEntry.Type = Entry.Type;
				OEntry.Value = Entry.Value;
			}
		}
	}
}