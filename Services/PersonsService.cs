using System;
using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        private readonly List<Person> _persons;
        private readonly ICountriesService _countryService;

        public PersonsService(bool initialize = true)
        {
            _persons = new List<Person>();
            _countryService = new CountriesService();

            if (initialize)
            {
                _persons.AddRange(new List<Person>()
                {
                    new()
                    {
                        PersonId = Guid.Parse("348A27F6-B6F7-42A3-96C4-4761B55EAE1A"),
                        PersonName = "John Doe",
                        Email = "johndoe@example.com",
                        Address = "123 Main St",
                        DateOfBirth = new DateTime(2001, 1, 1),
                        CountryId = Guid.Parse("C014565A-F840-4BEC-915E-FEEDF96BF421"),
                        Gender = "Male",
                        ReceiveNewsLetters = true,
                    },

                    new()
                    {
                        PersonId = Guid.Parse("9458296D-CE8E-429F-ACC9-5959828797FC"),
                        PersonName = "James New",
                        Email = "jamesnew@example.com",
                        Address = "123 Main St",
                        DateOfBirth = new DateTime(1997, 1, 1),
                        CountryId = Guid.Parse("C014565A-F840-4BEC-915E-FEEDF96BF421"),
                        Gender = "Male",
                        ReceiveNewsLetters = true,
                    },

                    new()
                    {
                        PersonId = Guid.Parse("45F73264-4137-41C0-8E6C-F7EF31CE7596"),
                        PersonName = "Joane Mills",
                        Email = "joanemills@example.com",
                        Address = "123 Main St",
                        DateOfBirth = new DateTime(1990, 1, 1),
                        CountryId = Guid.Parse("5D797884-CE65-4FA7-B3EC-BB01E5858B39"),
                        Gender = "Female",
                        ReceiveNewsLetters = true,
                    },

                    new()
                    {
                        PersonId = Guid.Parse("B3DE8101-1A54-44E0-9444-C2C6D8DB58C2"),
                        PersonName = "Damme Yul",
                        Email = "dammeyul@example.com",
                        Address = "123 Main St",
                        DateOfBirth = new DateTime(1990, 1, 1),
                        CountryId = Guid.Parse("AB7CC0EA-528B-415F-8975-5A0177F7AF64"),
                        Gender = "Female",
                        ReceiveNewsLetters = true,
                    },

                    new()
                    {
                        PersonId = Guid.Parse("224CFAEB-397C-4A19-9B1E-4328AC9D3AF0"),
                        PersonName = "Tosin Ayo",
                        Email = "tosinayo@example.com",
                        Address = "123 Main St",
                        DateOfBirth = new DateTime(1990, 1, 1),
                        CountryId = Guid.Parse("04E97365-F1DB-424F-B900-E2BE2A19DB71"),
                        Gender = "Male",
                        ReceiveNewsLetters = true,
                    }
                });
            }
        }

        private PersonResponse ConvertPersonToPersonResponse(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();

            personResponse.Country = _countryService.GetCountryByCountryId(personResponse.CountryId)?.CountryName;
            return personResponse;
        }

        public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
        {
            if (personAddRequest == null)
            {
                throw new ArgumentNullException(nameof(personAddRequest), "PersonAddRequest cannot be null");
            }

            // Model validations
            ValidationHelper.ModelValidation(personAddRequest);

            Person person = personAddRequest.ToPerson();

            _persons.Add(person);

            return ConvertPersonToPersonResponse(person);
        }

        public List<PersonResponse> GetAllPersons()
        {
            return _persons.Select(person => ConvertPersonToPersonResponse(person)).ToList();
        }

        public PersonResponse? GetPersonByPersonId(Guid? personId)
        {
            if(personId == null)
            {
                return null;
            }

            Person? person = _persons.FirstOrDefault(person => person.PersonId == personId);
            if (person == null)
            {
                return null;
            }

            return ConvertPersonToPersonResponse(person);
        }

        public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = GetAllPersons();
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

        public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
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

        public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if(personUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(Person));
            }

            // Validations
            ValidationHelper.ModelValidation(personUpdateRequest);

            // Get matching person object from _persons list
            Person? matchingPerson = _persons.FirstOrDefault(temp => temp.PersonId == personUpdateRequest.PersonId);

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


            return ConvertPersonToPersonResponse(matchingPerson);
        }

        public bool DeletePerson(Guid? personId)
        {
            if(personId == null)
            {
                throw new ArgumentNullException(nameof(personId));
            }

            Person? matchingPerson = _persons.FirstOrDefault(temp => temp.PersonId == personId);
            if(matchingPerson == null)
            {
                return false;
            }

            _persons.RemoveAll(person => person.PersonId == personId);

            return true;
        }
    }
}
