using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.UserModel
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } 
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Guid RoleId { get; set; }
        public string Status { get; set; } 
        public DateTime CreateAt { get; set; }
    }

    public class UserResgisterModel
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public string Phone { get; set; } = null!;
    }

    public class UserLoginModel
    {
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }
}
