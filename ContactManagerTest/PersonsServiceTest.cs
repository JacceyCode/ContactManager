using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using System.Net;
using System.Reflection;
using Xunit.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace ContactManagerTest
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countryService;
        private readonly ITestOutputHelper _testOutputHelper;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _countryService = new CountriesService(
                new ContactManagerDbContext(
                    new DbContextOptionsBuilder<ContactManagerDbContext>().Options
                    )
                );

            _personsService = new PersonsService(
                new ContactManagerDbContext(
                    new DbContextOptionsBuilder<ContactManagerDbContext>().Options
                    ),
                _countryService
                );
            
            _testOutputHelper = testOutputHelper;
        }


        #region AddPerson Tests
        // When person is null, AddPerson should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPersonAddRequest_ThrowsArgumentNullException()
        {
            // Arrange
            PersonAddRequest? request = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _personsService.AddPerson(request));
        }

        // When personName is null, throws ArgumentException
        [Fact]
        public async Task AddPerson_NullPersonName_ThrowsArgumentException()
        {
            // Arrange
            PersonAddRequest personAddRequest = new()
            {
                PersonName = null,
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _personsService.AddPerson(personAddRequest));
        }

        // When valid PersonAddRequest is provided, AddPerson should return PersonResponse with same details
        [Fact]
        public async Task AddPerson_ValidPersonAddRequest_ReturnsPersonResponse()
        {
            // Arrange
            PersonAddRequest personAddRequest = new()
            {
                PersonName = "John Doe",
                Email = "johndoe@example.com",
                Address = "123 Main St",
                DateOfBirth = new DateTime(1990, 1, 1),
                CountryId = Guid.NewGuid(),
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true,
            };

            // Act
            PersonResponse person = await _personsService.AddPerson(personAddRequest);

            List<PersonResponse> persons = await _personsService.GetAllPersons();

            // Assert
            Assert.NotNull(person);
            Assert.True(person.PersonId != Guid.Empty);
            Assert.True(person.ReceiveNewsLetters);

            Assert.Contains(person, persons);
        }
        #endregion


        #region GetPersonByPersonId Tests
        // When personId is null, GetPersonByPersonId should return null
        [Fact]
        public async Task GetPersonByPersonId_NullPersonId()
        {
            // Arrange
            Guid? personId = null;

            // Act
            PersonResponse? person = await _personsService.GetPersonByPersonId(personId);

            // Assert
            Assert.Null(person);
        }

        // When valid personId is provided, GetPersonByPersonId should return PersonResponse with same personId
        [Fact]
        public async Task GetPersonByPersonId_ValidPersonId_ReturnsPersonResponse()
        {
            // Arrange
            CountryAddRequest countryRequest = new()
            {
                CountryName = "Canada",
            };
            CountryResponse countryResponse = await _countryService.AddCountry(countryRequest);

            // Act
            PersonAddRequest personAddRequest = new()
            {
                PersonName = "John Doe",
                Email = "johndoe@example.com",
                Address = "123 Main St",
                DateOfBirth = new DateTime(1990, 1, 1),
                CountryId = countryResponse.CountryId,
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true,
            };

            PersonResponse addedPerson = await _personsService.AddPerson(personAddRequest);

            PersonResponse? fetchedPerson = await _personsService.GetPersonByPersonId(addedPerson.PersonId);

            // Assert
            Assert.NotNull(addedPerson);
            Assert.NotNull(fetchedPerson);
            Assert.Equal(addedPerson, fetchedPerson);
        }

        #endregion


        #region GetAllPersons Tests
        // GetAllPersons should return empty list when no persons are added
        [Fact]
        public async Task GetAllPersons_NoPersonsAdded_ReturnsEmptyList()
        {
            // Act
            List<PersonResponse> persons = await _personsService.GetAllPersons();

            // Assert
            Assert.NotNull(persons);
            Assert.Empty(persons);
        }

        // When multiple persons are added, GetAllPersons should return all added persons
        [Fact]
        public async Task GetAllPersons_MultiplePersonsAdded_ReturnsAllPersons()
        {
            // Arrange
            CountryAddRequest countryRequest1 = new()
            {
                CountryName = "Canada",
            };
            CountryAddRequest countryRequest2 = new()
            {
                CountryName = "USA",
            };

            CountryResponse countryResponse1 = await _countryService.AddCountry(countryRequest1);
            CountryResponse countryResponse2 = await _countryService.AddCountry(countryRequest2);

            // Act
            PersonAddRequest personAddRequest1 = new()
            {
                PersonName = "John Doe",
                Email = "johndoe@example.com",
                Address = "123 Main St",
                DateOfBirth = new DateTime(1990, 1, 1),
                CountryId = countryResponse1.CountryId,
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true,
            };
            PersonAddRequest personAddRequest2 = new()
            {
                PersonName = "Mark Ben",
                Email = "markben@example.com",
                Address = "137 Moore St",
                DateOfBirth = new DateTime(2000, 1, 1),
                CountryId = countryResponse2.CountryId,
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = false,
            };
            PersonAddRequest personAddRequest3 = new()
            {
                PersonName = "Rita Moose",
                Email = "ritamoose@example.com",
                Address = "701 Thomson St",
                DateOfBirth = new DateTime(2004, 1, 1),
                CountryId = countryResponse1.CountryId,
                Gender = GenderOptions.Female,
                ReceiveNewsLetters = true,
            };

            List<PersonAddRequest> persons_requests = new()
            {
                personAddRequest1,
                personAddRequest2,
                personAddRequest3,
            };

            List<PersonResponse> persons_response = [];

            foreach (PersonAddRequest personRequest in persons_requests)
            {
                PersonResponse personResponse = await _personsService.AddPerson(personRequest);

                persons_response.Add(personResponse);
            }

            List<PersonResponse> fetchedPersons = await _personsService.GetAllPersons();

            // Print expected persons_reponse
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse personResponse in persons_response)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            // Print Actual persons_reponse
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse personActual in fetchedPersons)
            {
                _testOutputHelper.WriteLine(personActual.ToString());
            }

            // Assert
            Assert.NotNull(fetchedPersons);
            Assert.Equal(persons_response.Count, fetchedPersons.Count);
            Assert.Equal(persons_response, fetchedPersons);

            foreach (PersonResponse person in persons_response)
            {
                Assert.Contains(person, fetchedPersons);
            }
        }
        #endregion


        #region GetFilteredPersons Tests
        // Returns a list of all persons if searchBy and searchString are empty
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText()
        {
            // Arrange
            CountryAddRequest countryRequest1 = new()
            {
                CountryName = "Canada",
            };
            CountryAddRequest countryRequest2 = new()
            {
                CountryName = "USA",
            };

            CountryResponse countryResponse1 = await _countryService.AddCountry(countryRequest1);
            CountryResponse countryResponse2 = await _countryService.AddCountry(countryRequest2);

            // Act
            PersonAddRequest personAddRequest1 = new()
            {
                PersonName = "John Doe",
                Email = "johndoe@example.com",
                Address = "123 Main St",
                DateOfBirth = new DateTime(1990, 1, 1),
                CountryId = countryResponse1.CountryId,
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true,
            };
            PersonAddRequest personAddRequest2 = new()
            {
                PersonName = "Mark Ben",
                Email = "markben@example.com",
                Address = "137 Moore St",
                DateOfBirth = new DateTime(2000, 1, 1),
                CountryId = countryResponse2.CountryId,
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = false,
            };
            PersonAddRequest personAddRequest3 = new()
            {
                PersonName = "Rita Moose",
                Email = "ritamoose@example.com",
                Address = "701 Thomson St",
                DateOfBirth = new DateTime(2004, 1, 1),
                CountryId = countryResponse1.CountryId,
                Gender = GenderOptions.Female,
                ReceiveNewsLetters = true,
            };

            List<PersonAddRequest> persons_requests = new()
            {
                personAddRequest1,
                personAddRequest2,
                personAddRequest3,
            };

            List<PersonResponse> persons_response = [];

            foreach (PersonAddRequest personRequest in persons_requests)
            {
                PersonResponse personResponse = await _personsService.AddPerson(personRequest);

                persons_response.Add(personResponse);
            }

            List<PersonResponse> filteredPersons = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "");

            // Print expected persons_reponse
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse personResponse in persons_response)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            // Print Actual persons_reponse
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse personActual in filteredPersons)
            {
                _testOutputHelper.WriteLine(personActual.ToString());
            }

            // Assert
            Assert.NotNull(filteredPersons);
            Assert.Equal(persons_response.Count, filteredPersons.Count);
            Assert.Equal(persons_response, filteredPersons);

            foreach (PersonResponse person in persons_response)
            {
                Assert.Contains(person, filteredPersons);
            }
        }

        // Add few persons and search based on personName with searchString to return matching persons
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {
            // Arrange
            CountryAddRequest countryRequest1 = new()
            {
                CountryName = "Canada",
            };
            CountryAddRequest countryRequest2 = new()
            {
                CountryName = "USA",
            };

            CountryResponse countryResponse1 = await _countryService.AddCountry(countryRequest1);
            CountryResponse countryResponse2 = await _countryService.AddCountry(countryRequest2);

            // Act
            PersonAddRequest personAddRequest1 = new()
            {
                PersonName = "John Doe",
                Email = "johndoe@example.com",
                Address = "123 Main St",
                DateOfBirth = new DateTime(1990, 1, 1),
                CountryId = countryResponse1.CountryId,
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true,
            };
            PersonAddRequest personAddRequest2 = new()
            {
                PersonName = "Mark Ben",
                Email = "markben@example.com",
                Address = "137 Moore St",
                DateOfBirth = new DateTime(2000, 1, 1),
                CountryId = countryResponse2.CountryId,
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = false,
            };
            PersonAddRequest personAddRequest3 = new()
            {
                PersonName = "Rita Maose",
                Email = "ritamaose@example.com",
                Address = "701 Thomson St",
                DateOfBirth = new DateTime(2004, 1, 1),
                CountryId = countryResponse1.CountryId,
                Gender = GenderOptions.Female,
                ReceiveNewsLetters = true,
            };

            List<PersonAddRequest> persons_requests = new()
            {
                personAddRequest1,
                personAddRequest2,
                personAddRequest3,
            };

            List<PersonResponse> persons_response = [];

            foreach (PersonAddRequest personRequest in persons_requests)
            {
                PersonResponse personResponse = await _personsService.AddPerson(personRequest);

                persons_response.Add(personResponse);
            }

            List<PersonResponse> filteredPersons = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "ma"); // searchString = 'ma'

            // Assert
            foreach (PersonResponse person in persons_response)
            {
                if (person.PersonName != null && person.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
                {
                    Assert.Contains(person, filteredPersons);
                }
            }
        }

        #endregion

        #region GetSortedPersons Tests
        // When sort based on PersonName in DESC order, persons should be sorted accordingly
        [Fact]
        public async Task GetSortedPersons()
        {
            // Arrange
            CountryAddRequest countryRequest1 = new()
            {
                CountryName = "Canada",
            };
            CountryAddRequest countryRequest2 = new()
            {
                CountryName = "USA",
            };

            CountryResponse countryResponse1 = await _countryService.AddCountry(countryRequest1);
            CountryResponse countryResponse2 = await _countryService.AddCountry(countryRequest2);

            // Act
            PersonAddRequest personAddRequest1 = new()
            {
                PersonName = "John Doe",
                Email = "johndoe@example.com",
                Address = "123 Main St",
                DateOfBirth = new DateTime(1990, 1, 1),
                CountryId = countryResponse1.CountryId,
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true,
            };
            PersonAddRequest personAddRequest2 = new()
            {
                PersonName = "Mark Ben",
                Email = "markben@example.com",
                Address = "137 Moore St",
                DateOfBirth = new DateTime(2000, 1, 1),
                CountryId = countryResponse2.CountryId,
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = false,
            };
            PersonAddRequest personAddRequest3 = new()
            {
                PersonName = "Rita Maose",
                Email = "ritamaose@example.com",
                Address = "701 Thomson St",
                DateOfBirth = new DateTime(2004, 1, 1),
                CountryId = countryResponse1.CountryId,
                Gender = GenderOptions.Female,
                ReceiveNewsLetters = true,
            };

            List<PersonAddRequest> persons_requests = new()
            {
                personAddRequest1,
                personAddRequest2,
                personAddRequest3,
            };

            List<PersonResponse> persons_response = [];

            foreach (PersonAddRequest personRequest in persons_requests)
            {
                PersonResponse personResponse = await _personsService.AddPerson(personRequest);

                persons_response.Add(personResponse);
            }
            List<PersonResponse> allPersons = await _personsService.GetAllPersons();

            List<PersonResponse> sortedPersonsDesc = await _personsService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            persons_response = persons_response.OrderByDescending(person => person.PersonName).ToList();


            // Assert
            for (int i = 0; i < persons_response.Count; i++)
            {
                Assert.Equal(persons_response[i], sortedPersonsDesc[i]);
            }
        }


        #endregion

        #region UpdatePerson Tests
        // When person is null, UpdatePerson should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPersonUpdateRequest_ThrowsArgumentNullException()
        {
            // Arrange
            PersonUpdateRequest? request = null;
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _personsService.UpdatePerson(request));
        }

        // When personId is not found, UpdatePerson should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_PersonIdNotFound_ThrowsArgumentException()
        {
            // Arrange
            PersonUpdateRequest personUpdateRequest = new()
            {
                PersonId = Guid.NewGuid(),
                PersonName = "John Doe Updated",
                Email = ""
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _personsService.UpdatePerson(personUpdateRequest));

        }

        // When PersonName is null, UpdatePerson should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_NullPersonName_ThrowsArgumentException()
        {
            // Arrange
            CountryAddRequest countryRequest = new()
            {
                CountryName = "Canada",
            };
            CountryResponse countryResponse = await _countryService.AddCountry(countryRequest);

            PersonAddRequest personAddRequest = new()
            {
                PersonName = "John Doe",
                Email = "johndoe@example.com",
                Address = "123 Main St",
                DateOfBirth = new DateTime(1990, 1, 1),
                CountryId = countryResponse.CountryId,
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true,
            };

            PersonResponse person = await _personsService.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest = person.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = null;
            personUpdateRequest.Email = "tonydon@example.com";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _personsService.UpdatePerson(personUpdateRequest));
        }

        // When valid PersonUpdateRequest is provided, UpdatePerson should return updated PersonResponse
        [Fact]
        public async Task UpdatePerson_ValidPersonUpdateRequest_ReturnsUpdatedPersonResponse()
        {
            // Arrange
            CountryAddRequest countryRequest = new()
            {
                CountryName = "Canada",
            };
            CountryResponse countryResponse = await _countryService.AddCountry(countryRequest);

            PersonAddRequest personAddRequest = new()
            {
                PersonName = "John Doe",
                Email = "johndoe@example.com",
                Address = "123 Main St",
                DateOfBirth = new DateTime(2001, 1, 1),
                CountryId = countryResponse.CountryId,
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true,
            };

            PersonResponse person = await _personsService.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest = person.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = "Tony Don";
            personUpdateRequest.Email = "tonydon@example.com";

            // Act
            PersonResponse updatedPerson = await _personsService.UpdatePerson(personUpdateRequest);

            PersonResponse? personResponseFromId = await _personsService.GetPersonByPersonId(updatedPerson.PersonId);

            // Assert
            Assert.Equal(personResponseFromId, updatedPerson);
        }

        #endregion


        #region DeletePerson Tests
        // When personId is invalid, DeletePerson should return false
        [Fact]
        public async Task DeletePerson_InvalidPersonId_ReturnsFalse()
        {
            // Arrange
            Guid? personId = Guid.NewGuid();

            // Act
            bool result = await _personsService.DeletePerson(personId);

            // Assert
            Assert.False(result);
        }

        // When valid personId is provided, DeletePerson should return true
        [Fact]
        public async Task DeletePerson_ValidPersonId_ReturnsTrue()
        {
            // Arrange
            CountryAddRequest countryRequest = new()
            {
                CountryName = "Canada",
            };
            CountryResponse countryResponse = await _countryService.AddCountry(countryRequest);

            PersonAddRequest personAddRequest = new()
            {
                PersonName = "John Doe",
                Email = "johndoe@example.com",
                Address = "123 Main St",
                DateOfBirth = new DateTime(2001, 1, 1),
                CountryId = countryResponse.CountryId,
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true,
            };

            PersonResponse person = await _personsService.AddPerson(personAddRequest);

            // Act
            bool isDeleted = await _personsService.DeletePerson(person.PersonId);

            // Assert
            Assert.True(isDeleted);
        }
        #endregion
    }
}
