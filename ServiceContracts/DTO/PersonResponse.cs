using System;
using Entities;
using ServiceContracts.Enums;


namespace ServiceContracts.DTO
{
    public class PersonResponse
    {
        public Guid PersonId { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryId { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }
        public double? Age { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(PersonResponse)) return false;

            PersonResponse person = (PersonResponse)obj;


            return this.PersonId == person.PersonId &&
                   this.PersonName == person.PersonName &&
                   this.Email == person.Email &&
                   this.DateOfBirth == person.DateOfBirth &&
                   this.Gender == person.Gender &&
                   this.CountryId == person.CountryId &&
                   this.Address == person.Address &&
                   this.ReceiveNewsLetters == person.ReceiveNewsLetters;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"PersonId: {PersonId}, Person Name: {PersonName}, Email: {Email}, DateOfBirth: {DateOfBirth?.ToString("dd MMM yyyy")}, Gender: {Gender}, CountryId: {CountryId}, Country Name: {Country}, Address: {Address}, Receive News Letters: {ReceiveNewsLetters}";
        }

        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                PersonId = this.PersonId,
                PersonName = this.PersonName,
                Email = this.Email,
                DateOfBirth = this.DateOfBirth,
                Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), this.Gender, true),
                Address = this.Address,
                CountryId = this.CountryId,
                ReceiveNewsLetters = this.ReceiveNewsLetters
            };
        }
    }

    public static class PersonResponseExtensions
    {
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse()
            {
                PersonId = person.PersonId,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender,
                CountryId = person.CountryId,
                Address = person.Address,
                ReceiveNewsLetters = person.ReceiveNewsLetters,
                Age = person.DateOfBirth.HasValue ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25, 1) : null
            };
        }
    }
}
