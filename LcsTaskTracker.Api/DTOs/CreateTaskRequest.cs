namespace LcsTaskTracker.Api.DTOs
{
    public class CreateTaskRequest
    {
        public DateTime TaskDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string TaskType { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }
}
