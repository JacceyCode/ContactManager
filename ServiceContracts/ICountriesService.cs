using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Defines a contract for services that provide country-related operations.
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Adds a new country to the system using the specified country details.
        /// </summary>
        /// <param name="country">An object containing the details of the country to add. May be null if no country information is provided.</param>
        /// <returns>A CountryResponse object containing the result of the add operation, including any validation errors or the
        /// details of the newly added country.</returns>
        CountryResponse AddCountry(CountryAddRequest? country);


        /// <summary>
        /// Retrieves a list of all available countries.
        /// </summary>
        /// <returns>A list of <see cref="CountryResponse"/> objects representing all countries. The list will be empty if no
        /// countries are available.</returns>
        List<CountryResponse> GetAllCountries();

        /// <summary>
        /// Retrieves country information for the specified country identifier.
        /// </summary>
        /// <param name="countryId">The unique identifier of the country to retrieve. If null, the method may return a default or empty result
        /// depending on implementation.</param>
        /// <returns>A CountryResponse object containing details of the country that matches the specified identifier, or null if
        /// no matching country is found.</returns>
        CountryResponse? GetCountryByCountryId(Guid? countryId);
    }
}
