namespace LcsTaskTracker.Api.Models
{
    public class WeeklyPlan
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public DateTime WeekStartDate { get; set; } // Monday
        public string PlannedTasks { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public User User { get; set; }
    }
}
