using System;
using Entities;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO class for country response
    /// </summary>
    public class CountryResponse
    {
        public Guid CountryId { get; set; }
        public string? CountryName { get; set; }
    }

    public static class CountryResponseExtension
    {
        /// <summary>
        /// Converts Country to CountryResponse
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse()
            {
                CountryId = country.CountryId,
                CountryName = country.CountryName
            };
        }
    }
}
