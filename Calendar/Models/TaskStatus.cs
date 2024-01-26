using Calendar.Models.Enums;
using System.ComponentModel;
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
        [DisplayName("BSE")]
        public TaskStatusEnum BSEStatus { get; set; }
        [DisplayName("No. of days submission delayed to BSE")]
        public int BSEDelayDays { get; set; }
        [DisplayName("NSE")]
        public TaskStatusEnum NSEStatus { get; set; }
        [DisplayName("No. of days submission delayed to NSE")]
        public int NSEDelayDays { get; set; }
        [DisplayName("MCX")]
        public TaskStatusEnum MCXStatus { get; set; }
        [DisplayName("No. of days submission delayed to MCX")]
        public int MCXDelayDays { get; set; }
        [DisplayName("NCDEX")]
        public TaskStatusEnum NCDEXStatus { get; set; }
        [DisplayName("No. of days submission delayed to NCDEX")]
        public int NCDEXDelayDays { get; set; }
        [DisplayName("CDSL")]
        public TaskStatusEnum CDSLStatus { get; set; }
        [DisplayName("No. of days submission delayed to CDSL")]
        public int CDSLDelayDays { get; set; }
        [DisplayName("NSDL")]
        public TaskStatusEnum NSDLStatus { get; set; }
        [DisplayName("No. of days submission delayed to NSDL")]
        public int NSDLDelayDays { get; set; }
        public int UserId { get; set; }

        [NotMapped]
        public string FormSubmittedFrom { get; set; }
    }
}
