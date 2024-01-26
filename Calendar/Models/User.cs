using Calendar.Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Calendar.Models
{
    public class User
    {
        public int Id { get; set; }

        [DisplayName("Company Name")]
        public string CompanyName { get; set; }

        [DisplayName("Representative Name")]
        public string RepresentativeName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [DisplayName("Contact Number")]
        public double ContactNumber { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        [DisplayName("User type")]
        public UserTypeEnum UserType { get; set; }

        [DisplayName("Mark Inactive")]
        public bool Inactive { get; set; }
    }
}
