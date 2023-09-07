using Microsoft.EntityFrameworkCore;

namespace Calendar.Models
{
    public class CalendarDbContext : DbContext
    {
        public CalendarDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

        public DbSet<Tasks> Tasks { get; set; }

        public DbSet<TaskGenerated> TaskGenerated { get; set; }

        public DbSet<TaskStatus> TaskStatus { get; set; }

        public DbSet<Holidays> Holidays { get; set; }

        public DbSet<Users> Users { get; set; }
    }
}
