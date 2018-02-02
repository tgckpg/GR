using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.DataSources
{
	using Model.ListItem;

	public class GRViewSource : TreeItem
	{
		public GRViewSource( string Name ) : base( Name ) { }

		public Type DataSourceType { get; set; }

		protected GRDataSource _DataSource;
		virtual public GRDataSource DataSource => _DataSource ?? ( _DataSource = ( GRDataSource ) Activator.CreateInstance( DataSourceType ) );
	}
}