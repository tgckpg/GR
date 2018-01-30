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

}