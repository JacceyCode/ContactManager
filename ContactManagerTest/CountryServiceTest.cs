using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace ContactManagerTest
{
    public class CountryServiceTest
    {
        private readonly ICountriesService _countriesService;
        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        private readonly ICountriesRepository _countriesRepository;
        private readonly IFixture _fixture;

        public CountryServiceTest()
        {
            _fixture = new Fixture();
            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;

            _countriesService = new CountriesService(_countriesRepository);
        }

        #region AddCountry Tests
        // WHen CountryAddRequest is null, AddCountry should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountryAddRequest_ThrowsArgumentNullException()
        {
            // Arrange
            CountryAddRequest? request = null;

            // Act & Assert
            Func<Task> action = async () => await _countriesService.AddCountry(request);

            await action.Should().ThrowAsync<ArgumentNullException>();

            //await Assert.ThrowsAsync<ArgumentNullException>(async () => await _countriesService.AddCountry(request));
        }

        // When countryName is null or empty, AddCountry should throw ArgumentException
        [Fact]
        public async Task AddCountry_NullCountryName_ThrowsArgumentException()
        {
            // Arrange
            CountryAddRequest request = _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, null as string)
                .Create();
            // Act & Assert
            Func<Task> action = async () => await _countriesService.AddCountry(request);

            await action.Should().ThrowAsync<ArgumentException>();

            //await Assert.ThrowsAsync<ArgumentException>(async () => await _countriesService.AddCountry(request));
        }

        // When countryName is duplicate, AddCountry should throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateCountryName_ThrowsArgumentException()
        {
            // Arrange
            CountryAddRequest country_request = _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, "USA")
                .Create();
            Country country = country_request.ToCountry();

            // Mock
            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country);
            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
                .ReturnsAsync(country);

            // Act
            //await _countriesService.AddCountry(country_request);

            // Act & Assert
            Func<Task> action = async () => await _countriesService.AddCountry(country_request);

            await action.Should().ThrowAsync<ArgumentException>();
            //await Assert.ThrowsAsync<ArgumentException>(async () => await _countriesService.AddCountry(request2));
        }

        // When valid countryName is provided, AddCountry should insert country into a list and return CountryResponse with valid CountryId and CountryName
        [Fact]
        public async Task AddCountry_ValidCountryName_ReturnsCountryResponse()
        {
            // Arrange
            CountryAddRequest request = _fixture.Build<CountryAddRequest>()
                .Create();
            Country country = request.ToCountry();

            // Mock
            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country);
            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
                .ReturnsAsync(null as Country);

            // Act
            CountryResponse response = await _countriesService.AddCountry(request);

            // Assert
            response.Should().NotBeNull();
            response.CountryName.Should().Be(request.CountryName);
            response.CountryId.Should().NotBe(Guid.Empty);

            //Assert.NotNull(response);
            //Assert.Equal(request.CountryName, response.CountryName);
            //Assert.NotEqual(Guid.Empty, response.CountryId);
            //Assert.True(response.CountryId != Guid.Empty);
        }
        #endregion


        #region GetAllCountries Tests
        // The countries list is initially empty, GetAllCountries should return empty list
        [Fact]
        public async Task GetAllCountries_NoCountries_ReturnsEmptyList()
        {
            // Arrange
            // Mock
            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(new List<Country>());
            
            // Act
            List<CountryResponse> countries = await _countriesService.GetAllCountries();

            // Assert
            countries.Should().NotBeNull();
            countries.Should().BeEmpty();
            //Assert.NotNull(countries);
            //Assert.Empty(countries);
        }

        // When countries are present, GetAllCountries should return list of countries
        [Fact]
        public async Task GetAllCountries_CountriesPresent_ReturnsListOfCountries()
        {
            // Arrange
            List<CountryAddRequest> country_request = new List<CountryAddRequest>() {
                _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, "India")
                .Create(),
                _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, "Germany")
                .Create()
            };

            List<Country> expected_countries = country_request.Select(country => country.ToCountry()).ToList();
            List<CountryResponse> response_countries = expected_countries.Select(country => country.ToCountryResponse()).ToList();

            // Mock
            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(expected_countries);

            // Act
            List<CountryResponse> countries = await _countriesService.GetAllCountries();

            // Assert
            countries.Should().NotBeNull();
            countries.Should().HaveCount(2);
            countries.Should().Contain(c => c.CountryName == "India");
            countries.Should().Contain(c => c.CountryName == "Germany");
            countries.Should().BeEquivalentTo(response_countries);
            //Assert.NotNull(countries);
            //Assert.Equal(2, countries.Count);
            //Assert.Contains(countries, c => c.CountryName == "India");
            //Assert.Contains(countries, c => c.CountryName == "Germany");
        }
        #endregion

        #region GetCountryByCountryId Tests
        // Null countryId should throw NotImplementedException
        [Fact]
        public async Task GetCountryByCountryId_NullCountryId_ThrowsNotImplementedException()
        {
            // Arrange
            Guid? countryId = null;

            // Act
            CountryResponse? result = await _countriesService.GetCountryByCountryId(countryId);

            // Assert
            result.Should().BeNull();


            //Assert.Null(await _countriesService.GetCountryByCountryId(countryId));
        }

        // Valid countryId should return country details
        [Fact]
        public async Task GetCountryByCountryId_ValidCountryId_ReturnsCountryResponse()
        {
            // Arrange
            CountryAddRequest request = _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, "Australia")
                .Create();
            Country country = request.ToCountry();

            // Mock
            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryId(It.IsAny<Guid>()))
                .ReturnsAsync(country);

            // Act
            CountryResponse? countryResponse = await _countriesService.GetCountryByCountryId(country.CountryId);

            // Assert
            countryResponse.Should().NotBeNull();
            countryResponse!.CountryId.Should().Be(country.CountryId);
            countryResponse.CountryName.Should().Be(country.CountryName);


            //Assert.NotNull(countryResponse);
            //Assert.Equal(addedCountry.CountryId, countryResponse!.CountryId);
            //Assert.Equal(addedCountry.CountryName, countryResponse.CountryName);
        }
        #endregion
    }
}