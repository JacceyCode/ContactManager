using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly List<Country> _countries;

        public CountriesService(bool initialize = true)
        {
            _countries = new List<Country>();
            if (initialize)
            {                 _countries.AddRange(new List<Country>() { 
                new() { CountryId = Guid.Parse("E7B91144-9D3B-42C0-B3E9-CD9C308E2F5D"), CountryName = "India" },

                new() { CountryId = Guid.Parse("C014565A-F840-4BEC-915E-FEEDF96BF421"), CountryName = "USA" },

                new() { CountryId = Guid.Parse("5D797884-CE65-4FA7-B3EC-BB01E5858B39"), CountryName = "UK" },

                new() { CountryId = Guid.Parse("AB7CC0EA-528B-415F-8975-5A0177F7AF64"), CountryName = "Australia" },

                new() { CountryId = Guid.Parse("04E97365-F1DB-424F-B900-E2BE2A19DB71"), CountryName = "Nigeria" }
                });
            }
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
