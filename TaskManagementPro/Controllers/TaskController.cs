
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementPro.Data;
using TaskManagementPro.DTOs;
using TaskManagementPro.Models;
using System.Security.Claims;
using TaskManagementPro.Enums;

namespace TaskManagementPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TaskController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTask(
            [FromBody] CreateTaskDTO dto)
        {
            var employee = await _context.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x =>
                    x.Id == dto.AssignedUserId);

            if (employee == null || employee.Role?.Name != "User")
            {
                return BadRequest("Invalid UserId.");
            }

            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                Status = TaskStatusEnum.Pending,
                AssignedUserId = dto.AssignedUserId,
                CreatedDate = DateTime.UtcNow
            };

            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Task created successfully.",
                taskId = task.Id
            });
        }



        [HttpGet("get-all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTasks(
            [FromQuery] string? search,
            [FromQuery] TaskStatusEnum? status,
            [FromQuery] TaskPriorityEnum? priority,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            // Pagination validation
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest("Invalid pagination values.");
            }

            // Status validation
            if (status.HasValue &&
                !Enum.IsDefined(typeof(TaskStatusEnum), status.Value))
            {
                return BadRequest("Invalid task status.");
            }

            // Priority validation
            if (priority.HasValue &&
                !Enum.IsDefined(typeof(TaskPriorityEnum), priority.Value))
            {
                return BadRequest("Invalid task priority.");
            }

            var query = _context.TaskItems
                .AsNoTracking()
                .AsQueryable();

            // Searching
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(x =>
                    x.Title.Contains(search) ||
                    x.Description.Contains(search));
            }

            // Status filtering
            if (status.HasValue)
            {
                query = query.Where(x =>
                    x.Status == status.Value);
            }

            // Priority filtering
            if (priority.HasValue)
            {
                query = query.Where(x =>
                    x.Priority == priority.Value);
            }

            // Total records after filtering
            var totalRecords = await query.CountAsync();

            // Pagination
            var tasks = await query
                .OrderByDescending(x => x.CreatedDate)
                .ThenByDescending(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.Description,
                    x.Priority,
                    x.Status,
                    x.AssignedUserId,
                    AssignedUser = x.AssignedUser.Name,
                    x.CreatedDate
                })
                .ToListAsync();

            return Ok(new
            {
                totalRecords,
                pageNumber,
                pageSize,
                totalPages = (int)Math.Ceiling(
                    (double)totalRecords / pageSize),
                data = tasks
            });
        }



        [HttpPut("update/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTask(
            int id,
            [FromBody] UpdateTaskDTO dto)
        {
            var task = await _context.TaskItems
                .FirstOrDefaultAsync(x => x.Id == id);

            if (task == null)
            {
                return NotFound("Task not found.");
            }

            // Check employee
            var employee = await _context.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x =>
                    x.Id == dto.AssignedUserId);

            if (employee == null || employee.Role?.Name != "User")
            {
                return BadRequest("Invalid  UserId.");
            }

            // Validate title and description
            if (string.IsNullOrWhiteSpace(dto.Title) ||
                string.IsNullOrWhiteSpace(dto.Description))
            {
                return BadRequest("Title and Description are required.");
            }

            // Validate Priority Enum
            if (!Enum.IsDefined(typeof(TaskPriorityEnum), dto.Priority))
            {
                return BadRequest("Invalid priority.");
            }

            // Update task
            task.Title = dto.Title.Trim();
            task.Description = dto.Description.Trim();
            task.Priority = dto.Priority;
            task.AssignedUserId = dto.AssignedUserId;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Task updated successfully.",
                taskId = task.Id
            });
        }

        [HttpGet("get-by-id/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await _context.TaskItems
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.Description,
                    x.Priority,
                    x.Status,
                    x.AssignedUserId
                })
                .FirstOrDefaultAsync();

            if (task == null)
                return NotFound(new { message = "Task not found" });

            return Ok(task);
        }


        [HttpDelete("delete/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.TaskItems
                .FirstOrDefaultAsync(x => x.Id == id);

            if (task == null)
            {
                return NotFound(new
                {
                    message = "Task not found."
                });
            }

            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Task deleted successfully.",
                taskId = id
            });
        }

        [HttpGet("my-tasks")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetMyTasks()
        {
            // Logged-in employee ka UserId JWT se milega
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);


            if (!int.TryParse(userIdValue, out int userId))
            {
                return Unauthorized(new
                {
                    message = "Invalid user token."
                });
            }

            // Sirf logged-in employee ke assigned tasks
            var tasks = await _context.TaskItems
                .AsNoTracking()
                .Where(x => x.AssignedUserId == userId)
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.Description,
                    x.Priority,
                    x.Status,
                    x.AssignedUserId,
                    x.CreatedDate
                })
                .ToListAsync();

            return Ok(tasks);
        }

        [HttpPut("update-status/{id:int}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateTaskStatus(
            int id,
            [FromBody] UpdateTaskStatusDTO dto)
        {
            // Logged-in Employee ka UserId JWT se lo
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out int userId))
            {
                return Unauthorized(new
                {
                    message = "Invalid user token."
                });
            }

            var task = await _context.TaskItems
                .FirstOrDefaultAsync(x =>
                    x.Id == id && x.AssignedUserId == userId);

            if (task == null)
            {
                return NotFound(new
                {
                    message = "Task not found or not assigned to you."
                });
            }

            // Validate Status Enum
            if (!Enum.IsDefined(typeof(TaskStatusEnum), dto.Status))
            {
                return BadRequest(new
                {
                    message = "Invalid status."
                });
            }

            // Update task status
            task.Status = dto.Status;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Task status updated successfully.",
                taskId = task.Id,
                status = task.Status
            });
        }

        [HttpGet("status-options")]
        public IActionResult GetStatusOptions()
        {
            var statuses = Enum.GetValues<TaskStatusEnum>()
                .Select(x => new
                {
                    id = (int)x,
                    name = x.ToString()
                })
                .ToList();

            return Ok(statuses);
        }

        [HttpGet("priority-options")]
        public IActionResult GetPriorityOptions()
        {
            var priorities = Enum.GetValues<TaskPriorityEnum>()
                .Select(x => new
                {
                    id = (int)x,
                    name = x.ToString()
                })
                .ToList();

            return Ok(priorities);
        }






    }
}

