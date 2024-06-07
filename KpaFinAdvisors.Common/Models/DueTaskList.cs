namespace KpaFinAdvisors.Common.Models
{
    public class DueTaskList
    {
        public IEnumerable<TaskWithStatus> PastDue { get; set; }

        public IEnumerable<TaskWithStatus> DueToday { get; set; }

        public IEnumerable<TaskWithStatus> DueThisWeek { get; set; }

        public IEnumerable<TaskWithStatus> DueThisMonth { get; set; }

        public IEnumerable<TaskWithStatus> DueNextSixMonth { get; set; }

        public IEnumerable<TaskWithStatus> DueThisYear { get; set; }

        public IEnumerable<TaskWithStatus> FilteredTasks { get; set; }
    }
}
