using KpaFinAdvisors.Common.Models;
using Microsoft.EntityFrameworkCore;
using TaskStatus = KpaFinAdvisors.Common.Models.TaskStatus;

namespace KpaFinAdvisors.ComplianceCalendar
{
    public class CalendarDbContext : DbContext
    {
        public CalendarDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

        public DbSet<Tasks> Tasks { get; set; }

        public DbSet<TaskGenerated> TaskGenerated { get; set; }

        public DbSet<TaskStatus> TaskStatus { get; set; }

        public DbSet<Holidays> Holidays { get; set; }

        public DbSet<User> Users { get; set; }
    }
}
