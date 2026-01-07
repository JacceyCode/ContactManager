using System;
using System.Collections.Generic;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace ContactManagerTest
{
    public class CountryServiceTest
    {
        private readonly ICountriesService _countriesService;

        public CountryServiceTest()
        {
            _countriesService = new CountriesService();
        }

        #region AddCountry Tests
        // WHen CountryAddRequest is null, AddCountry should throw ArgumentNullException
        [Fact]
        public void AddCountry_NullCountryAddRequest_ThrowsArgumentNullException()
        {
            // Arrange
            CountryAddRequest? request = null;

            // Act 
            //_countriesService.AddCountry(request);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _countriesService.AddCountry(request));
        }

        // When countryName is null or empty, AddCountry should throw ArgumentException
        [Fact]
        public void AddCountry_NullCountryName_ThrowsArgumentException()
        {
            // Arrange
            CountryAddRequest request = new()
            {
                CountryName = null
            };
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _countriesService.AddCountry(request));
        }

        // When countryName is duplicate, AddCountry should throw ArgumentException
        [Fact]
        public void AddCountry_DuplicateCountryName_ThrowsArgumentException()
        {
            // Arrange
            CountryAddRequest request1 = new()
            {
                CountryName = "USA"
            };
            CountryAddRequest request2 = new()
            {
                CountryName = "USA"
            };
            // Act
            _countriesService.AddCountry(request1);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _countriesService.AddCountry(request2));
        }

        // When valid countryName is provided, AddCountry should insert country into a list and return CountryResponse with valid CountryId and CountryName
        [Fact]
        public void AddCountry_ValidCountryName_ReturnsCountryResponse()
        {
            // Arrange
            CountryAddRequest request = new()
            {
                CountryName = "Canada"
            };
            // Act
            CountryResponse response = _countriesService.AddCountry(request);

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
        public void GetAllCountries_NoCountries_ReturnsEmptyList()
        {
            // Arrange
            // Act
            List<CountryResponse> countries = _countriesService.GetAllCountries();

            // Assert
            Assert.NotNull(countries);
            Assert.Empty(countries);
        }

        // When countries are present, GetAllCountries should return list of countries
        [Fact]
        public void GetAllCountries_CountriesPresent_ReturnsListOfCountries()
        {
            // Arrange
            CountryAddRequest request1 = new()
            {
                CountryName = "India"
            };
            CountryAddRequest request2 = new()
            {
                CountryName = "Germany"
            };

            _countriesService.AddCountry(request1);
            _countriesService.AddCountry(request2);

            // Act
            List<CountryResponse> countries = _countriesService.GetAllCountries();

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
        public void GetCountryByCountryId_NullCountryId_ThrowsNotImplementedException()
        {
            // Arrange
            Guid? countryId = null;

            // Act & Assert
            Assert.Null(_countriesService.GetCountryByCountryId(countryId));
        }

        // Valid countryId should return country details
        [Fact]
        public void GetCountryByCountryId_ValidCountryId_ReturnsCountryResponse()
        {
            // Arrange
            CountryAddRequest request = new()
            {
                CountryName = "Australia"
            };
            CountryResponse addedCountry = _countriesService.AddCountry(request);
            Guid? countryId = addedCountry.CountryId;

            // Act
            CountryResponse? countryResponse = _countriesService.GetCountryByCountryId(countryId);

            // Assert
            Assert.NotNull(countryResponse);
            Assert.Equal(addedCountry.CountryId, countryResponse!.CountryId);
            Assert.Equal(addedCountry.CountryName, countryResponse.CountryName);
        }
        #endregion
    }
}