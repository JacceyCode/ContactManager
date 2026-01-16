using System;
using System.Collections.Generic;
using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using Moq;
using AutoFixture;

namespace ContactManagerTest
{
    public class CountryServiceTest
    {
        private readonly ICountriesService _countriesService;
        private readonly IFixture _fixture;

        public CountryServiceTest()
        {
            _fixture = new Fixture();
            var countriesInitialData = new List<Country>() { };

            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);

            _countriesService = new CountriesService(dbContext);
        }

        #region AddCountry Tests
        // WHen CountryAddRequest is null, AddCountry should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountryAddRequest_ThrowsArgumentNullException()
        {
            // Arrange
            CountryAddRequest? request = null;

            // Act 
            //_countriesService.AddCountry(request);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _countriesService.AddCountry(request));
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
            await Assert.ThrowsAsync<ArgumentException>(async () => await _countriesService.AddCountry(request));
        }

        // When countryName is duplicate, AddCountry should throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateCountryName_ThrowsArgumentException()
        {
            // Arrange
            CountryAddRequest request1 = _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, "USA")
                .Create();
            CountryAddRequest request2 = _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, "USA")
                .Create();

            // Act
            await _countriesService.AddCountry(request1);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await _countriesService.AddCountry(request2));
        }

        // When valid countryName is provided, AddCountry should insert country into a list and return CountryResponse with valid CountryId and CountryName
        [Fact]
        public async Task AddCountry_ValidCountryName_ReturnsCountryResponse()
        {
            // Arrange
            CountryAddRequest request = _fixture.Build<CountryAddRequest>()
                .Create();
            // Act
            CountryResponse response = await _countriesService.AddCountry(request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(request.CountryName, response.CountryName);
            Assert.NotEqual(Guid.Empty, response.CountryId);
            Assert.True(response.CountryId != Guid.Empty);
        }
        #endregion


        #region GetAllCountries Tests
        // The countries list is initially empty, GetAllCountries should return empty list
        [Fact]
        public async Task GetAllCountries_NoCountries_ReturnsEmptyList()
        {
            // Arrange
            // Act
            List<CountryResponse> countries = await _countriesService.GetAllCountries();

            // Assert
            Assert.NotNull(countries);
            Assert.Empty(countries);
        }

        // When countries are present, GetAllCountries should return list of countries
        [Fact]
        public async Task GetAllCountries_CountriesPresent_ReturnsListOfCountries()
        {
            // Arrange
            CountryAddRequest request1 = _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, "India")
                .Create();
            CountryAddRequest request2 = _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, "Germany")
                .Create();

            await _countriesService.AddCountry(request1);
            await _countriesService.AddCountry(request2);

            // Act
            List<CountryResponse> countries = await _countriesService.GetAllCountries();

            // Assert
            Assert.NotNull(countries);
            Assert.Equal(2, countries.Count);
            Assert.Contains(countries, c => c.CountryName == "India");
            Assert.Contains(countries, c => c.CountryName == "Germany");
        }
        #endregion

        #region GetCountryByCountryId Tests
        // Null countryId should throw NotImplementedException
        [Fact]
        public async Task GetCountryByCountryId_NullCountryId_ThrowsNotImplementedException()
        {
            // Arrange
            Guid? countryId = null;

            // Act & Assert
            Assert.Null(await _countriesService.GetCountryByCountryId(countryId));
        }

        // Valid countryId should return country details
        [Fact]
        public async Task GetCountryByCountryId_ValidCountryId_ReturnsCountryResponse()
        {
            // Arrange
            CountryAddRequest request = _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, "Australia")
                .Create();
            
            CountryResponse addedCountry = await _countriesService.AddCountry(request);
            Guid? countryId = addedCountry.CountryId;

            // Act
            CountryResponse? countryResponse = await _countriesService.GetCountryByCountryId(countryId);

            // Assert
            Assert.NotNull(countryResponse);
            Assert.Equal(addedCountry.CountryId, countryResponse!.CountryId);
            Assert.Equal(addedCountry.CountryName, countryResponse.CountryName);
        }
        #endregion
    }
}