using System;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Defines methods for adding and retrieving person records.
    /// </summary>
    public interface IPersonsDeleterService
    {
        /// <summary>
        /// Deletes the person with the specified identifier from the data store.
        /// </summary>
        /// <param name="personId">The unique identifier of the person to delete. Specify <see langword="null"/> to indicate that no person
        /// should be deleted.</param>
        /// <returns><see langword="true"/> if the person was successfully deleted; otherwise, <see langword="false"/>.</returns>
        Task<bool> DeletePerson(Guid? personId);
    }
}
