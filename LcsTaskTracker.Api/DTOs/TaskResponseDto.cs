namespace LcsTaskTracker.Api.DTOs
{
    public class TaskResponseDto
    {
        public int Id { get; set; }
        public DateTime TaskDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public double TotalHours { get; set; }
        public string TaskType { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
    }
}
