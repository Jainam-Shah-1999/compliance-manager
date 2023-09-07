using Calendar.Models.Enums;
using System.ComponentModel;

namespace Calendar.Models
{
    public class Users
    {
        public int Id { get; set; }

        [DisplayName("First name")]
        public string FirstName { get; set; }

        [DisplayName("Last name")]
        public string LastName { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        [DisplayName("User type")]
        public UserTypeEnum UserType { get; set; }
    }
}
