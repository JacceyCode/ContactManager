using System;
using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        private readonly ContactManagerDbContext _db;
        private readonly ICountriesService _countryService;

        public PersonsService(ContactManagerDbContext contactManagerDbContext, ICountriesService countryService)
        {
            _db = contactManagerDbContext;
            _countryService = countryService;
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            if (personAddRequest == null)
            {
                throw new ArgumentNullException(nameof(personAddRequest), "PersonAddRequest cannot be null");
            }

            // Model validations
            ValidationHelper.ModelValidation(personAddRequest);

            Person person = personAddRequest.ToPerson();

            _db.Persons.Add(person);
            await _db.SaveChangesAsync();
            //_db.sp_InsertPerson(person);

            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
            List<Person> persons = await _db.Persons.Include("Country").ToListAsync();

            return persons.Select(person => person.ToPersonResponse()).ToList();

            //return _db.sp_GetAllPersons()
            //    .Select(person => person.ToPersonResponse()).ToList();
        }

        public async Task<PersonResponse?> GetPersonByPersonId(Guid? personId)
        {
            if(personId == null)
            {
                return null;
            }

            Person? person = await _db.Persons.Include("Country")
                .FirstOrDefaultAsync(person => person.PersonId == personId);

            if (person == null)
            {
                return null;
            }

            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = await  GetAllPersons();
            List<PersonResponse> matchingPersons = allPersons;

            if(string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            {
                return matchingPersons;
            }

            switch (searchBy)
            {
                case nameof(PersonResponse.PersonName):
                    matchingPersons = allPersons
                        .Where(person => !string.IsNullOrEmpty(person.PersonName) ?
                                         person.PersonName
                                             .Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)
                        .ToList();
                    break;


                case nameof(PersonResponse.Email):
                    matchingPersons = allPersons
                        .Where(person => !string.IsNullOrEmpty(person.Email) ?
                                         person.Email
                                             .Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)
                        .ToList();
                    break;


                case nameof(PersonResponse.DateOfBirth):
                    matchingPersons = allPersons
                        .Where(person => (person.DateOfBirth != null) ?
                                       person.DateOfBirth.Value.ToString("dd MMM yyyy")
                                             .Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)
                        .ToList();
                    break;


                case nameof(PersonResponse.Gender):
                    matchingPersons = allPersons
                        .Where(person => !string.IsNullOrEmpty(person.Gender) ?
                                         person.Gender
                                             .StartsWith(searchString, StringComparison.OrdinalIgnoreCase) : true)
                        .ToList();
                    break;


                case nameof(PersonResponse.CountryId):
                    matchingPersons = allPersons
                        .Where(person => !string.IsNullOrEmpty(person.Country) ?
                                         person.Country
                                             .Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)
                        .ToList();
                    break;


                case nameof(PersonResponse.Address):
                    matchingPersons = allPersons
                        .Where(person => !string.IsNullOrEmpty(person.Address) ?
                                         person.Address
                                             .Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)
                        .ToList();
                    break;

                default: matchingPersons = allPersons; break;
            }

            return matchingPersons;
        }

        public async Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        {
           if(string.IsNullOrEmpty(sortBy))
            {
                return allPersons;
            }

            List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) => allPersons.OrderBy(person => person.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) => allPersons.OrderByDescending(person => person.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.ASC) => allPersons.OrderBy(person => person.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.DESC) => allPersons.OrderByDescending(person => person.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => allPersons.OrderBy(person => person.DateOfBirth).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => allPersons.OrderByDescending(person => person.DateOfBirth).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.ASC) => allPersons.OrderBy(person => person.Age).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.DESC) => allPersons.OrderByDescending(person => person.Age).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.ASC) => allPersons.OrderBy(person => person.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.DESC) => allPersons.OrderByDescending(person => person.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.ASC) => allPersons.OrderBy(person => person.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.DESC) => allPersons.OrderByDescending(person => person.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.ASC) => allPersons.OrderBy(person => person.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.DESC) => allPersons.OrderByDescending(person => person.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) => allPersons.OrderBy(person => person.ReceiveNewsLetters).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) => allPersons.OrderByDescending(person => person.ReceiveNewsLetters).ToList(),

                _ => allPersons,
            };

            return sortedPersons;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if(personUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(Person));
            }

            // Validations
            ValidationHelper.ModelValidation(personUpdateRequest);

            // Get matching person object from _persons list
            Person? matchingPerson = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonId == personUpdateRequest.PersonId);

            if(matchingPerson == null)
            {
                throw new ArgumentException($"Person with PersonId: {personUpdateRequest.PersonId} not found");
            }

            //Update properties
            matchingPerson.PersonName = personUpdateRequest.PersonName;
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString();
            matchingPerson.CountryId = personUpdateRequest.CountryId;
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

            await _db.SaveChangesAsync();

            return matchingPerson.ToPersonResponse();
        }

        public async Task<bool> DeletePerson(Guid? personId)
        {
            if(personId == null)
            {
                throw new ArgumentNullException(nameof(personId));
            }

            Person? matchingPerson = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonId == personId);
            if(matchingPerson == null)
            {
                return false;
            }

            _db.Persons.Remove(
                _db.Persons.First(person => person.PersonId == personId));

            await _db.SaveChangesAsync();

            return true;
        }
    }
}
