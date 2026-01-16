using System;
using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;
using AutoFixture;

namespace ContactManagerTest
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();

            var countriesInitialData = new List<Country>() { };
            var personsInitialData = new List<Person>() { };

            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
            dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);

            _countriesService = new CountriesService(dbContext);

            _personsService = new PersonsService(dbContext,
                _countriesService
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
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(p => p.PersonName, null as string)
                .Create();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _personsService.AddPerson(personAddRequest));
        }

        // When valid PersonAddRequest is provided, AddPerson should return PersonResponse with same details
        [Fact]
        public async Task AddPerson_ValidPersonAddRequest_ReturnsPersonResponse()
        {
            // Arrange
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "some@example.com")
                .Create();

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
            CountryAddRequest countryRequest = _fixture.Create<CountryAddRequest>();
            CountryResponse countryResponse = await _countriesService.AddCountry(countryRequest);

            // Act
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "johndoe@example.com")
                .Create();

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
            CountryAddRequest countryRequest1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest countryRequest2 = _fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryRequest2);

            // Act
            PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "johndoe@example.com")
                .Create();
            PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "markben@example.com")
                .Create();
            PersonAddRequest personAddRequest3 = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "ritamoose@example.com")
                .Create();

            List <PersonAddRequest> persons_requests = new()
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
            CountryAddRequest countryRequest1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest countryRequest2 = _fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryRequest2);

            // Act
            PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "johndoe@example.com")
                .Create();
            PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "markben@example.com")
                .Create();
            PersonAddRequest personAddRequest3 = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "ritamoose@example.com")
                .Create();

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
            CountryAddRequest countryRequest1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest countryRequest2 = _fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryRequest2);

            // Act
            PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
                .With(p => p.PersonName, "John Doe")
                .With(p => p.Email, "johndoe@example.com")
                .With(p => p.CountryId, countryResponse1.CountryId)
                .Create();
            PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
                .With(p => p.PersonName, "Mark Ben")
                .With(p => p.Email, "markben@example.com")
                .With(p => p.CountryId, countryResponse2.CountryId)
                .Create();
            PersonAddRequest personAddRequest3 = _fixture.Build<PersonAddRequest>()
                .With(p => p.PersonName, "Rita Maose")
                .With(p => p.Email, "ritamoose@example.com")
                .With(p => p.CountryId, countryResponse1.CountryId)
                .Create();

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
            CountryAddRequest countryRequest1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest countryRequest2 = _fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryRequest2);

            // Act
            PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
                .With(p => p.PersonName, "John Doe")
                .With(p => p.Email, "johndoe@example.com")
                .With(p => p.CountryId, countryResponse1.CountryId)
                .Create();
            PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
                .With(p => p.PersonName, "Mark Ben")
                .With(p => p.Email, "markben@example.com")
                .With(p => p.CountryId, countryResponse2.CountryId)
                .Create();
            PersonAddRequest personAddRequest3 = _fixture.Build<PersonAddRequest>()
                .With(p => p.PersonName, "Rita Maose")
                .With(p => p.Email, "ritamoose@example.com")
                .With(p => p.CountryId, countryResponse1.CountryId)
                .Create();

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
            PersonUpdateRequest personUpdateRequest = _fixture.Build<PersonUpdateRequest>()
                .Create();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _personsService.UpdatePerson(personUpdateRequest));

        }

        // When PersonName is null, UpdatePerson should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_NullPersonName_ThrowsArgumentException()
        {
            // Arrange
            CountryAddRequest countryRequest = _fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse = await _countriesService.AddCountry(countryRequest);

            // Act
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(p => p.PersonName, "John Doe")
                .With(p => p.Email, "johndoe@example.com")
                .With(p => p.CountryId, countryResponse.CountryId)
                .Create();

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
            CountryAddRequest countryRequest = _fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse = await _countriesService.AddCountry(countryRequest);

            // Act
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(p => p.PersonName, "John Doe")
                .With(p => p.Email, "johndoe@example.com")
                .With(p => p.CountryId, countryResponse.CountryId)
                .Create();

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
            CountryAddRequest countryRequest = _fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse = await _countriesService.AddCountry(countryRequest);

            // Act
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(p => p.PersonName, "John Doe")
                .With(p => p.Email, "johndoe@example.com")
                .With(p => p.CountryId, countryResponse.CountryId)
                .Create();

            PersonResponse person = await _personsService.AddPerson(personAddRequest);

            // Act
            bool isDeleted = await _personsService.DeletePerson(person.PersonId);

            // Assert
            Assert.True(isDeleted);
        }
        #endregion
    }
}
