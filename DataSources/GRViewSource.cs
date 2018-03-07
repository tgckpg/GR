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
		private bool _IsLoading;
		public bool IsLoading
		{
			get => _IsLoading;
			set
			{
				_IsLoading = value;
				NotifyChanged( "IsLoading" );
			}
		}

		private string _Message;
		public string Message
		{
			get => _Message;
			set
			{
				_Message = value;
				NotifyChanged( "Message" );
			}
		}

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