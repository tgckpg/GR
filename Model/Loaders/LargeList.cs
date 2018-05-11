using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.Loaders;

namespace GR.Model.Loaders
{
	class LargeList<T> : ILoader<T>
	{
		public Action<IList<T>> Connector { get; set; }

		public int CurrentPage => NLoaded;

		public bool PageEnded => NTotal <= NLoaded;
		public int NTotal { get; private set; }

		private IEnumerable<T> Source;

		private int NLoaded = 0;

		public LargeList( IEnumerable<T> Source )
		{
			this.Source = Source;
			NTotal = Source.Count();
		}

		public Task<IList<T>> NextPage( uint count )
		{
			return Task.Run( () =>
			{
				IList<T> Items = Source.Skip( NLoaded ).Take( ( int ) count ).ToArray();
				NLoaded += Items.Count();
				return Items;
			} );
		}
	}
}