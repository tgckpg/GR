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
		public TreeItem Self => this;

		protected string _ItemTitle;
		public string ItemTitle
		{
			get => _ItemTitle;
			protected set
			{
				_ItemTitle = value;
				NotifyChanged( "ItemTitle" );
			}
		}

		public int TreeLevel
		{
			get
			{
				if ( Parent == null )
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

		private IList<TreeItem> _Children = new TreeItem[ 0 ];
		public IEnumerable<TreeItem> Children
		{
			get => _Children;
			set
			{
				foreach ( TreeItem x in value )
				{
					_AddChild( x );
				}
				NotifyChanged( "Children" );
			}
		}

		public void AddChild( TreeItem Item )
		{
			_AddChild( Item );
			NotifyChanged( "Children" );
		}

		public void RemoveChild( TreeItem Item )
		{
			_RemoveChild( Item );
			NotifyChanged( "Children" );
		}

		public TreeItem( string Name, int Level )
		{
			_ItemTitle = Name;
		}

		public TreeItem( string Name ) : this( Name, 0 ) { }

		private void _AddChild( TreeItem x )
		{
			if ( !( _Children is List<TreeItem> ) )
				_Children = new List<TreeItem>();

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

		private void _RemoveChild( TreeItem x )
		{
			if ( _Children.Contains( x ) )
			{
				_Children.Remove( x );
				x.Parent = null;
			}
		}
	}

	public class TreeList : ObservableCollection<TreeItem>
	{
		private TreeItem[] Source;

		public TreeList( IList<TreeItem> TreeItems )
			: base( TreeItems )
		{
		}

		public void Toggle( TreeItem Item )
		{
			lock ( this ) _Toggle( Item );
		}

		public void Filter( string Name )
		{
			if ( Source == null )
			{
				Source = this.Where( x => x.Parent == null ).ToArray();
			}

			Clear();

			if ( string.IsNullOrEmpty( Name ) && Source != null )
			{
				Source.ExecEach( x => Add( x ) );
				Source = null;
				return;
			}

			_Filter( Name, Source );
		}

		private bool _Filter( string Name, IEnumerable<TreeItem> Items )
		{
			bool AnyMatched = false;

			if ( Items?.Any() == true )
			{
				foreach ( TreeItem Item in Items )
				{
					Add( Item );
					bool ChildrenMatch = _Filter( Name, Item.Children );

					if ( ChildrenMatch || Item.ItemTitle.Contains( Name ) )
					{
						AnyMatched = true;
					}
					else
					{
						Remove( Item );
					}
				}
			}

			return AnyMatched;
		}

		public void Open( TreeItem Item )
		{
			lock ( this )
			{
				List<TreeItem> Path = new List<TreeItem>();
				_BuildPath( Item, Path );
				for ( int i = 0, l = Path.Count(); i < l; i++ )
				{
					TreeItem K = Path[ i ];
					if ( IndexOf( K ) == -1 && 0 < i )
					{
						_Toggle( Path[ i - 1 ] );
					}
				}
			}
		}

		private void _Toggle( TreeItem Item )
		{
			int ItemIndex = IndexOf( Item );

			if ( ItemIndex == -1 )
				throw new InvalidOperationException( "Item not found" );

			TreeItem[] OItems = this.Where( x => x != Item && x.Path.StartsWith( Item.Path ) ).ToArray();
			int OLen = OItems.Where( x => x.Parent == Item ).Count();

			if ( OItems.Any() && OLen == Item.Children.Count() )
			{
				OItems.ExecEach( x => Remove( x ) );
			}
			else
			{
				ItemIndex++;
				Item.Children
					.Where( x => !OItems.Contains( x ) )
					.ExecEach( ( x, i ) => InsertItem( ItemIndex + i + OLen, x ) );
			}
		}

		private void _BuildPath( TreeItem Item, List<TreeItem> PathTree )
		{
			if ( Item.Parent != null )
			{
				_BuildPath( Item.Parent, PathTree );
			}

			PathTree.Add( Item );
		}

	}
}