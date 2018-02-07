using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.Helpers;
using Net.Astropenguin.Logging;

namespace GR.Database.Contexts
{
	using Models;
	using System.Collections.Concurrent;

	class ZCacheContext : DbContext
	{
		public DbSet<ZCache> KeyStore { get; set; }

		protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder )
		{
			optionsBuilder.UseSqlite( "Data Source=caches.db" );
			optionsBuilder.ReplaceService<IMigrationsSqlGenerator, GRMigrationsSqlGenerator>();
			optionsBuilder.ReplaceService<IMigrationsAnnotationProvider, GRMigrationsAnnotationProvider>();
		}

		private volatile object FlushLock = new Object();
		private volatile bool Flushing = false;

		private Worker DelayedWriter = new Worker();
		private ConcurrentQueue<ZCache> Caches = new ConcurrentQueue<ZCache>();

		public ZCache GetCache( string Id )
		{
			ZCache Cache = Caches.FirstOrDefault( x => x.Key == Id );
			if ( Cache == null )
			{
				Cache = KeyStore.Find( Id );
				if( Cache == null )
				{
					return null;
				}

				Caches.Enqueue( Cache );
			}

			return Cache;
		}

		public void Write( string Id, byte[] Data )
		{
			DelayedWriter.Queue( () =>
			{
				ZCache Cache = Caches.FirstOrDefault( x => x.Key == Id ) ?? KeyStore.Find( Id );

				if( Cache == null )
				{
					Cache = new ZCache() { Key = Id };
					Caches.Enqueue( Cache );
				}

				Cache.Data.BytesValue = Data;

				DelayedWriter.Queue( FlushCaches );
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

			int l = Caches.Count();
			Logger.Log( "ZCache", "Flushing caches: " + l, LogType.DEBUG );

			List<ZCache> Untrack = new List<ZCache>();
			foreach ( ZCache Cache in Caches )
			{
				// Tracked entries are handled by EF Core
				if ( Entry( Cache ).State != EntityState.Detached )
					continue;

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
			for ( int i = 0; i < l; )
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

			Flushing = false;
		}

	}
}