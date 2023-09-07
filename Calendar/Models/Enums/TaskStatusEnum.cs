using System.ComponentModel;

namespace Calendar.Models.Enums
{
    public enum TaskStatusEnum
    {
        None = 0,
        [Description("Not started")]
        NotStarted,
        Completed,
        [Description("Not applicable")]
        NotApplicable
    }
}
