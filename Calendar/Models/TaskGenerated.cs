using System.ComponentModel.DataAnnotations.Schema;

namespace Calendar.Models
{
    public class TaskGenerated
    {
        public int Id { get; set; }

        [ForeignKey("Tasks")]
        public int OriginalTaskId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
