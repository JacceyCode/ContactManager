using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
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

        public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
        {
            MemoryStream memoryStream = new MemoryStream();

            await formFile.CopyToAsync(memoryStream);
            int countriesInserted = 0;

            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets["Countries"];

                int rowCount = workSheet.Dimension.Rows;
                

                for (int row = 2; row <= rowCount; row++)
                {
                    string? cellValue = workSheet.Cells[row, 1].Value?.ToString();

                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string? countryName = cellValue;
                                        
                        if (!_db.Countries.Where(temp => temp.CountryName.ToLower() == countryName.ToLower()).Any())
                        {
                            Country country = new Country()
                            {
                                CountryName = countryName,
                            };
                            _db.Countries.Add(country);

                            await _db.SaveChangesAsync();

                            countriesInserted++;
                        }

                    }
                }
            }

            return countriesInserted;
        }
    }
}
