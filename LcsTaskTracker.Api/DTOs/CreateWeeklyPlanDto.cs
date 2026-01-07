namespace LcsTaskTracker.Api.DTOs
{
    public class CreateWeeklyPlanDto
    {
        public DateTime WeekStartDate { get; set; }
        public string PlannedTasks { get; set; }
    }
}
