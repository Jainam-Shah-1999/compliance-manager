using Calendar.Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [DisplayName("BSE")]
        public TaskStatusEnum BSEStatus { get; set; }

        [DisplayName("NSE")]
        public TaskStatusEnum NSEStatus { get; set; }

        [DisplayName("MCX")]
        public TaskStatusEnum MCXStatus { get; set; }

        [DisplayName("NCDEX")]
        public TaskStatusEnum NCDEXStatus { get; set; }

        [DisplayName("CDSL")]
        public TaskStatusEnum CDSLStatus { get; set; }

        [DisplayName("NSDL")]
        public TaskStatusEnum NSDLStatus { get; set; }

        public int TaskStatusId { get; set; }

        public string UserName {  get; set; }
    }
}
