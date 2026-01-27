using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<PersonsRepository> _logger;

        public PersonsRepository(ApplicationDbContext db, ILogger<PersonsRepository> logger)
        {
            _db = db;
            _logger = logger;
        }
        public async Task<Person> AddPerson(Person person)
        {
            _db.Persons.Add(person);
            await _db.SaveChangesAsync();

            return person;
        }

        public async Task<bool> DeletePersonByPersonId(Guid personId)
        {
            _db.Persons.RemoveRange(_db.Persons.Where(temp => temp.PersonId == personId));
            int rowsDeleted = await _db.SaveChangesAsync();

            return rowsDeleted > 0;
        }

        public async Task<List<Person>> GetAllPersons()
        {
            return await _db.Persons.Include("Country").ToListAsync();
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            _logger.LogInformation("GetFilteredPersons of PersonsRepository");

            return await _db.Persons.Include("Country").Where(predicate).ToListAsync();
        }

        public async Task<Person?> GetPersonByPersonId(Guid personId)
        {
            return await _db.Persons.Include("Country").FirstOrDefaultAsync(temp => temp.PersonId == personId);
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            Person? personToUpdate = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonId == person.PersonId);

            if (personToUpdate == null)
                return person;

            personToUpdate.PersonName = person.PersonName;
            personToUpdate.Email = person.Email;
            personToUpdate.DateOfBirth = person.DateOfBirth;
            personToUpdate.Gender = person.Gender;
            personToUpdate.CountryId = person.CountryId;
            personToUpdate.Address = person.Address;
            personToUpdate.ReceiveNewsLetters = person.ReceiveNewsLetters;
            personToUpdate.TIN = person.TIN;

            await _db.SaveChangesAsync();

            return personToUpdate;
        }
    }
}
