using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.Helpers;
using Net.Astropenguin.Logging;

namespace GR.Database.Contexts
{
	using Model.Interfaces;
	using Models;
	using Settings;

	class ZCacheContext : DbContext, ISafeContext
	{
		public DbSet<ZCache> KeyStore { get; set; }

		protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder )
		{
			optionsBuilder.UseSqlite( "Data Source=" + FileLinks.DB_ZCACHE );
			optionsBuilder.ReplaceService<IMigrationsSqlGenerator, GRMigrationsSqlGenerator>();
			optionsBuilder.ReplaceService<IMigrationsAnnotationProvider, GRMigrationsAnnotationProvider>();
		}

		private volatile object TransactionLock = new Object();

		private volatile object FlushLock = new Object();
		private volatile bool Flushing = false;

		private Worker DelayedWriter = new Worker( "ZCache" );
		private ConcurrentQueue<ZCache> Caches = new ConcurrentQueue<ZCache>();

		public ZCache GetCache( string Id )
		{
			ZCache Cache = Caches.FirstOrDefault( x => x.Key == Id );
			if ( Cache == null )
			{
				Cache = KeyStore.Find( Id );
				if ( Cache == null )
				{
					return null;
				}

				Caches.Enqueue( Cache );
			}

			return Cache;
		}

		public void Write( string Id, object Data )
		{
			DelayedWriter.Queue( () =>
			{
				ZCache Cache = Caches.FirstOrDefault( x => x.Key == Id ) ?? KeyStore.Find( Id );

				if ( Cache == null )
				{
					Cache = new ZCache() { Key = Id };
					Caches.Enqueue( Cache );
				}

				if( Data is byte[] BytesData )
				{
					Cache.Data.BytesValue = BytesData;
				}
				else if( Data is string StringData )
				{
					Cache.Data.StringValue = StringData;
				}
				else
				{
					throw new InvalidOperationException( "Unsupported data" );
				}

				FlushCaches();
			} );
		}

		private async void FlushCaches()
		{
			lock ( FlushLock )
			{
				if ( Flushing ) return;
				Flushing = true;
			}

			await Task.Delay( 3000 );

			BeginFlush:
			int l = Caches.Count();
			Logger.Log( "ZCache", "Flushing caches: " + l, LogType.DEBUG );

			List<ZCache> Untrack = new List<ZCache>();

			ZCache[] DetachedCaches = SafeRun( () => Caches.Where( x => Entry( x ).State == EntityState.Detached ).ToArray() );

			foreach ( ZCache Cache in DetachedCaches )
			{
				ZCache DbCache = KeyStore.Find( Cache.Key );
				if ( DbCache == null )
				{
					KeyStore.Add( Cache );
				}
				else
				{
					DbCache.RawData = Cache.RawData;
					Untrack.Add( Cache );
				}
			}

			SaveChanges();

			// Drop inactive entries
			int i = 0;
			while ( i < l )
			{
				if ( Caches.TryDequeue( out ZCache CCache ) )
				{
					if ( !( Untrack.Contains( CCache ) || Entry( CCache ).State == EntityState.Unchanged ) )
					{
						Caches.Enqueue( CCache );
						i++;
					}
				}
				else
				{
					break;
				}
			}

			if( 0 < i )
			{
				goto BeginFlush;
			}

			Flushing = false;
		}

		public void Reset()
		{
			lock ( FlushLock )
			{
				while ( Caches.TryDequeue( out ZCache NOP ) ) ;
				lock ( TransactionLock )
				{
					Database.ExecuteSqlCommand( "DELETE FROM \"KeyStore\"" );
					Database.ExecuteSqlCommand( "VACUUM;" );
				}
				SaveChanges();
			}
		}

		public override TEntity Find<TEntity>( params object[] keyValues )
		{
			lock ( TransactionLock )
			{
				return base.Find<TEntity>( keyValues );
			}
		}

		public override int SaveChanges()
		{
			lock ( TransactionLock )
			{
				return base.SaveChanges();
			}
		}

		public T SafeRun<T>( Func<T> Operation )
		{
			lock( TransactionLock )
			{
				return Operation();
			}
		}
	}
}