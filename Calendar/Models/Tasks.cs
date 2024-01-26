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
        public string? TaskDescription { get; set; }

        [DisplayName("Recurrence")]
        public RecurrenceFrequencyEnum RecurrenceFrequency { get; set; }

        [DisplayName("Due days")]
        public int DueDays { get; set; }

        [DisplayName("Business days")]
        public BusinessDaysEnum BusinessDays { get; set; }

        [DisplayName("Due completion")]
        public DueCompletionEnum DueCompletion { get; set; }

        [DisplayName("Start date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DisplayName("NSE?")]
        public bool IsNSE { get; set; }

        [DisplayName("Delay submission - NSE")]
        public int DelaySubmissionNSE { get; set; }

        [DisplayName("Non submission - NSE")]
        public int NonSubmissionNSE { get; set; }

        [DisplayName("BSE?")]
        public bool IsBSE { get; set; }

        [DisplayName("Delay submission - BSE")]
        public int DelaySubmissionBSE { get; set; }

        [DisplayName("Non submission - BSE")]
        public int NonSubmissionBSE { get; set; }

        [DisplayName("MCX?")]
        public bool IsMCX { get; set; }

        [DisplayName("Delay submission - MCX")]
        public int DelaySubmissionMCX { get; set; }

        [DisplayName("Non submission - MCX")]
        public int NonSubmissionMCX { get; set; }

        [DisplayName("NCDEX?")]
        public bool IsNCDEX { get; set; }

        [DisplayName("Delay submission - NCDEX")]
        public int DelaySubmissionNCDEX { get; set; }

        [DisplayName("Non submission - NCDEX")]
        public int NonSubmissionNCDEX { get; set; }

        [DisplayName("CDSL?")]
        public bool IsCDSL { get; set; }

        [DisplayName("Delay submission - CDSL")]
        public int DelaySubmissionCDSL { get; set; }

        [DisplayName("Non submission - CDSL")]
        public int NonSubmissionCDSL { get; set; }

        [DisplayName("NSDL?")]
        public bool IsNSDL { get; set; }

        [DisplayName("Delay submission - NSDL")]
        public int DelaySubmissionNSDL { get; set; }

        [DisplayName("Non submission - NSDL")]
        public int NonSubmissionNSDL { get; set; }

        [DisplayName("Mark Inactive")]
        public bool Inactive { get; set; }

        [DisplayName("Inactive date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? InactiveDate { get; set; }
    }
}
