using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Calendar.Models
{
    public class Holidays
    {
        [Key]
        public int HolidayId { get; set; }

        [DisplayName("Holiday date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime HolidayDate { get; set; }

        [DisplayName("Holiday name")]
        public string HolidayName { get; set;}
    }
}
