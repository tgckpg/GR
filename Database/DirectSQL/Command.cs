using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.Database.DirectSQL
{
	sealed class Command
	{
		private Command() { }

		public static (ResultDisplayData, string) Exec( DbContext Context, string Query )
		{
			DbConnection Conn = Context.Database.GetDbConnection();
			Conn.Open();

			try
			{
				DbTransaction Transaction = Conn.BeginTransaction();

				DbCommand Command = Conn.CreateCommand();
				Command.CommandText = Query;
				DbDataReader Reader = Command.ExecuteReader();

				Transaction.Commit();


				if( Reader.RecordsAffected != -1 )
				{
					return (null, $"Query OK, {Reader.RecordsAffected} row(s) affected.");
				}
				else
				{
					return (new ResultDisplayData( Reader ), "Query OK.");
				}
			}
			catch ( Exception ex )
			{
				return (null, ex.Message);
			}
			finally
			{
				Conn.Close();
			}
		}

	}
}