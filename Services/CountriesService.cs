using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly List<Country> _countries;

        public CountriesService()
        {
            _countries = new List<Country>();
        }

        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {
            ArgumentNullException.ThrowIfNull(countryAddRequest);

            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(null, nameof(countryAddRequest));
            }

            if(_countries.Any(temp => temp.CountryName.Equals(countryAddRequest.CountryName, StringComparison.CurrentCultureIgnoreCase)))
            {
                throw new ArgumentException("Country with the same name already exists.", nameof(countryAddRequest));
            }

            Country country = countryAddRequest.ToCountry();

            _countries.Add(country);

            return country.ToCountryResponse();
        }

        public List<CountryResponse> GetAllCountries()
        {
            return _countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByCountryId(Guid? countryId)
        {
            if (countryId == null)
            {
                return null;
            }

            Country? country = _countries.FirstOrDefault(temp => temp.CountryId == countryId);

            if (country == null)
            {
                return null;
            }

            return country.ToCountryResponse();
        }
    }
}
