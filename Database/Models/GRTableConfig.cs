using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace GR.Database.Models
{
	using Schema;

	public class GRTableConfig
	{
		[Key]
		public string Id { get; set; }

		[NotMapped]
		public DbList<ColumnConfig> Columns { get; set; } = new DbList<ColumnConfig>();
		public string Json_Columns
		{
			get => Columns.Data;
			set => Columns.Data = value;
		}
	}

	public class GRWidgetConfig
	{
		[Key]
		public int Id { get; set; }

		[NotMapped]
		public WidgetConfig Conf { get; set; } = new WidgetConfig();
		public string Json_Conf
		{
			get => Conf.Data.Stringify();
			set => Conf.Parse( value );
		}
	}

	public class ColumnConfig : DbJsonInstance
	{
		public string Name { get; set; }
		public double Width { get; set; }
		public int Order { get; set; }

		public override IJsonValue Data
		{
			get
			{
				JsonObject obj = new JsonObject();
				obj[ "name" ] = JsonValue.CreateStringValue( Name );
				obj[ "width" ] = JsonValue.CreateNumberValue( Width );
				obj[ "order" ] = JsonValue.CreateNumberValue( Order );
				return obj;
			}
		}

		public ColumnConfig() : base( null ) { }
		public ColumnConfig( IJsonValue Data )
			: base( Data )
		{
			JsonObject Obj = Data.GetObject();
			Name = Obj.GetNamedString( "name" );
			Width = Obj.GetNamedNumber( "width" );
			Order = ( int ) Obj.GetNamedNumber( "order" );
		}
	}

	public class WidgetConfig : DbJsonInstance
	{
		public string Name { get; set; }
		public string TargetType { get; set; }
		public string Template { get; set; }
		public bool Enable { get; set; }
		public string Query { get; set; }

		public override IJsonValue Data
		{
			get
			{
				JsonObject obj = new JsonObject();
				obj[ "name" ] = JsonValue.CreateStringValue( Name ?? "" );
				obj[ "target_type" ] = JsonValue.CreateStringValue( TargetType );
				obj[ "template" ] = JsonValue.CreateStringValue( Template );
				obj[ "query" ] = JsonValue.CreateStringValue( Query ?? "" );
				obj[ "enable" ] = JsonValue.CreateBooleanValue( Enable );
				return obj;
			}
		}

		public WidgetConfig() : base( null ) { }
		public WidgetConfig( IJsonValue Data ) : base( Data ) => Parse( Data );

		public void Parse( string Data ) => Parse( JsonObject.Parse( Data ) );
		public void Parse( IJsonValue Data )
		{
			JsonObject Obj = Data.GetObject();
			Name = Obj.GetNamedString( "name" );
			Enable = Obj.GetNamedBoolean( "enable" );
			Template = Obj.GetNamedString( "template" );
			TargetType = Obj.GetNamedString( "target_type" );
			Query = Obj.GetNamedString( "query" );
		}

	}
}