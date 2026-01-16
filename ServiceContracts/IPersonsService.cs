using System;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Defines methods for adding and retrieving person records.
    /// </summary>
    public interface IPersonsService
    {
        /// <summary>
        /// Adds a new person to the system using the specified request data.
        /// </summary>
        /// <param name="personAddRequest">The request containing the details of the person to add. Cannot be null.</param>
        /// <returns>A response object containing information about the added person, including any generated identifiers or
        /// status information.</returns>
        Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);

        /// <summary>
        /// Retrieves a list of all persons.
        /// </summary>
        /// <returns>A list of <see cref="PersonResponse"/> objects representing all persons. The list is empty if no persons are
        /// found.</returns>
        Task<List<PersonResponse>> GetAllPersons();

        /// <summary>
        /// Retrieves person details for the specified person identifier.
        /// </summary>
        /// <param name="personId">The unique identifier of the person to retrieve. If <paramref name="personId"/> is <see langword="null"/>,
        /// the method returns <see langword="null"/>.</param>
        /// <returns>A <see cref="PersonResponse"/> containing the person's details if found; otherwise, <see langword="null"/>.</returns>
        Task<PersonResponse?> GetPersonByPersonId(Guid? personId);


        /// <summary>
        /// Retrieves a list of persons that match the specified search criteria.
        /// </summary>
        /// <param name="searchBy">The name of the property to search by. Supported values include "FirstName", "LastName", or "Email".</param>
        /// <param name="searchString">The value to search for within the specified property. If null or empty, no filtering is applied and all
        /// persons are returned.</param>
        /// <returns>A list of <see cref="PersonResponse"/> objects that match the search criteria. Returns an empty list if no
        /// persons match the criteria.</returns>
        Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);

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


        /// <summary>
        /// Updates the details of an existing person based on the specified update request.
        /// </summary>
        /// <param name="personUpdateRequest">An object containing the updated information for the person. Cannot be null.</param>
        /// <returns>A response object containing the updated details of the person.</returns>
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);


        /// <summary>
        /// Deletes the person with the specified identifier from the data store.
        /// </summary>
        /// <param name="personId">The unique identifier of the person to delete. Specify <see langword="null"/> to indicate that no person
        /// should be deleted.</param>
        /// <returns><see langword="true"/> if the person was successfully deleted; otherwise, <see langword="false"/>.</returns>
        Task<bool> DeletePerson(Guid? personId);


        /// <summary>
        /// Asynchronously generates a CSV file containing information about all persons.
        /// </summary>
        /// <remarks>The caller is responsible for disposing the returned <see cref="MemoryStream"/> after
        /// use. The CSV format includes all available person records at the time of the call.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="MemoryStream"/>
        /// with the CSV data for all persons. The stream's position is set to the beginning.</returns>
        Task<MemoryStream> GetPersonsCSV();


        /// <summary>
        /// Returns persons data in an Excel file format.
        /// </summary>
        /// <returns></returns>
        Task<MemoryStream> GetPersonsExcel();
    }
}
