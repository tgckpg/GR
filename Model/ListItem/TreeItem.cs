using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Net.Astropenguin.DataModel;
using Net.Astropenguin.Linq;

namespace GR.Model.ListItem
{
	using GSystem;
	public class TreeItem : ActiveData
	{
		public string ItemTitle { get; protected set; }
		public int TreeLevel
		{
			get
			{
				if( Parent == null )
				{
					return 0;
				}

				return 1 + Parent.TreeLevel;
			}
		}

		public string Path
		{
			get
			{
				if ( Parent == null )
				{
					return Utils.Base62( GetHashCode() );
				}

				return Parent.Path + "." + Utils.Base62( GetHashCode() );
			}
		}

		public TreeItem Parent { get; protected set; }

		private IList<TreeItem> _Children;
		public IEnumerable<TreeItem> Children
		{
			get => _Children;
			set
			{
				_Children = _Children ?? new List<TreeItem>();
				foreach ( TreeItem x in value )
				{
					if ( x.Parent == null )
					{
						x.Parent = this;
					}
					else
					{
						if ( x.Parent == this )
						{
							// Move item to the end of the list
							_Children.Remove( x );
						}
						else
						{
							throw new InvalidOperationException( "Item belongs to another tree" );
						}
					}

					_Children.Add( x );
				}
				NotifyChanged( "Children" );
			}
		}

		public TreeItem( string Name, int Level )
		{
			ItemTitle = Name;
		}

		public TreeItem( string Name ) : this( Name, 0 ) { }
	}

	public class TreeList : ObservableCollection<TreeItem>
	{
		public TreeList( IList<TreeItem> TreeItems )
			: base( TreeItems )
		{
		}

		public void Toggle( TreeItem Item )
		{
			lock ( this )
			{
				int ItemIndex = IndexOf( Item );

				if ( ItemIndex == -1 )
					throw new InvalidOperationException( "Item not found" );

				TreeItem[] OItems = this.Where( x => x != Item && x.Path.StartsWith( Item.Path ) ).ToArray();

				if ( OItems.Any() )
				{
					OItems.ExecEach( x => Remove( x ) );
				}
				else
				{
					ItemIndex++;
					Item.Children.ExecEach( ( x, i ) => InsertItem( ItemIndex + i, x ) );
				}
			}
		}
	}
}