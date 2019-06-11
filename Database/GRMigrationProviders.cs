using GR.Database.Schema;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GR.Database
{
	internal class GRMigrationsAnnotationProvider : SqliteMigrationsAnnotationProvider
	{
		public override IEnumerable<IAnnotation> For( IProperty property )
		{
			MemberInfo memberInfo = property.PropertyInfo ?? ( MemberInfo ) property.FieldInfo;
			AutoNow AutoDT = memberInfo?.GetCustomAttribute<AutoNow>();

			if ( AutoDT != null )
			{
				return base.For( property ).Concat( new IAnnotation[] { new Annotation( "AutoNow", ( byte ) AutoDT.Triggers ) } );
			}

			return base.For( property );
		}
	}

	internal class GRMigrationsSqlGenerator : SqliteMigrationsSqlGenerator 
	{
		public GRMigrationsSqlGenerator( IRelationalCommandBuilderFactory IRFactory, ISqlGenerationHelper ISHelper,  IRelationalTypeMapper Mapper, IRelationalAnnotationProvider AnProvider )
			: base( IRFactory, ISHelper, Mapper, AnProvider )
		{
			
		}

		protected override void Generate( CreateTableOperation operation, IModel model, MigrationCommandListBuilder builder )
		{
			base.Generate( operation, model, builder );
			foreach ( AddColumnOperation ColOp in operation.Columns )
			{
				OnColumnAdd( ColOp, builder );
			}
		}

		private Regex MSQLiteAlterCol = new Regex( @"^M\d{4}_" );

		private void OnColumnAdd( AddColumnOperation colOp, MigrationCommandListBuilder builder )
		{
			Annotation AutoDT = colOp.FindAnnotation( "AutoNow" );

			if ( AutoDT != null )
			{
				string TableName = SqlGenerationHelper.DelimitIdentifier( colOp.Table );
				string ColName = SqlGenerationHelper.DelimitIdentifier( colOp.Name );
				string UpdateTarget = TableName;

				if ( MSQLiteAlterCol.IsMatch( colOp.Table ) )
				{
					UpdateTarget = SqlGenerationHelper.DelimitIdentifier( colOp.Table.Substring( 6 ) );
				}

				SqliteTriggers Events = ( SqliteTriggers ) AutoDT.Value;

				if ( Events.HasFlag( SqliteTriggers.INSERT ) )
				{
					builder
						.Append( "CREATE TRIGGER " ).Append( SqlGenerationHelper.DelimitIdentifier( $"autodti_{colOp.Table}_{colOp.Name}" ) )
						.Append( $" AFTER INSERT ON {TableName}" )
						.Append( $" FOR EACH ROW BEGIN UPDATE {UpdateTarget} SET {ColName} = DATETIME( 'now' ) WHERE rowid = NEW.rowid; END" )
						.EndCommand();
				}

				if ( Events.HasFlag( SqliteTriggers.UPDATE ) )
				{
					builder
						.Append( "CREATE TRIGGER " ).Append( SqlGenerationHelper.DelimitIdentifier( $"autodtu_{colOp.Table}_{colOp.Name}" ) )
						.Append( $" AFTER UPDATE ON {TableName}" )
						.Append( $" FOR EACH ROW BEGIN UPDATE {UpdateTarget} SET {ColName} = DATETIME( 'now' ) WHERE rowid = NEW.rowid; END" )
						.EndCommand();
				}
			}
		}

		protected override void Generate( AddColumnOperation operation, IModel model, MigrationCommandListBuilder builder )
		{
			base.Generate( operation, model, builder );
			OnColumnAdd( operation, builder );
		}
	}
}