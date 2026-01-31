using System;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Defines methods for adding and retrieving person records.
    /// </summary>
    public interface IPersonsSorterService
    {
        /// <summary>
        /// Returns a list of persons sorted according to the specified property and sort order.
        /// </summary>
        /// <remarks>If an invalid property name is provided in <paramref name="sortBy"/>, the method may
        /// throw an exception. The original list is not modified.</remarks>
        /// <param name="allPersons">The collection of persons to be sorted. Cannot be null.</param>
        /// <param name="sortBy">The name of the property to sort by. Must correspond to a valid property of the PersonResponse type.</param>
        /// <param name="sortOrder">ASC or DESC</param>
        /// <returns>A new list of PersonResponse objects sorted by the specified property and order. If the input list is empty,
        /// returns an empty list.</returns>
        Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder);
    }
}
