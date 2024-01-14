using Calendar.Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Calendar.Models
{
    public class Tasks
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [DisplayName("Description")]
        public string TaskDescription { get; set; }

        [DisplayName("Recurrence frequency")]
        public RecurrenceFrequencyEnum RecurrenceFrequency { get; set; }

        public int DueDays { get; set; }

        public BusinessDaysEnum BusinessDays { get; set; }

        public DueCompletionEnum DueCompletion { get; set; }

        [DisplayName("Start date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DisplayName("Applicable for NSE?")]
        public bool IsNSE { get; set; }

        [DisplayName("Delay submission penalty for NSE")]
        public int DelaySubmissionNSE { get; set; }

        [DisplayName("Non submission penalty for NSE")]
        public int NonSubmissionNSE { get; set; }

        [DisplayName("Applicable for BSE?")]
        public bool IsBSE { get; set; }

        [DisplayName("Delay submission penalty for BSE")]
        public int DelaySubmissionBSE { get; set; }

        [DisplayName("Non-submission penalty for BSE")]
        public int NonSubmissionBSE { get; set; }

        [DisplayName("Applicable for MCX?")]
        public bool IsMCX { get; set; }

        [DisplayName("Delay submission penalty for MCX")]
        public int DelaySubmissionMCX { get; set; }

        [DisplayName("Non-submission penalty for MCX")]
        public int NonSubmissionMCX { get; set; }

        [DisplayName("Applicable for NCDEX?")]
        public bool IsNCDEX { get; set; }

        [DisplayName("Delay submission penalty for NCDEX")]
        public int DelaySubmissionNCDEX { get; set; }

        [DisplayName("Non-submission penalty for NCDEX")]
        public int NonSubmissionNCDEX { get; set; }

        [DisplayName("Applicable for CDSL?")]
        public bool IsCDSL { get; set; }

        [DisplayName("Delay submission penalty for CDSL")]
        public int DelaySubmissionCDSL { get; set; }

        [DisplayName("Non-submission penalty for CDSL")]
        public int NonSubmissionCDSL { get; set; }

        [DisplayName("Applicable for NSDL?")]
        public bool IsNSDL { get; set; }

        [DisplayName("Delay submission penalty for NSDL")]
        public int DelaySubmissionNSDL { get; set; }

        [DisplayName("Non-submission penalty for NSDL")]
        public int NonSubmissionNSDL { get; set; }

        [DisplayName("Mark Inactive")]
        public bool MarkInactive { get; set; } = true;

        [DisplayName("Inactive date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime InactiveDate { get; set; }
    }
}
