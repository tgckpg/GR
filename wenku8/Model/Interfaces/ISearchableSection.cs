using System.Collections.Generic;

namespace wenku8.Model.Interfaces
{
    interface ISearchableSection<T>
    {
        IEnumerable<T> SearchSet { get; set; }
        string SearchTerm { get; set; }
    }
}
