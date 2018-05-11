using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR.DataSources
{
	using Data;
	using Model.ListItem;

	public class GRViewSource : TreeItem
	{
		public GRViewSource( string Name ) : base( Name ) { }

		public Type DataSourceType { get; set; }

		protected GRDataSource _DataSource;
		virtual public GRDataSource DataSource => _DataSource ?? ( _DataSource = ( GRDataSource ) Activator.CreateInstance( DataSourceType ) );

		virtual public Action<IGRRow> ItemAction { get; set; }

		virtual public GRViewSource Clone()
		{
			GRViewSource VS = ( GRViewSource ) MemberwiseClone();
			VS._DataSource = null;
			return VS;
		}
	}
}