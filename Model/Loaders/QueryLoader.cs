using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Net.Astropenguin.Loaders;

namespace GR.Model.Loaders
{
	using Interfaces;

	class QueryLoader<T> : ILoader<T>
	{
		public Action<IList<T>> Connector { get; set; }
		public int CurrentPage => NLoaded;
		public bool PageEnded => NTotal <= NLoaded;
		public int NTotal { get; private set; }

		private IQueryable<T> QueryExp;
		private ISafeContext SContext;

		private int NLoaded = 0;

		public QueryLoader( IQueryable<T> QueryExp, ISafeContext SContext )
		{
			this.QueryExp = QueryExp;
			this.SContext = SContext;
			NTotal = SContext.SafeRun( () => QueryExp.Count() );
		}

		public Task<IList<T>> NextPage( uint count )
		{
			return Task.Run( () =>
			{
				IList<T> Items = SContext.SafeRun( () => QueryExp.Skip( NLoaded ).Take( ( int ) count ).ToArray() );
				NLoaded += Items.Count();
				return Items;
			} );
		}

	}
}