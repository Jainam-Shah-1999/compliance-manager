using System.ComponentModel;

namespace Calendar.Models.Enums
{
    public enum RecurranceFrequencyEnum
    {
        Daily,
        Weekly,
        [Description("Bi-weekly")]
        BiWeekly,
        Monthly,
        [Description("Bi-monthly")]
        BiMonthly,
        Quaterly,
        [Description("Half-yearly")]
        HalfYearly,
        Yearly
    }
}
