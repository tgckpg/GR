using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.DataModel;
using Net.Astropenguin.Linq;

namespace GR.Database.DirectSQL
{
	using Data;
	using DataSources;
	using Model.Loaders;
	using Models;

	sealed class ResultViewSource: GRViewSource
	{
		public ResultDisplayData RSource { get; private set; }
		public override GRDataSource DataSource => RSource;

		public ResultViewSource( ResultDisplayData Data )
			: base( "Result view" )
		{
			RSource = Data;
		}
	}

	sealed class ResultDisplayData : GRDataSource
	{
		public const int MAX_COLS = 10;

		public override IGRTable Table => DataTable;

		public override string ConfigId => "ResultDisplayData";
		public override bool Searchable => false;

		public bool HasData { get; private set; }

		protected override ColumnConfig[] DefaultColumns => SelectedCols;

		private GRTable<ResultData> DataTable;

		private string[] Headers;
		private IGRCell[] CellProps;
		private List<ResultData> ResultSet;
		private ColumnConfig[] SelectedCols;

		public ResultDisplayData( DbDataReader Reader )
		{
			HasData = Reader.Read();
			if ( !HasData )
				return;

			Headers = Reader.Remap( 0, Reader.FieldCount, ( r, i ) => r.GetName( i ) );

			Type DR = typeof( ResultData );
			Type StrType = typeof( string );

			PropertyInfo[] DRStrings = DR.GetProperties().Where( p => p.PropertyType == StrType ).ToArray();

			CellProps = new IGRCell[ Headers.Length ];
			SelectedCols = new ColumnConfig[ Math.Min( Headers.Length, MAX_COLS ) ];

			Headers.ExecEach( ( x, i ) =>
			{
				PropertyInfo PInfo = DRStrings[ i ];
				if ( i < MAX_COLS )
				{
					SelectedCols[ i ] = new ColumnConfig() { Name = PInfo.Name, Width = 200 };
				}
				CellProps[ i ] = new GRCell<ResultData>( PInfo );
			} );

			ResultSet = new List<ResultData>();

			do
			{
				ResultData Data = new ResultData();
				CellProps.ExecEach( ( x, i ) => CellProps[ i ].Property.SetValue( Data, Reader.GetValue( i ).ToString() ) );
				ResultSet.Add( Data );
			}
			while ( Reader.Read() );
		}

		public override void StructTable()
		{
			DataTable = new GRTable<ResultData>( new List<IGRCell>( CellProps ) ) { Font = ConsolasFont };
			DataTable.Cell = ( i, x ) => DataTable.ColEnabled( i ) ? ColumnName( DataTable.CellProps[ i ] ) : "";
		}

		public override string ColumnName( IGRCell CellProp ) => Headers[ Array.IndexOf( CellProps, CellProp ) ];

		public override void Reload()
		{
			LargeList<ResultData> DataList = new LargeList<ResultData>( ResultSet );
			Observables<ResultData, GRRow<ResultData>> DataObs = new Observables<ResultData, GRRow<ResultData>>();
			DataObs.ConnectLoader( DataList, s => s.Remap( x => new GRRow<ResultData>( DataTable ) { Source = x, Font = ConsolasFont } ) );
			DataTable.Items = DataObs;
		}

		private string ConsolasFont( int arg1, object arg2 ) => "Consolas";

		public override Task ConfigureAsync()
		{
			GRTableConfig Config = new GRTableConfig() { Id = ConfigId };
			Config.Columns.AddRange( DefaultColumns );
			Table.Configure( Config );

			return Task.Delay( 0 );
		}

		public override void Sort( int ColIndex, int Order ) { /* Not Supported */ }
		public override void ToggleSort( int ColIndex ) { /* Not Supported */  }
		protected override void ConfigureSort( string PropertyName, int Order ) {  /* Not Supported */ }
	}

	public class ResultData
	{
		public string V00 { get; set; } public string V01 { get; set; } public string V02 { get; set; } public string V03 { get; set; } public string V04 { get; set; }
		public string V05 { get; set; } public string V06 { get; set; } public string V07 { get; set; } public string V08 { get; set; } public string V09 { get; set; }
		public string V10 { get; set; } public string V11 { get; set; } public string V12 { get; set; } public string V13 { get; set; } public string V14 { get; set; }
		public string V15 { get; set; } public string V16 { get; set; } public string V17 { get; set; } public string V18 { get; set; } public string V19 { get; set; }
		public string V20 { get; set; } public string V21 { get; set; } public string V22 { get; set; } public string V23 { get; set; } public string V24 { get; set; }
		public string V25 { get; set; } public string V26 { get; set; } public string V27 { get; set; } public string V28 { get; set; } public string V29 { get; set; }
		public string V30 { get; set; } public string V31 { get; set; } public string V32 { get; set; } public string V33 { get; set; } public string V34 { get; set; }
		public string V35 { get; set; } public string V36 { get; set; } public string V37 { get; set; } public string V38 { get; set; } public string V39 { get; set; }
		public string V40 { get; set; } public string V41 { get; set; } public string V42 { get; set; } public string V43 { get; set; } public string V44 { get; set; }
		public string V45 { get; set; } public string V46 { get; set; } public string V47 { get; set; } public string V48 { get; set; } public string V49 { get; set; }
		public string V50 { get; set; } public string V51 { get; set; } public string V52 { get; set; } public string V53 { get; set; } public string V54 { get; set; }
		public string V55 { get; set; } public string V56 { get; set; } public string V57 { get; set; } public string V58 { get; set; } public string V59 { get; set; }
		public string V60 { get; set; } public string V61 { get; set; } public string V62 { get; set; } public string V63 { get; set; } public string V64 { get; set; }
		public string V65 { get; set; } public string V66 { get; set; } public string V67 { get; set; } public string V68 { get; set; } public string V69 { get; set; }
		public string V70 { get; set; } public string V71 { get; set; } public string V72 { get; set; } public string V73 { get; set; } public string V74 { get; set; }
		public string V75 { get; set; } public string V76 { get; set; } public string V77 { get; set; } public string V78 { get; set; } public string V79 { get; set; }
		public string V80 { get; set; } public string V81 { get; set; } public string V82 { get; set; } public string V83 { get; set; } public string V84 { get; set; }
		public string V85 { get; set; } public string V86 { get; set; } public string V87 { get; set; } public string V88 { get; set; } public string V89 { get; set; }
		public string V90 { get; set; } public string V91 { get; set; } public string V92 { get; set; } public string V93 { get; set; } public string V94 { get; set; }
		public string V95 { get; set; } public string V96 { get; set; } public string V97 { get; set; } public string V98 { get; set; } public string V99 { get; set; }
	}

}