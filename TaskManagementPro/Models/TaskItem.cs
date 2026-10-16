using TaskManagementPro.Enums;

namespace TaskManagementPro.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;


        public TaskPriorityEnum Priority { get; set; }

        public TaskStatusEnum Status { get; set; }
            = TaskStatusEnum.Pending;


        public int? AssignedUserId { get; set; }

        public User? AssignedUser { get; set; }

        public DateTime CreatedDate { get; set; }
            = DateTime.UtcNow;
    }
}
