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

        public TaskStatusEnum BSEStatus { get; set; }

        public TaskStatusEnum NSEStatus { get; set; }

        public TaskStatusEnum MCXStatus { get; set; }

        public TaskStatusEnum NCDEXStatus { get; set; }

        public TaskStatusEnum CDSLStatus { get; set; }

        public TaskStatusEnum NSDLStatus { get; set; }

        public int UserId { get; set; }
    }
}
