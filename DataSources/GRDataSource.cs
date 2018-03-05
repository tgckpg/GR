using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

using Net.Astropenguin.DataModel;
using Net.Astropenguin.Linq;

namespace GR.DataSources
{
	using Data;
	using Database.Contexts;
	using Database.Models;

	public class EmptySearchQueryException : Exception { }

	abstract public class GRDataSource : ActiveData
	{
		abstract public IGRTable Table { get; }
		virtual public string SearchExample => "";

		private bool _IsLoading;
		virtual public bool IsLoading
		{
			get => _IsLoading;
			protected set
			{
				_IsLoading = value;
				NotifyChanged( "IsLoading" );
			}
		}

		private string _Message;
		virtual public string Message
		{
			get => _Message;
			protected set
			{
				_Message = value;
				NotifyChanged( "Message" );
			}
		}

		virtual public bool Searchable => true;

		protected string _Search = "";
		virtual public string Search
		{
			get => _Search;
			set
			{
				_Search = value;
				NotifyChanged( "Search" );
				Task.Run( () => Reload() );
			}
		}

		/// <summary>
		/// Construct table columns
		/// </summary>
		abstract public void StructTable();

		abstract public void Reload();
		abstract public void Sort( int ColIndex, int Order );
		abstract public void ToggleSort( int ColIndex );

		abstract public string ColumnName( IGRCell CellProp );

		abstract public string ConfigId { get; }
		abstract protected ColumnConfig[] DefaultColumns { get; }

		virtual public async Task SaveConfig()
		{
			using ( SettingsContext Settings = new SettingsContext() )
			{
				GRTableConfig Config = Settings.TableConfigs.Find( ConfigId );
				if ( Config == null )
				{
					Config = new GRTableConfig() { Id = ConfigId };
					Settings.TableConfigs.Add( Config );
				}

				Config.Columns.Clear();
				Config.Columns.AddRange(
					Table.Headers
					.Take( Table.CellProps.Count() )
					.Remap( ( x, i ) => new ColumnConfig()
					{
						Name = Table.CellProps[ i ].Property.Name,
						Width = ( ( GridLength ) x.GetValue( Table ) ).Value,
						Order = ( int ) Table.Sortings[ i ].GetValue( Table )
					} ) );

				await Settings.SaveChangesAsync();
			}
		}

		/// <summary>
		/// Load table configurations for this DataSource
		/// </summary>
		/// <returns></returns>
		virtual public async Task ConfigureAsync()
		{
			using ( SettingsContext Settings = new SettingsContext() )
			{
				GRTableConfig Config = Settings.TableConfigs.Find( ConfigId );

				// Set the default configs
				if ( Config == null )
				{
					Config = new GRTableConfig() { Id = ConfigId };
					Config.Columns.AddRange( DefaultColumns );

					Settings.TableConfigs.Add( Config );
					await Settings.SaveChangesAsync();
				}

				Table.Configure( Config );
				ColumnConfig SortingCol = Config.Columns.FirstOrDefault( x => x.Order != 0 && 0 < x.Width );
				if ( SortingCol != null )
				{
					ConfigureSort( SortingCol.Name, SortingCol.Order );
				}
			}
		}

		abstract protected void ConfigureSort( string PropertyName, int Order );
	}
}