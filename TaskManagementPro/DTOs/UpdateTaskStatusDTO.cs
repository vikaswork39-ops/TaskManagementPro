using System.ComponentModel.DataAnnotations;
using TaskManagementPro.Enums;

namespace TaskManagementPro.DTOs
{
    public class UpdateTaskStatusDTO
    {
        [Required]
        [EnumDataType(typeof(TaskStatusEnum))]
        public TaskStatusEnum Status { get; set; }
    }
}
