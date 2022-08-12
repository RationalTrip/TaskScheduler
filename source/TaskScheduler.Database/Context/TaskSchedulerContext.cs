using Microsoft.EntityFrameworkCore;
using TaskScheduler.Domain;

namespace TaskScheduler.Database
{
    public class TaskSchedulerContext:DbContext
    {
        public DbSet<LoginAuth> LoginAuths { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ScheduleTask> ScheduleTasks { get; set; }

        public TaskSchedulerContext(DbContextOptions options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new LoginAuthConfiguration());

            modelBuilder.ApplyConfiguration(new ScheduleTaskConfiguration());

            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
