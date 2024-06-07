using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace KpaFinAdvisors.Common.Models
{
    public class TaskStatus : TaskStatusBase
    {
        public int Id { get; set; }
        [ForeignKey("Tasks")]
        public int OriginalTaskId { get; set; }
        [ForeignKey("TaskGenerated")]
        public int GeneratedTaskId { get; set; }

        [DisplayName("No. of days submission delayed to BSE")]
        public int BSEDelayDays { get; set; }

        [DisplayName("No. of days submission delayed to NSE")]
        public int NSEDelayDays { get; set; }

        [DisplayName("No. of days submission delayed to MCX")]
        public int MCXDelayDays { get; set; }

        [DisplayName("No. of days submission delayed to NCDEX")]
        public int NCDEXDelayDays { get; set; }

        [DisplayName("No. of days submission delayed to CDSL")]
        public int CDSLDelayDays { get; set; }

        [DisplayName("No. of days submission delayed to NSDL")]
        public int NSDLDelayDays { get; set; }
        public int UserId { get; set; }

        [NotMapped]
        public string FormSubmittedFrom { get; set; }
    }
}
