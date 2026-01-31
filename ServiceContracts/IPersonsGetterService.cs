using System;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Defines methods for retrieving person records.
    /// </summary>
    public interface IPersonsGetterService
    {
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
