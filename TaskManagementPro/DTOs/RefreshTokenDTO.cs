using System.ComponentModel.DataAnnotations;

namespace TaskManagementPro.DTOs
{
    public class RefreshTokenDTO
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
