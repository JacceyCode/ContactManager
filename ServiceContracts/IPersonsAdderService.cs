using System;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Defines methods for adding and retrieving person records.
    /// </summary>
    public interface IPersonsAdderService
    {
        /// <summary>
        /// Adds a new person to the system using the specified request data.
        /// </summary>
        /// <param name="personAddRequest">The request containing the details of the person to add. Cannot be null.</param>
        /// <returns>A response object containing information about the added person, including any generated identifiers or
        /// status information.</returns>
        Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);
    }
}
