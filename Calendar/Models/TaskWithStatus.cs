using Calendar.Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Calendar.Models
{
    public class TaskWithStatus
    {
        public int OriginalTaskId { get; set; }

        [ForeignKey("Tasks")]
        public int GeneratedTaskId { get; set; }

        public string Name { get; set; }

        [DisplayName("Description")]
        public string TaskDescription { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [Description("Task status")]
        public TaskStatusEnum TaskStatus { get; set; }
    }
}
