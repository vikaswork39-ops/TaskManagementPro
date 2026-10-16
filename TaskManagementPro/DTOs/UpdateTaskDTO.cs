using System.ComponentModel.DataAnnotations;
using TaskManagementPro.Enums;
namespace TaskManagementPro.DTOs
{
    public class UpdateTaskDTO
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [EnumDataType(typeof(TaskPriorityEnum))]
        public TaskPriorityEnum Priority { get; set; }

        [Required]
        public int AssignedUserId { get; set; }
    }
}
