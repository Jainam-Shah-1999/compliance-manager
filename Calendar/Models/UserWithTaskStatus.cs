using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Calendar.Models
{
    public class UserWithTaskStatus
    {
        public int Id { get; set; }

        [DisplayName("Company name")]
        public string CompanyName { get; set; }

        [DisplayName("Representative name")]
        public string RepresentativeName { get; set; }

        [DisplayName("Contact number")]
        public double ContactNumber { get; set; }

        [DisplayName("Past due tasks")]
        public int PastDue { get; set; }

        [DisplayName("Completed tasks")]
        public int CompletedTasks { get; set; }
    }
}
