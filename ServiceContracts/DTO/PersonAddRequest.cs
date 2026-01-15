using System;
using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts.Enums;


namespace ServiceContracts.DTO
{
    /// <summary>
    /// Represents a request to add a new person.
    /// </summary>
    public class PersonAddRequest
    {
        [Required(ErrorMessage = "Person Name can't be blank.")]
        public string? PersonName { get; set; }

        [Required(ErrorMessage = "Email can't be blank.")]
        [EmailAddress(ErrorMessage = "Email should be valid.")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Please select gender")]
        public GenderOptions? Gender { get; set; }

        [Required(ErrorMessage = "Please select a country.")]
        public Guid? CountryId { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }


        public Person ToPerson()
        {
            return new Person()
            {
                PersonId = Guid.NewGuid(),
                PersonName = this.PersonName,
                Email = this.Email,
                DateOfBirth = this.DateOfBirth,
                Gender = this.Gender.ToString(),
                Address = this.Address,
                CountryId = this.CountryId,
                ReceiveNewsLetters = this.ReceiveNewsLetters
            };
        }
    }
}
