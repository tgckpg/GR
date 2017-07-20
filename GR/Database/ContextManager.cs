using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.Logging;

namespace GR.Database
{
	using Contexts;
	class ContextManager
	{
		public const LogType INFO = LogType.R19;

		private static Type[] Contexts = new Type[]{
			typeof( SettingsContext )
		};

		public static void Migrate()
		{
			foreach ( Type Dbt in Contexts )
			{
				using ( DbContext Context = ( DbContext ) Activator.CreateInstance( Dbt ) )
				{
					Logger.Log( "EntityFramework", "Init Db: " + Dbt.Name, INFO );
					Context.Database.Migrate();
				}
			}

			MigrateWen8.Start();
		}

	}
}
