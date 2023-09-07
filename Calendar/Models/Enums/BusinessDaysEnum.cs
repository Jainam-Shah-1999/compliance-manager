using System.ComponentModel;

namespace Calendar.Models.Enums
{
    public enum BusinessDaysEnum
    {
        [Description("Trading days")]
        TradingDays = 1,
        [Description("Calendar days")]
        CalendarDays = 2,
    }
}
