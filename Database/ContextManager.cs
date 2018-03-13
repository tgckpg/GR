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
	using Model.Interfaces;
	using Resources;

	class ContextManager
	{
		private static Type[] Contexts = new Type[]{
			typeof( SettingsContext )
			, typeof( BooksContext )
			, typeof( ZCacheContext )
		};

		public static List<Func<IMigrationOp, Task>> MigrationOps = new List<Func<IMigrationOp, Task>>();

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

		public static void RemoveFTSContext()
		{
			using ( DbContext Context = new FTSDataContext() )
			{
				Context.Database.EnsureDeleted();
			}
		}

		public static void ClearBookTexts()
		{
			Shared.BooksDb.SafeRun( Db => Db.Volumes.RemoveRange( Db.Volumes.ToList() ) );
			Shared.BooksDb.SaveChanges();
			Shared.BooksDb.SafeRun( Db => Db.Database.ExecuteSqlCommand( "VACUUM;" ) );
			Shared.BooksDb.SaveChanges();
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