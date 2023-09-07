using Calendar.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Calendar.Models
{
    public class TaskStatus
    {
        public int Id { get; set; }

        [ForeignKey("Tasks")]
        public int OriginalTaskId { get; set; }

        [ForeignKey("TaskGenerated")]
        public int GeneratedTaskId { get; set; }

        public TaskStatusEnum Status { get; set; }
    }
}
