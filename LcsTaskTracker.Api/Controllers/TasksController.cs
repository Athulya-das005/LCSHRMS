using System.Security.Claims;
using LcsTaskTracker.Api.Data;
using LcsTaskTracker.Api.DTOs;
using LcsTaskTracker.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LcsTaskTracker.Api.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    [Authorize] // 🔒 JWT REQUIRED
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult AddTask(CreateTaskDto dto)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            if (dto.EndTime <= dto.StartTime)
                return BadRequest("EndTime must be greater than StartTime");

            var task = new DailyTask
            {
                TaskDate = dto.TaskDate,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                TotalHours = (decimal)(dto.EndTime - dto.StartTime).TotalHours,
                TaskType = dto.TaskType,
                Description = dto.Description,
                Status = dto.Status,
                UserId = userId
            };

            _context.DailyTasks.Add(task);
            _context.SaveChanges();

            return Ok(task);
        }

        [HttpGet("me")]
        public IActionResult GetMyTasks(
     [FromQuery] DateTime? fromDate,
     [FromQuery] DateTime? toDate,
     [FromQuery] string? taskType)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var query = _context.DailyTasks
                .Where(x => x.UserId == userId)
                .AsQueryable();

            // ✅ Apply FromDate filter
            if (fromDate.HasValue)
            {
                query = query.Where(x => x.TaskDate >= fromDate.Value.Date);
            }

            // ✅ Apply ToDate filter
            if (toDate.HasValue)
            {
                query = query.Where(x => x.TaskDate <= toDate.Value.Date);
            }

            if (fromDate.HasValue && toDate.HasValue && fromDate > toDate)
            {
                return BadRequest("FromDate cannot be greater than ToDate");
            }

            // ✅ Apply TaskType filter
            if (!string.IsNullOrWhiteSpace(taskType))
            {
                query = query.Where(x => x.TaskType == taskType);
            }

            var tasks = query
                .OrderByDescending(x => x.TaskDate)
                .ToList();

            return Ok(tasks);
        }

        [Authorize]
        [HttpPost("weekly-plan")]
        public IActionResult AddWeeklyPlan(CreateWeeklyPlanDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            if (dto.WeekStartDate.DayOfWeek != DayOfWeek.Monday)
                return BadRequest("Weekly plan can be added only on Monday");

            var exists = _context.WeeklyPlans.Any(x =>
                x.UserId == userId &&
                x.WeekStartDate.Date == dto.WeekStartDate.Date);

            if (exists)
                return BadRequest("Weekly plan already submitted for this week");

            var plan = new WeeklyPlan
            {
                UserId = userId,
                WeekStartDate = dto.WeekStartDate.Date,
                PlannedTasks = dto.PlannedTasks
            };

            _context.WeeklyPlans.Add(plan);
            _context.SaveChanges();

            return Ok(plan);
        }

        //Admin 

        //[Authorize(Roles = "Admin")]
        //[HttpGet("employees")]
        //public IActionResult GetEmployees()
        //{
        //    var employees = _context.Users
        //        .Where(u => u.IsActive)
        //        .Select(u => new
        //        {
        //            u.Id,
        //            Name = u.Username,   
        //            Position = u.Role   // 👈 Job role / position
        //        })
        //        .ToList();

        //    return Ok(employees);
        //}
        [Authorize(Roles = "Admin")]
        [HttpGet("employees")]
        public IActionResult GetEmployees()
        {
            var employees = _context.Users
                .Where(u => u.IsActive && u.Role == "Employee")
                .Select(u => new
                {
                    u.Id,
                    Name = u.Username,
                    Position = u.Role,

                    // ✅ Total working hours (sum of task hours)
                    WorkingHours = _context.DailyTasks
                        .Where(t => t.UserId == u.Id)
                        .Sum(t => (double?)t.TotalHours) ?? 0
                })
                .ToList();

            return Ok(employees);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("employees/{employeeId}/tasks/daily")]
        public IActionResult GetEmployeeDailyTasks(int employeeId, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.DailyTasks
                .Where(x => x.UserId == employeeId);

            if (fromDate.HasValue)
                query = query.Where(x => x.TaskDate >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(x => x.TaskDate <= toDate.Value.Date);

            var result = query
                .OrderByDescending(x => x.TaskDate)
                .ToList();

            return Ok(result);
        }

        [HttpGet("employees/{employeeId}/weekly-plans")]
        public IActionResult GetEmployeeWeeklyPlans(int employeeId)
        {
            var plans = _context.WeeklyPlans
                .Where(x => x.UserId == employeeId)
                .OrderByDescending(x => x.WeekStartDate)
                .ToList();

            return Ok(plans);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("project/tasks-by-status")]
        public IActionResult GetProjectTasksGroupedByStatus()
        {
            var result = _context.DailyTasks
                .Join(
                    _context.Users,
                    task => task.UserId,
                    user => user.Id,
                    (task, user) => new
                    {
                        task.Status,
                        Task = new
                        {
                            task.Id,
                            task.TaskDate,
                            task.TaskType,
                            task.Description,
                            task.TotalHours,
                            EmployeeName = user.Username
                        }
                    }
                )
                .GroupBy(x => x.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count(),
                    Tasks = g.Select(x => x.Task).ToList()
                })
                .ToList();

            return Ok(result);
        }
    }
}

