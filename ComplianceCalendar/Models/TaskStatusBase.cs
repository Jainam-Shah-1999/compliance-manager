using Calendar.Models.Enums;
using System.ComponentModel;

namespace Calendar.Models
{
    public class TaskStatusBase
    {
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
    }
}
