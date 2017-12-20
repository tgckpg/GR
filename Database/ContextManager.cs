using Microsoft.EntityFrameworkCore;
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

			GR.Migrations.MigrateGR.Start();
		}

	}
}