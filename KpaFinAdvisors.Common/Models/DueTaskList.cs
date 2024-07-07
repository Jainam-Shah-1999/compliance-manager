namespace KpaFinAdvisors.Common.Models
{
    public class DueTaskList
    {
        public IEnumerable<TaskWithStatus> PastDue { get; set; }

        public int PastDueCount { get; set; } = 0;

        public IEnumerable<TaskWithStatus> DueToday { get; set; }

        public int DueTodayCount { get; set; }

        public IEnumerable<TaskWithStatus> DueThisWeek { get; set; }

        public int DueThisWeekCount { get; set; }

        public IEnumerable<TaskWithStatus> DueThisMonth { get; set; }

        public IEnumerable<TaskWithStatus> DueNextSixMonth { get; set; }

        public IEnumerable<TaskWithStatus> DueThisYear { get; set; }

        public IEnumerable<TaskWithStatus> FilteredTasks { get; set; }
    }
}
