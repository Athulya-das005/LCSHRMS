using LcsTaskTracker.Api.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LcsTaskTracker.Api.Data
{
    
        public class AppDbContext : DbContext
        {
            public AppDbContext(DbContextOptions<AppDbContext> options)
                : base(options) { }

            public DbSet<User> Users { get; set; }
            public DbSet<DailyTask> DailyTasks { get; set; }
            public DbSet<WeeklyPlan> WeeklyPlans { get; set; }
        }
}
