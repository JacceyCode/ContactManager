using Entities;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents data acess operations for managing Country entity.
    /// </summary>
    public interface ICountriesRepository
    {
        Task<Country> AddCountry(Country country);

        Task<List<Country>> GetAllCountries();

        Task<Country?> GetCountryByCountryId(Guid countryId);

        Task<Country?> GetCountryByCountryName(string countryName);

    }
}
