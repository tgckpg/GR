using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Database
{
	using Contexts;

	class ContextManager
	{
		private static Type[] Contexts = new Type[]{
			typeof( SettingsContext )
			, typeof( BooksContext )
			, typeof( ZCacheContext )
		};

		public static void Migrate()
		{
			foreach ( Type Dbt in Contexts )
			{
				using ( DbContext Context = ( DbContext ) Activator.CreateInstance( Dbt ) )
				{
					Context.Database.Migrate();
				}
			}
		}

		public static void CreateFTSContext()
		{
			using ( DbContext Context = new FTSDataContext() )
			{
				Context.Database.EnsureDeleted();
				Context.Database.Migrate();
			}
		}

		public static bool ContextExists( Type ContextType )
		{
			using ( DbContext Context = ( DbContext ) Activator.CreateInstance( ContextType ) )
			{
				return ( Context.GetService<IDatabaseCreator>() as RelationalDatabaseCreator ).Exists();
			}
		}
	}
}