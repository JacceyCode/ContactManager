using System;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Defines methods for adding and retrieving person records.
    /// </summary>
    public interface IPersonsUpdaterService
    {
        /// <summary>
        /// Updates the details of an existing person based on the specified update request.
        /// </summary>
        /// <param name="personUpdateRequest">An object containing the updated information for the person. Cannot be null.</param>
        /// <returns>A response object containing the updated details of the person.</returns>
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);
    }
}
