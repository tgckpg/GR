using System.Collections.Generic;

namespace GR.Model.Interfaces
{
	interface ISearchableSection<T>
	{
		IEnumerable<T> SearchSet { get; set; }
		string SearchTerm { get; set; }
	}
}
