using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ContactManagerDbContext _db;

        public CountriesService(ContactManagerDbContext contactManagerDbContext)
        {
            _db = contactManagerDbContext;
        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            ArgumentNullException.ThrowIfNull(countryAddRequest);

            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(null, nameof(countryAddRequest));
            }

            if(_db.Countries.Any(temp => temp.CountryName.Equals(countryAddRequest.CountryName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException("Country with the same name already exists.", nameof(countryAddRequest));
            }

            Country country = countryAddRequest.ToCountry();

            _db.Countries.Add(country);
            await _db.SaveChangesAsync();

            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            return await _db.Countries.Select(country => country.ToCountryResponse()).ToListAsync();
        }

        public async Task<CountryResponse?> GetCountryByCountryId(Guid? countryId)
        {
            if (countryId == null)
            {
                return null;
            }

            Country? country = await _db.Countries.FirstOrDefaultAsync(temp => temp.CountryId == countryId);

            if (country == null)
            {
                return null;
            }

            return country.ToCountryResponse();
        }
    }
}
