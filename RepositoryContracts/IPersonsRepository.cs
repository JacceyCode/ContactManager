using Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents data acess operations for managing Persons entity.
    /// </summary>
    public interface IPersonsRepository
    {
        Task<Person> AddPerson(Person person);

        Task<List<Person>> GetAllPersons();

        Task<Person?> GetPersonByPersonId(Guid personId);

        Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);

        Task<Person> UpdatePerson(Person person);

        Task<bool> DeletePersonByPersonId(Guid personId);
    }
}
