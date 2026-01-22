using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mentora.Application.DTOs.Auth
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Token { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string NewPassword { get; set; } = null!;
    }
}