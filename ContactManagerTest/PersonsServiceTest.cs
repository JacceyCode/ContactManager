using System;
using Azure.Core;
using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;
using System.Linq.Expressions;

namespace ContactManagerTest
{
    public class PersonsServiceTest
    {
        private readonly IPersonsService _personsService;

        private readonly Mock<IPersonsRepository> _personRepositoryMock;
        private readonly IPersonsRepository _personsRepository;

        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();

            _personRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personRepositoryMock.Object;

            
            _personsService = new PersonsService(_personsRepository);
            
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
            Func<Task> action = async () => await _personsService.AddPerson(request);

            await action.Should().ThrowAsync<ArgumentNullException>();

            //await Assert.ThrowsAsync<ArgumentNullException>(async () => await _personsService.AddPerson(request));
        }

        // When personName is null, throws ArgumentException
        [Fact]
        public async Task AddPerson_NullPersonName_ThrowsArgumentException()
        {
            // Arrange
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(p => p.PersonName, null as string)
                .Create();

            // Mock
            Person person = personAddRequest.ToPerson();
            _personRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            // Act & Assert
            Func<Task> action = async () => await _personsService.AddPerson(personAddRequest);

            await action.Should().ThrowAsync<ArgumentException>();

            //await Assert.ThrowsAsync<ArgumentException>(async () => await _personsService.AddPerson(personAddRequest));
        }

        // When valid PersonAddRequest is provided, AddPerson should return PersonResponse with same details
        [Fact]
        public async Task AddPerson_ValidPersonDetails_ToBeSuccessful()
        {
            // Arrange
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(p => p.Email, "some@example.com")
                .Create();

            // Mock
            Person mockPerson = personAddRequest.ToPerson();
            _personRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(mockPerson);
            PersonResponse mockPersonResponse = mockPerson.ToPersonResponse();

            // Act
            PersonResponse person = await _personsService.AddPerson(personAddRequest);
            mockPersonResponse.PersonId = person.PersonId;

            // Assert
            person.Should().NotBeNull();
            person.PersonId.Should().NotBe(Guid.Empty);
            person.ReceiveNewsLetters.Should().Be(personAddRequest.ReceiveNewsLetters);
            person.Should().Be(mockPersonResponse);

            //Assert.NotNull(person);
            //Assert.True(person.PersonId != Guid.Empty);
            //Assert.True(person.ReceiveNewsLetters);

            //Assert.Contains(person, persons);
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
            person.Should().BeNull();
            //Assert.Null(person);

        }

        // When valid personId is provided, GetPersonByPersonId should return PersonResponse with same personId
        [Fact]
        public async Task GetPersonByPersonId_ValidPersonId_ToBeSuccessful()
        {
            // Arrange
            Person person = _fixture.Build<Person>()
                .With(p => p.Email, "johndoe@example.com")
                .With(p => p.Country, null as Country)
                .Create();
            PersonResponse personResponse = person.ToPersonResponse();

            // Mock
            _personRepositoryMock.Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(person);


            // Act
            PersonResponse? fetchedPerson = await _personsService.GetPersonByPersonId(person.PersonId);

            // Assert
            fetchedPerson.Should().NotBeNull();
            fetchedPerson.Should().BeEquivalentTo(personResponse);
            //Assert.NotNull(addedPerson);
            //Assert.NotNull(fetchedPerson);
            //Assert.Equal(addedPerson, fetchedPerson);
        }

        #endregion


        #region GetAllPersons Tests
        // GetAllPersons should return empty list when no persons are added
        [Fact]
        public async Task GetAllPersons_NoPersonsAdded_ReturnsEmptyList()
        {
            // Mock
            List<Person> mockPersons = new List<Person>();
            _personRepositoryMock.Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(mockPersons);

            // Act
            List<PersonResponse> persons = await _personsService.GetAllPersons();

            // Assert
            persons.Should().NotBeNull();
            persons.Should().BeEmpty();
            //Assert.NotNull(persons);
            //Assert.Empty(persons);
        }

        // When multiple persons are added, GetAllPersons should return all added persons
        [Fact]
        public async Task GetAllPersons_FewPersonsAdded_ReturnsAllPersons()
        {
            // Arrange
            List<Person> mockPersons = new List<Person>() {
             _fixture.Build<Person>()
                .With(p => p.Email, "johndoe@example.com")
                .With(p => p.Country, null as Country)
                .Create(),

            _fixture.Build<Person>()
                .With(p => p.Email, "markben@example.com")
                .With(p => p.Country, null as Country)
                .Create(),

            _fixture.Build<Person>()
                .With(p => p.Email, "ritamoose@example.com")
                .With(p => p.Country, null as Country)
                .Create()
            };

            
            List<PersonResponse> persons_response_expected = mockPersons.Select(person => person.ToPersonResponse()).ToList();

            // Mock
            _personRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(mockPersons);

            // Act
            List<PersonResponse> fetchedPersons = await _personsService.GetAllPersons();

            // Print expected persons_reponse
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse personResponse in persons_response_expected)
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
            fetchedPersons.Should().NotBeNull();
            fetchedPersons.Should().HaveCount(persons_response_expected.Count);
            fetchedPersons.Should().BeEquivalentTo(persons_response_expected);
            //Assert.NotNull(fetchedPersons);
            //Assert.Equal(persons_response.Count, fetchedPersons.Count);
            //Assert.Equal(persons_response, fetchedPersons);

            //foreach (PersonResponse person in persons_response)
            //{
            //    Assert.Contains(person, fetchedPersons);
            //}
        }
        #endregion


        #region GetFilteredPersons Tests
        // Returns a list of all persons if searchBy and searchString are empty
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText()
        {
            // Arrange
            List<Person> mockPersons = new List<Person>() {
             _fixture.Build<Person>()
                .With(p => p.Email, "johndoe@example.com")
                .With(p => p.Country, null as Country)
                .Create(),

            _fixture.Build<Person>()
                .With(p => p.Email, "markben@example.com")
                .With(p => p.Country, null as Country)
                .Create(),

            _fixture.Build<Person>()
                .With(p => p.Email, "ritamoose@example.com")
                .With(p => p.Country, null as Country)
                .Create()
            };


            List<PersonResponse> persons_response_expected = mockPersons.Select(person => person.ToPersonResponse()).ToList();

            // Mock
            _personRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(mockPersons);


            // Act
            List<PersonResponse> filteredPersons = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "");

            // Print Actual persons_reponse
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse personActual in filteredPersons)
            {
                _testOutputHelper.WriteLine(personActual.ToString());
            }

            // Assert
            filteredPersons.Should().NotBeNull();
            filteredPersons.Should().HaveCount(persons_response_expected.Count);
            filteredPersons.Should().BeEquivalentTo(persons_response_expected);
            //Assert.NotNull(filteredPersons);
            //Assert.Equal(persons_response.Count, filteredPersons.Count);
            //Assert.Equal(persons_response, filteredPersons);

            //foreach (PersonResponse person in persons_response)
            //{
            //    Assert.Contains(person, filteredPersons);
            //}
        }

        // Add few persons and search based on personName with searchString to return matching persons
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {
            // Arrange
            List<Person> mockPersons = new List<Person>() {
             _fixture.Build<Person>()
                .With(p => p.Email, "johndoe1@example.com")
                .With(p => p.Country, null as Country)
                .Create(),

            _fixture.Build<Person>()
                .With(p => p.Email, "johndoe2@example.com")
                .With(p => p.Country, null as Country)
                .Create(),

            _fixture.Build<Person>()
                .With(p => p.Email, "johndoe3@example.com")
                .With(p => p.Country, null as Country)
                .Create()
            };


            List<PersonResponse> persons_response_expected = mockPersons.Select(person => person.ToPersonResponse()).ToList();

            // Mock
            _personRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(mockPersons);


            // Act
            List<PersonResponse> filteredPersons = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "jo");

            // Print Actual persons_reponse
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse personActual in filteredPersons)
            {
                _testOutputHelper.WriteLine(personActual.ToString());
            }

            // Assert
            filteredPersons.Should().NotBeNull();
            filteredPersons.Should().HaveCount(persons_response_expected.Count);
            filteredPersons.Should().BeEquivalentTo(persons_response_expected);
        }

        #endregion

        #region GetSortedPersons Tests
        // When sort based on PersonName in DESC order, persons should be sorted accordingly
        [Fact]
        public async Task GetSortedPersons_ToBeSuccessful()
        {
            // Arrange
            List<Person> mockPersons = new List<Person>() {
             _fixture.Build<Person>()
                .With(p => p.Email, "johndoe1@example.com")
                .With(p => p.Country, null as Country)
                .Create(),

            _fixture.Build<Person>()
                .With(p => p.Email, "johndoe2@example.com")
                .With(p => p.Country, null as Country)
                .Create(),

            _fixture.Build<Person>()
                .With(p => p.Email, "johndoe3@example.com")
                .With(p => p.Country, null as Country)
                .Create()
            };


            List<PersonResponse> persons_response_expected = mockPersons.Select(person => person.ToPersonResponse()).ToList();

            // Mock
            _personRepositoryMock.Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(mockPersons);

            // Act
            List<PersonResponse> allPersons = await _personsService.GetAllPersons();

            List<PersonResponse> sortedPersonsDesc = await _personsService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            // Assert
            //sortedPersonsDesc.Should().BeEquivalentTo(persons_response);
            sortedPersonsDesc.Should().BeInDescendingOrder(person => person.PersonName);
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
            Func<Task> action = async () => await _personsService.UpdatePerson(request);

            await action.Should().ThrowAsync<ArgumentNullException>();


            //await Assert.ThrowsAsync<ArgumentNullException>(async () => await _personsService.UpdatePerson(request));
        }

        // When personId is not found, UpdatePerson should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_PersonIdNotFound_ThrowsArgumentException()
        {
            // Arrange
            PersonUpdateRequest personUpdateRequest = _fixture.Build<PersonUpdateRequest>()
                .Create();

            // Act & Assert
            Func<Task> action = async () => await _personsService.UpdatePerson(personUpdateRequest);

            await action.Should().ThrowAsync<ArgumentException>();


            //await Assert.ThrowsAsync<ArgumentException>(async () => await _personsService.UpdatePerson(personUpdateRequest));

        }

        // When PersonName is null, UpdatePerson should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_NullPersonName_ThrowsArgumentException()
        {
            // Arrange
            Person person = _fixture.Build<Person>()
                .With(p => p.PersonName, null as string)
                .With(p => p.Email, "johndoe@example.com")
                .With(p => p.Country, null as Country)
                .With(p => p.Gender, GenderOptions.Male.ToString())
                .Create();

            PersonResponse person_response_expected = person.ToPersonResponse();

            PersonUpdateRequest personUpdateRequest = person_response_expected.ToPersonUpdateRequest();

            // Act & Assert
            Func<Task> action = async () => await _personsService.UpdatePerson(personUpdateRequest);

            await action.Should().ThrowAsync<ArgumentException>();

            //await Assert.ThrowsAsync<ArgumentException>(async () => await _personsService.UpdatePerson(personUpdateRequest));
        }

        // When valid PersonUpdateRequest is provided, UpdatePerson should return updated PersonResponse
        [Fact]
        public async Task UpdatePerson_ValidPersonUpdateRequest_ToBeSuccessful()
        {
            // Arrange
            Person person = _fixture.Build<Person>()
                .With(p => p.PersonName, "John Doe")
                .With(p => p.Email, "johndoe@example.com")
                .With(p => p.Country, null as Country)
                .With(p => p.Gender, GenderOptions.Male.ToString())
                .Create();

            PersonResponse person_response_expected = person.ToPersonResponse();

            PersonUpdateRequest personUpdateRequest = person_response_expected.ToPersonUpdateRequest();
            //personUpdateRequest.PersonName = "Tony Don";
            //personUpdateRequest.Email = "tonydon@example.com";

            // Mock
            _personRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>()))
                .ReturnsAsync(person);
            _personRepositoryMock.Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(person);


            // Act
            PersonResponse updatedPerson = await _personsService.UpdatePerson(personUpdateRequest);

            // Assert
            updatedPerson.Should().Be(person_response_expected);
            //Assert.Equal(personResponseFromId, updatedPerson);
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
            result.Should().BeFalse();
            //Assert.False(result);
        }

        // When valid personId is provided, DeletePerson should return true
        [Fact]
        public async Task DeletePerson_ValidPersonId_ReturnsTrue()
        {
            // Arrange
            Person person = _fixture.Build<Person>()
                .With(p => p.PersonName, "John Doe")
                .With(p => p.Email, "johndoe@example.com")
                .With(p => p.Country, null as Country)
                .With(p => p.Gender, GenderOptions.Female.ToString())
                .Create();

            // Mock
            _personRepositoryMock.Setup(temp => temp.DeletePersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            _personRepositoryMock.Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            // Act
            bool isDeleted = await _personsService.DeletePerson(person.PersonId);

            // Assert
            isDeleted.Should().BeTrue();
            //Assert.True(isDeleted);
        }
        #endregion
    }
}
