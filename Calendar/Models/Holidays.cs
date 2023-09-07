
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Calendar.Models
{
    public class Holidays
    {
        [Key]
        public int HolidayId { get; set; }

        [DisplayName("Holiday date")]
        public DateTime HolidayDate { get; set; }

        [DisplayName("Holiday name")]
        public string HolidayName { get; set;}
    }
}
