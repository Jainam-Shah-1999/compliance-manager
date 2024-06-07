using KpaFinAdvisors.Common.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KpaFinAdvisors.Common.Models
{
    public class User
    {
        public int Id { get; set; }

        [DisplayName("Company name")]
        public string CompanyName { get; set; }

        [DisplayName("Representative name")]
        public string RepresentativeName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [DisplayName("Contact number")]
        public double ContactNumber { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        [DisplayName("User type")]
        public UserTypeEnum UserType { get; set; }

        [DisplayName("Mark inactive")]
        public bool Inactive { get; set; }
    }
}
