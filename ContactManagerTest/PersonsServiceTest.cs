using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using System.Net;
using System.Reflection;
using Xunit.Abstractions;

namespace ContactManagerTest
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countryService;
        private readonly ITestOutputHelper _testOutputHelper;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _personsService = new PersonsService();
            _countryService = new CountriesService();
            _testOutputHelper = testOutputHelper;
        }


        #region AddPerson Tests
        // When person is null, AddPerson should throw ArgumentNullException
        [Fact]
        public void AddPerson_NullPersonAddRequest_ThrowsArgumentNullException()
        {
            // Arrange
            PersonAddRequest? request = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _personsService.AddPerson(request));
        }

        // When personName is null, throws ArgumentException
        [Fact]
        public void AddPerson_NullPersonName_ThrowsArgumentException()
        {
            // Arrange
            PersonAddRequest personAddRequest = new()
            {
                PersonName = null,
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _personsService.AddPerson(personAddRequest));
        }

        // When valid PersonAddRequest is provided, AddPerson should return PersonResponse with same details
        [Fact]
        public void AddPerson_ValidPersonAddRequest_ReturnsPersonResponse()
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
            PersonResponse person = _personsService.AddPerson(personAddRequest);

            List<PersonResponse> persons = _personsService.GetAllPersons();

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
        public void GetPersonByPersonId_NullPersonId()
        {
            // Arrange
            Guid? personId = null;

            // Act
            PersonResponse? person = _personsService.GetPersonByPersonId(personId);

            // Assert
            Assert.Null(person);
        }

        // When valid personId is provided, GetPersonByPersonId should return PersonResponse with same personId
        [Fact]
        public void GetPersonByPersonId_ValidPersonId_ReturnsPersonResponse()
        {
            // Arrange
            CountryAddRequest countryRequest = new()
            {
                CountryName = "Canada",
            };
            CountryResponse countryResponse = _countryService.AddCountry(countryRequest);

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

            PersonResponse addedPerson = _personsService.AddPerson(personAddRequest);

            PersonResponse? fetchedPerson = _personsService.GetPersonByPersonId(addedPerson.PersonId);

            // Assert
            Assert.NotNull(addedPerson);
            Assert.NotNull(fetchedPerson);
            Assert.Equal(addedPerson, fetchedPerson);
        }

        #endregion


        #region GetAllPersons Tests
        // GetAllPersons should return empty list when no persons are added
        [Fact]
        public void GetAllPersons_NoPersonsAdded_ReturnsEmptyList()
        {
            // Act
            List<PersonResponse> persons = _personsService.GetAllPersons();

            // Assert
            Assert.NotNull(persons);
            Assert.Empty(persons);
        }

        // When multiple persons are added, GetAllPersons should return all added persons
        [Fact]
        public void GetAllPersons_MultiplePersonsAdded_ReturnsAllPersons()
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

            CountryResponse countryResponse1 = _countryService.AddCountry(countryRequest1);
            CountryResponse countryResponse2 = _countryService.AddCountry(countryRequest2);

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
                PersonResponse personResponse = _personsService.AddPerson(personRequest);

                persons_response.Add(personResponse);
            }

            List<PersonResponse> fetchedPersons = _personsService.GetAllPersons();

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
        public void GetFilteredPersons_EmptySearchText()
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

            CountryResponse countryResponse1 = _countryService.AddCountry(countryRequest1);
            CountryResponse countryResponse2 = _countryService.AddCountry(countryRequest2);

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
                PersonResponse personResponse = _personsService.AddPerson(personRequest);

                persons_response.Add(personResponse);
            }

            List<PersonResponse> filteredPersons = _personsService.GetFilteredPersons(nameof(Person.PersonName), "");

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
        public void GetFilteredPersons_SearchByPersonName()
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

            CountryResponse countryResponse1 = _countryService.AddCountry(countryRequest1);
            CountryResponse countryResponse2 = _countryService.AddCountry(countryRequest2);

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
                PersonResponse personResponse = _personsService.AddPerson(personRequest);

                persons_response.Add(personResponse);
            }

            List<PersonResponse> filteredPersons = _personsService.GetFilteredPersons(nameof(Person.PersonName), "ma"); // searchString = 'ma'

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
        public void GetSortedPersons()
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

            CountryResponse countryResponse1 = _countryService.AddCountry(countryRequest1);
            CountryResponse countryResponse2 = _countryService.AddCountry(countryRequest2);

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
                PersonResponse personResponse = _personsService.AddPerson(personRequest);

                persons_response.Add(personResponse);
            }
            List<PersonResponse> allPersons = _personsService.GetAllPersons();

            List<PersonResponse> sortedPersonsDesc = _personsService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

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
        public void UpdatePerson_NullPersonUpdateRequest_ThrowsArgumentNullException()
        {
            // Arrange
            PersonUpdateRequest? request = null;
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _personsService.UpdatePerson(request));
        }

        // When personId is not found, UpdatePerson should throw ArgumentException
        [Fact]
        public void UpdatePerson_PersonIdNotFound_ThrowsArgumentException()
        {
            // Arrange
            PersonUpdateRequest personUpdateRequest = new()
            {
                PersonId = Guid.NewGuid(),
                PersonName = "John Doe Updated",
                Email = ""
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _personsService.UpdatePerson(personUpdateRequest));

        }

        // When PersonName is null, UpdatePerson should throw ArgumentException
        [Fact]
        public void UpdatePerson_NullPersonName_ThrowsArgumentException()
        {
            // Arrange
            CountryAddRequest countryRequest = new()
            {
                CountryName = "Canada",
            };
            CountryResponse countryResponse = _countryService.AddCountry(countryRequest);

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

            PersonResponse person = _personsService.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest = person.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = null;
            personUpdateRequest.Email = "tonydon@example.com";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _personsService.UpdatePerson(personUpdateRequest));
        }

        // When valid PersonUpdateRequest is provided, UpdatePerson should return updated PersonResponse
        [Fact]
        public void UpdatePerson_ValidPersonUpdateRequest_ReturnsUpdatedPersonResponse()
        {
            // Arrange
            CountryAddRequest countryRequest = new()
            {
                CountryName = "Canada",
            };
            CountryResponse countryResponse = _countryService.AddCountry(countryRequest);

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

            PersonResponse person = _personsService.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest = person.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = "Tony Don";
            personUpdateRequest.Email = "tonydon@example.com";

            // Act
            PersonResponse updatedPerson = _personsService.UpdatePerson(personUpdateRequest);

            PersonResponse? personResponseFromId = _personsService.GetPersonByPersonId(updatedPerson.PersonId);

            // Assert
            Assert.Equal(personResponseFromId, updatedPerson);
        }

        #endregion


        #region DeletePerson Tests
        // When personId is invalid, DeletePerson should return false
        [Fact]
        public void DeletePerson_InvalidPersonId_ReturnsFalse()
        {
            // Arrange
            Guid? personId = Guid.NewGuid();

            // Act
            bool result = _personsService.DeletePerson(personId);

            // Assert
            Assert.False(result);
        }

        // When valid personId is provided, DeletePerson should return true
        [Fact]
        public void DeletePerson_ValidPersonId_ReturnsTrue()
        {
            // Arrange
            CountryAddRequest countryRequest = new()
            {
                CountryName = "Canada",
            };
            CountryResponse countryResponse = _countryService.AddCountry(countryRequest);

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

            PersonResponse person = _personsService.AddPerson(personAddRequest);

            // Act
            bool isDeleted = _personsService.DeletePerson(person.PersonId);

            // Assert
            Assert.True(isDeleted);
        }


        #endregion
    }
}
